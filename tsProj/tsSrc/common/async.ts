
//#region -- run on idle tricks ------------

import { DestroyLike } from "./Destroy";
import { canceled } from "./errors";

export interface IdleDeadline {
	readonly didTimeout: boolean;
	timeRemaining(): number;
}
/**
 * Execute the callback the next time the browser is idle
 */
export let runWhenIdle: (callback: (idle: IdleDeadline) => void, timeout?: number) => DestroyLike;
/**
 * An implementation of the "idle-until-urgent"-strategy as introduced
 * here: https://philipwalton.com/articles/idle-until-urgent/
 */
 export class IdleValue<T> {

	private readonly _executor: () => void;
	private readonly _handle: DestroyLike;

	private _didRun: boolean = false;
	private _value?: T;
	private _error: unknown;

	constructor(executor: () => T) {
		this._executor = () => {
			try {
				this._value = executor();
			} catch (err) {
				this._error = err;
			} finally {
				this._didRun = true;
			}
		};
		this._handle = runWhenIdle(() => this._executor());
	}

	destroy(): void {
		this._handle.destroy();
	}

	get value(): T {
		if (!this._didRun) {
			this._handle.destroy();
			this._executor();
		}
		if (this._error) {
			throw this._error;
		}
		return this._value!;
	}
}

//#region Promises

export namespace Promises {

	export interface IResolvedPromise<T> {
		status: 'fulfilled';
		value: T;
	}

	export interface IRejectedPromise {
		status: 'rejected';
		reason: Error;
	}

	/**
	 * Interface of https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/allSettled
	 */
	interface PromiseWithAllSettled<T> {
		allSettled<T>(promises: Promise<T>[]): Promise<ReadonlyArray<IResolvedPromise<T> | IRejectedPromise>>;
	}

	/**
	 * A polyfill of `Promise.allSettled`: returns after all promises have
	 * resolved or rejected and provides access to each result or error
	 * in the order of the original passed in promises array.
	 * See: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise/allSettled
	 */
	export async function allSettled<T>(promises: Promise<T>[]): Promise<ReadonlyArray<IResolvedPromise<T> | IRejectedPromise>> {
		if (typeof (Promise as unknown as PromiseWithAllSettled<T>).allSettled === 'function') {
			return allSettledNative(promises); // in some environments we can benefit from native implementation
		}

		return allSettledShim(promises);
	}

	async function allSettledNative<T>(promises: Promise<T>[]): Promise<ReadonlyArray<IResolvedPromise<T> | IRejectedPromise>> {
		return (Promise as unknown as PromiseWithAllSettled<T>).allSettled(promises);
	}

	async function allSettledShim<T>(promises: Promise<T>[]): Promise<ReadonlyArray<IResolvedPromise<T> | IRejectedPromise>> {
		return Promise.all(promises.map(promise => (promise.then(value => {
			const fulfilled: IResolvedPromise<T> = { status: 'fulfilled', value };

			return fulfilled;
		}, error => {
			const rejected: IRejectedPromise = { status: 'rejected', reason: error };

			return rejected;
		}))));
	}

	/**
	 * A drop-in replacement for `Promise.all` with the only difference
	 * that the method awaits every promise to either fulfill or reject.
	 *
	 * Similar to `Promise.all`, only the first error will be returned
	 * if any.
	 */
	export async function settled<T>(promises: Promise<T>[]): Promise<T[]> {
		let firstError: Error | undefined = undefined;

		const result = await Promise.all(promises.map(promise => promise.then(value => value, error => {
			if (!firstError) {
				firstError = error;
			}

			return undefined; // do not rethrow so that other promises can settle
		})));

		if (typeof firstError !== 'undefined') {
			throw firstError;
		}

		return result as unknown as T[]; // cast is needed and protected by the `throw` above
	}
}

//#endregion

//#region

export type ValueCallback<T = unknown> = (value: T | Promise<T>) => void;

/**
 * Creates a promise whose resolution or rejection can be controlled imperatively.
 */
export class DeferredPromise<T> {

	private completeCallback!: ValueCallback<T>;
	private errorCallback!: (err: unknown) => void;
	private rejected = false;
	private resolved = false;

	public get isRejected() {
		return this.rejected;
	}

	public get isResolved() {
		return this.resolved;
	}

	public get isSettled() {
		return this.rejected || this.resolved;
	}

	public p: Promise<T>;

	constructor() {
		this.p = new Promise<T>((c, e) => {
			this.completeCallback = c;
			this.errorCallback = e;
		});
	}

	public complete(value: T) {
		return new Promise<void>(resolve => {
			this.completeCallback(value);
			this.resolved = true;
			resolve();
		});
	}

	public error(err: unknown) {
		return new Promise<void>(resolve => {
			this.errorCallback(err);
			this.rejected = true;
			resolve();
		});
	}

	public cancel() {
		new Promise<void>(resolve => {
			this.errorCallback(canceled());
			this.rejected = true;
			resolve();
		});
	}
}

//#endregion


/**
 * A helper to delay (debounce) execution of a task that is being requested often.
 *
 * Following the throttler, now imagine the mail man wants to optimize the number of
 * trips proactively. The trip itself can be long, so he decides not to make the trip
 * as soon as a letter is submitted. Instead he waits a while, in case more
 * letters are submitted. After said waiting period, if no letters were submitted, he
 * decides to make the trip. Imagine that N more letters were submitted after the first
 * one, all within a short period of time between each other. Even though N+1
 * submissions occurred, only 1 delivery was made.
 *
 * The delayer offers this behavior via the trigger() method, into which both the task
 * to be executed and the waiting period (delay) must be passed in as arguments. Following
 * the example:
 *
 * 		const delayer = new Delayer(WAITING_PERIOD);
 * 		const letters = [];
 *
 * 		function letterReceived(l) {
 * 			letters.push(l);
 * 			delayer.trigger(() => { return makeTheTrip(); });
 * 		}
 */
 export class Delayer<T> implements DestroyLike {

	private timeout: any;
	private completionPromise: Promise<any> | null;
	private doResolve: ((value?: any | Promise<any>) => void) | null;
	private doReject: ((err: any) => void) | null;
	private task: ITask<T | Promise<T>> | null;

	constructor(public defaultDelay: number) {
		this.timeout = null;
		this.completionPromise = null;
		this.doResolve = null;
		this.doReject = null;
		this.task = null;
	}

	trigger(task: ITask<T | Promise<T>>, delay: number = this.defaultDelay): Promise<T> {
		this.task = task;
		this.cancelTimeout();

		if (!this.completionPromise) {
			this.completionPromise = new Promise((resolve, reject) => {
				this.doResolve = resolve;
				this.doReject = reject;
			}).then(() => {
				this.completionPromise = null;
				this.doResolve = null;
				if (this.task) {
					const task = this.task;
					this.task = null;
					return task();
				}
				return undefined;
			});
		}

		this.timeout = setTimeout(() => {
			this.timeout = null;
			if (this.doResolve) {
				this.doResolve(null);
			}
		}, delay);

		return this.completionPromise;
	}

	isTriggered(): boolean {
		return this.timeout !== null;
	}

	cancel(): void {
		this.cancelTimeout();

		if (this.completionPromise) {
			if (this.doReject) {
				this.doReject(canceled());
			}
			this.completionPromise = null;
		}
	}

	private cancelTimeout(): void {
		if (this.timeout !== null) {
			clearTimeout(this.timeout);
			this.timeout = null;
		}
	}

	destroy(): void {
		this.cancelTimeout();
	}
}

export interface ITask<T> {
	(): T;
}

/**
 * A helper to prevent accumulation of sequential async tasks.
 *
 * Imagine a mail man with the sole task of delivering letters. As soon as
 * a letter submitted for delivery, he drives to the destination, delivers it
 * and returns to his base. Imagine that during the trip, N more letters were submitted.
 * When the mail man returns, he picks those N letters and delivers them all in a
 * single trip. Even though N+1 submissions occurred, only 2 deliveries were made.
 *
 * The throttler implements this via the queue() method, by providing it a task
 * factory. Following the example:
 *
 * 		const throttler = new Throttler();
 * 		const letters = [];
 *
 * 		function deliver() {
 * 			const lettersToDeliver = letters;
 * 			letters = [];
 * 			return makeTheTrip(lettersToDeliver);
 * 		}
 *
 * 		function onLetterReceived(l) {
 * 			letters.push(l);
 * 			throttler.queue(deliver);
 * 		}
 */
 export class Throttler {

	private activePromise: Promise<any> | null;
	private queuedPromise: Promise<any> | null;
	private queuedPromiseFactory: ITask<Promise<any>> | null;

	constructor() {
		this.activePromise = null;
		this.queuedPromise = null;
		this.queuedPromiseFactory = null;
	}

	queue<T>(promiseFactory: ITask<Promise<T>>): Promise<T> {
		if (this.activePromise) {
			this.queuedPromiseFactory = promiseFactory;

			if (!this.queuedPromise) {
				const onComplete = () => {
					this.queuedPromise = null;

					const result = this.queue(this.queuedPromiseFactory!);
					this.queuedPromiseFactory = null;

					return result;
				};

				this.queuedPromise = new Promise(resolve => {
					this.activePromise!.then(onComplete, onComplete).then(resolve);
				});
			}

			return new Promise((resolve, reject) => {
				this.queuedPromise!.then(resolve, reject);
			});
		}

		this.activePromise = promiseFactory();

		return new Promise((resolve, reject) => {
			this.activePromise!.then((result: T) => {
				this.activePromise = null;
				resolve(result);
			}, (err: unknown) => {
				this.activePromise = null;
				reject(err);
			});
		});
	}
}
/**
 * A helper to delay execution of a task that is being requested often, while
 * preventing accumulation of consecutive executions, while the task runs.
 *
 * The mail man is clever and waits for a certain amount of time, before going
 * out to deliver letters. While the mail man is going out, more letters arrive
 * and can only be delivered once he is back. Once he is back the mail man will
 * do one more trip to deliver the letters that have accumulated while he was out.
 */
 export class ThrottledDelayer<T> {

	private delayer: Delayer<Promise<T>>;
	private throttler: Throttler;

	constructor(defaultDelay: number) {
		this.delayer = new Delayer(defaultDelay);
		this.throttler = new Throttler();
	}

	trigger(promiseFactory: ITask<Promise<T>>, delay?: number): Promise<T> {
		return this.delayer.trigger(() => this.throttler.queue(promiseFactory), delay) as unknown as Promise<T>;
	}

	isTriggered(): boolean {
		return this.delayer.isTriggered();
	}

	cancel(): void {
		this.delayer.cancel();
	}

	destroy(): void {
		this.delayer.destroy();
	}
}