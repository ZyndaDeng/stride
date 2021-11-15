declare class AudioSystem extends GameSystemBase {
}
interface IClearScriptConfig {
    readonly Services: IServiceRegistry;
    readonly graphicsDeviceService: IGraphicsDeviceService;
    readonly Game: IGame;
    readonly Content: IContentManager;
    readonly Input: InputManager;
    readonly SceneSystem: SceneSystem;
    readonly EffectSystem: EffectSystem;
    readonly Audio: AudioSystem;
    readonly SpriteAnimation: SpriteAnimationSystem;
    readonly GameProfiler: GameProfilingSystem;
    readonly DebugText: DebugTextSystem;
    readonly Streaming: StreamingManager;
}
interface IClearScript extends IClearScriptConfig {
    readonly Entity: Entity;
    Initialize(args: IClearScriptConfig): void;
    Cancel(): void;
    Update?: () => void;
}
declare class TrackingCollection<T> {
}
declare interface IServiceRegistry {
    AddService<T>(service: T): void;
    GetService<T>(servieCtor: Constructor<T>): T;
    RemoveService<T>(servieCtor: Constructor<T>): T;
    GetSafeServiceAs<T>(servieCtor: Constructor<T>): T;
}
declare class ComponentBase {
    Name: string;
}
declare interface IContentManager {
    Exists(url: string): boolean;
}
declare class Vector3 {
    static readonly Zero: Vector3;
    static readonly UnitX: Vector3;
    static readonly UnitY: Vector3;
    static readonly UnitZ: Vector3;
    static readonly One: Vector3;
    X: float;
    Y: float;
    Z: float;
    constructor(value: float);
    constructor(x: float, y: float, z: float);
}
declare class Quaternion {
    static readonly Zero: Quaternion;
    static readonly One: Quaternion;
    static readonly Identity: Quaternion;
    X: float;
    Y: float;
    Z: float;
    W: float;
    constructor(value: float);
    constructor(value: Vector3, w: float);
    constructor(x: float, y: float, z: float, w: float);
    IsNormalized(): boolean;
    readonly Angle: float;
    readonly Axis: Vector3;
    readonly YawPitchRoll: Vector3;
    Length(): float;
    LengthSquared(): float;
}
declare class Matrix {
    static readonly Zero: Matrix;
    static readonly Identity: Matrix;
}
declare const enum ExecutionMode {
    None = 0,
    Runtime = 1,
    Editor = 2,
    Thumbnail = 4,
    Preview = 8,
    EffectCompile = 16,
    All = 31
}
declare class EntityManager extends ComponentBase {
    readonly ExecutionMode: ExecutionMode;
}
declare class TransformComponent {
    LocalMatrix: Matrix;
    Position: Vector3;
    Rotation: Quaternion;
    Scale: Vector3;
    UseTRS: boolean;
    RotationEulerXYZ: Vector3;
    Parent: TransformComponent;
    UpdateLocalMatrix(): void;
    UpdateLocalFromWorld(): void;
    UpdateWorldMatrix(): void;
}
declare class EntityComponentCollection {
}
declare class Entity extends ComponentBase {
    constructor();
    constructor(name: string);
    constructor(position: Vector3, name?: string);
    Id: Guid;
    Scene: Scene;
    readonly EntityManager: EntityManager;
    readonly Transform: TransformComponent;
    readonly Components: EntityComponentCollection;
    GetOrCreate<T extends EntityComponent>(ctor: Constructor<T>): T;
    Add(component: EntityComponent): void;
    Get<T extends EntityComponent>(ctor: Constructor<T>): T;
    Get<T extends EntityComponent>(index: int): T;
    Remove<T extends EntityComponent>(ctor: Constructor<T>): void;
    Remove(component: EntityComponent): void;
}
declare class SceneSystem extends GameSystemBase {
}
declare class Scene extends ComponentBase {
    constructor();
    Id: Guid;
    Parent: Scene;
    readonly Entities: TrackingCollection<Entity>;
    readonly Children: TrackingCollection<Scene>;
    Offset: Vector3;
    WorldMatrix: Matrix;
    UpdateWorldMatrix(): void;
    ToString(): string;
}
declare abstract class EntityComponent {
    readonly Entity: Entity;
    Id: Guid;
}
declare type PropertyType = any;
interface PropertyMetaData {
    name: string | symbol;
    type: PropertyType;
}
interface ScriptMetaData {
    ctor: Function;
    propertys?: Array<PropertyMetaData>;
}
declare class Environment {
    Components: {
        [name: string]: ScriptMetaData;
    };
    constructor();
}
declare const __ScriptMap: Map<Function, ScriptMetaData>;
declare const __environment: Environment;
interface GameTime {
    readonly Elapsed: TimeSpan;
    readonly Total: TimeSpan;
    readonly FrameCount: int;
    readonly FramePerSecond: float;
    readonly TimePerFrame: TimeSpan;
    readonly FramePerSecondUpdated: boolean;
    readonly WarpElapsed: TimeSpan;
    Factor: double;
}
interface IGame {
    readonly UpdateTime: GameTime;
    readonly DrawTime: GameTime;
    readonly DrawInterpolationFactor: float;
    readonly Content: IContentManager;
}
declare class GameSystemBase extends ComponentBase {
}
declare interface IGraphicsDeviceService {
    GraphicsDevice: GraphicsDevice;
}
declare interface GraphicsDevice extends ComponentBase {
}
declare class InputManager extends ComponentBase {
}
declare class GameProfilingSystem extends GameSystemBase {
}
declare class DebugTextSystem extends GameSystemBase {
}
declare class EffectSystem extends GameSystemBase {
}
declare class SpriteAnimationSystem extends GameSystemBase {
}
declare class StreamingManager extends GameSystemBase {
}
declare type Constructor<T> = new (...args: any[]) => T;
declare type int = number;
declare type float = number;
declare type double = number;
declare class TimeSpan {
}
declare class Guid {
}
interface IMSConsole {
    WriteLine(str: string, ...args: any[]): void;
}
declare let MSConsole: IMSConsole;
declare abstract class ClearScript implements IClearScript {
    Entity: Entity;
    Initialize(args: IClearScriptConfig): void;
    abstract Start(): void;
    abstract Cancel(): void;
    Update?: (() => void) | undefined;
    Services: IServiceRegistry;
    graphicsDeviceService: IGraphicsDeviceService;
    Game: IGame;
    Content: IContentManager;
    Input: InputManager;
    SceneSystem: SceneSystem;
    EffectSystem: EffectSystem;
    Audio: AudioSystem;
    SpriteAnimation: SpriteAnimationSystem;
    GameProfiler: GameProfilingSystem;
    DebugText: DebugTextSystem;
    Streaming: StreamingManager;
}
/**将脚本注册到引擎 */
declare function RegisterComponent(name: string): ClassDecorator;
declare function getMetaData(array: Array<PropertyMetaData>, keyName: string | symbol): PropertyMetaData;
declare function DisplayProperty(type: PropertyType): PropertyDecorator;
