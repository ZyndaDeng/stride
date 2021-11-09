

 declare interface IServiceRegistry{

    AddService<T>( service:T):void;
    GetService<T>(servieCtor:Constructor<T>):T
    RemoveService<T>(servieCtor:Constructor<T>):T

    GetSafeServiceAs<T>(servieCtor:Constructor<T>):T
}

 declare class ComponentBase{
    Name:string

}

 declare interface IContentManager{
    Exists(url:string):boolean
}

declare class Vector3{
    static readonly  Zero:Vector3
    static readonly  UnitX:Vector3
    static readonly  UnitY:Vector3
    static readonly  UnitZ:Vector3
    static readonly  One:Vector3

    X:float
    Y:float
    Z:float

    constructor(value:float);
    constructor(x:float,y:float,z:float);
}

declare class Quaternion{
    static readonly  Zero:Quaternion
    static readonly  One:Quaternion
    static readonly  Identity:Quaternion

    X:float
    Y:float
    Z:float
    W:float

    constructor(value:float)
    //constructor(value:Vector4)
    constructor(value:Vector3,w:float)
    constructor(x:float,y:float,z:float,w:float)

    IsNormalized():boolean
    readonly Angle:float
    readonly Axis:Vector3
    readonly YawPitchRoll:Vector3
    Length():float
    LengthSquared():float
}

declare class Matrix{
    static readonly  Zero:Matrix
    static readonly  Identity:Matrix
}