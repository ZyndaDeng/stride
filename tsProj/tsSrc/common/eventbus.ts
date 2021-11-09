import { DestroyLike } from "./Destroy";




export class Eventbus implements DestroyLike{
    private listeners: { [key: string]: Array<{fn:Function,priority:number}> } = {};
    private listenersOncer: { [key: string]: Array<Function> } = {};

    /**
     * 一般用于构造函数中调用, 否则会造成同一个对象会收到多次同一个事件的回调.
     * 由于js没有析构函数, 所以要注意移除的时机否则会造成内存泄漏
     * @param {string} evt
     * @param listener
     * @returns {DestroyLike} 用于取消监听的对象
     */
    on(evt: string, listener: Function,priority=0): DestroyLike{
       return this.baseOn(evt,listener,priority);
    };

    hasListener(evt:string){
        return this.listeners[evt]!=null||this.listenersOncer[evt]!=null;
    }

    protected sortListener(a:{fn:Function,priority:number},b:{fn:Function,priority:number}){
        return b.priority-a.priority;
    }

    protected baseOn(evt: string, listener: Function,priority:number): DestroyLike{
        let a = this.listeners[evt];
        if (a == null) {
            a = [];
            this.listeners[evt] = a;
        }
        a.push({fn:listener,priority:priority});
        a.sort(this.sortListener);
        return {
            destroy: () => this.off(evt, listener)
        };
    }

    once (evt: string, listener: Function): void {
        return this.baseOnce(evt,listener);
    };

    protected baseOnce(evt: string, listener: Function): void{
        let a = this.listenersOncer[evt];
        if (a == null) {
            a = [];
            this.listenersOncer[evt] = a;
        }
        a.push(listener);
    }

    off(evt: string, listener: Function) {
        let a = this.listeners[evt];
        if (a == null) return;
        
        let callbackIndex = -1;
        for(let i=0;i<a.length;i++){
            if(a[i].fn==listener){
                callbackIndex=i;
                break;
            }
        }
        if (callbackIndex > -1) a.splice(callbackIndex, 1);
    };

    emit(evt: string, ...events: any[]){
       this.baseEmit(evt,...events);
    };

    protected baseEmit(evt: string, ...events: any[]){
        let a = this.listeners[evt];
        if (a != null) {
            /** Update any general listeners */
            let b = a.slice(0); // avoid modify during loop
            b.forEach((listener) => listener.fn(...events));
        }

        let a2 = this.listenersOncer[evt];
        if (a2 != null) {
            /** Clear the `once` queue */
            a2.forEach((listener) => listener(...events));
            this.listenersOncer[evt] = [];
        }
    }   

    destroy(){
        this.listeners={};
        this.listenersOncer={};
    }
}

export class EventbusDefine<V extends {[k:string]:any}> extends Eventbus{
    emit<K extends keyof V>(evt:K,event:V[K]){
        return super.baseEmit(evt as string,event);
    }

    on<K extends keyof V>(evt: K, listener: (event:V[K])=>void){
        return super.baseOn(evt as string,listener,0);
    }

    once<K extends keyof V>(evt: K, listener: (event:V[K])=>void){
        return super.baseOnce(evt as string,listener);
    }

    static readonly  None:EventbusDefine<any>=new EventbusDefine();
}