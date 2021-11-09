import { Iterable } from './Iterable';



export const enum StartupKind {
	NewWindow = 1,
	ReloadedWindow = 3,
	ReopenedWindow = 4,
}

export interface DestroyLike{
    destroy():void;
}

export function destroy<T extends DestroyLike>(destroyLike: T): T;
export function destroy<T extends DestroyLike>(destroyLike: T | undefined): T | undefined;
export function destroy<T extends DestroyLike, A extends IterableIterator<T> = IterableIterator<T>>(destroyLikes: IterableIterator<T>): A;
export function destroy<T extends DestroyLike>(destroyLikes: Array<T>): Array<T>;
export function destroy<T extends DestroyLike>(destroyLikes: ReadonlyArray<T>): ReadonlyArray<T>;
export function destroy<T extends DestroyLike>(arg: T | IterableIterator<T> | undefined): any {
	if (Iterable.is(arg)) {
		let errors: any[] = [];

		for (const d of arg) {
			if (d) {
				//markTracked(d);
				try {
					d.destroy();
				} catch (e) {
					errors.push(e);
				}
			}
		}

		if (errors.length === 1) {
			throw errors[0];
		} else if (errors.length > 1) {
			throw new MultiDisposeError(errors);
		}

		return Array.isArray(arg) ? [] : arg;
	} else if (arg) {
		//markTracked(arg);
		arg.destroy();
		return arg;
	}
}

export class MultiDisposeError extends Error {
	constructor(
		public readonly errors: any[]
	) {
		super(`Encountered errors while disposing of store. Errors: [${errors.join(', ')}]`);
	}
}

export class Destroyable implements DestroyLike{
	static readonly None = Object.freeze<DestroyLike>({ destroy() { } });

    protected destroys:DestroyStore

    constructor(){
        this.destroys=new DestroyStore();
    }

	protected _register<T extends DestroyLike>(t: T): T {
		if ((t as unknown as DestroyLike) === this) {
			throw new Error('Cannot register a disposable on itself!');
		}
		return this.destroys.add(t);
	}

    destroy(){
        this.destroys.destroy();
    }
}

export function toDestroy(fn: () => void): DestroyLike {
	const self = {
		destroy: () => {
			fn();
		}
	};
	return self;
}

export class DestroyStore implements DestroyLike {

	static DISABLE_DISPOSED_WARNING = false;

	private _toDispose = new Set<DestroyLike>();
	private _isDisposed = false;

	/**
	 * Dispose of all registered disposables and mark this object as disposed.
	 *
	 * Any future disposables added to this object will be disposed of on `add`.
	 */
	public destroy(): void {
		if (this._isDisposed) {
			return;
		}

		//markTracked(this);
		this._isDisposed = true;
		this.clear();
	}

	/**
	 * Dispose of all registered disposables but do not mark this object as disposed.
	 */
	public clear(): void {
		try {
			destroy(this._toDispose.values());
		} finally {
			this._toDispose.clear();
		}
	}

	public add<T extends DestroyLike>(t: T): T {
		if (!t) {
			return t;
		}
		if ((t as unknown as DestroyStore) === this) {
			throw new Error('Cannot register a disposable on itself!');
		}

		//markTracked(t);
		if (this._isDisposed) {
			if (!DestroyStore.DISABLE_DISPOSED_WARNING) {
				console.warn(new Error('Trying to add a disposable to a DisposableStore that has already been disposed of. The added object will be leaked!').stack);
			}
		} else {
			this._toDispose.add(t);
		}

		return t;
	}
}

