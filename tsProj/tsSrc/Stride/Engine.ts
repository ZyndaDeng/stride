

declare class EntityManager extends ComponentBase{
   readonly ExecutionMode:ExecutionMode
}

declare class TransformComponent{
    LocalMatrix:Matrix
    Position:Vector3
    Rotation:Quaternion
    Scale:Vector3

    UseTRS:boolean
    RotationEulerXYZ:Vector3
    Parent:TransformComponent
    UpdateLocalMatrix():void
    UpdateLocalFromWorld():void
    UpdateWorldMatrix():void
}

declare class EntityComponentCollection{

}

declare class Entity extends ComponentBase{

    constructor()
    constructor(name:string)
    constructor(position:Vector3,name?:string)
    Id:Guid
    Scene:Scene
    readonly EntityManager:EntityManager
    readonly Transform:TransformComponent
    readonly Components:EntityComponentCollection
    GetOrCreate<T extends EntityComponent>(ctor:Constructor<T>):T
    Add(component:EntityComponent):void
    Get<T extends EntityComponent>(ctor:Constructor<T>):T
    Get<T extends EntityComponent>(index:int):T
    Remove<T extends EntityComponent>(ctor:Constructor<T>):void
    Remove(component:EntityComponent):void
}

declare class SceneSystem extends GameSystemBase{

}

declare class Scene extends ComponentBase{
    constructor();
    Id:Guid
    Parent:Scene
   readonly Entities:TrackingCollection<Entity>
   readonly Children:TrackingCollection<Scene>
   Offset:Vector3
   WorldMatrix:Matrix

   UpdateWorldMatrix():void;
   ToString():string
}

declare abstract class EntityComponent{
    readonly Entity:Entity
    Id:Guid
}