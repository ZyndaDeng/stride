
export class  ClearScript implements IClearScript{
    constructor() {
        
    }
    Entity!: Entity;
    
    Initialize(args: IClearScriptConfig): void {
        this.Services=args.Services;
        this.graphicsDeviceService=args.graphicsDeviceService;
        this.Game=args.Game;
        this.Content=args.Content;
        this.Input=args.Input;
        this.SceneSystem=args.SceneSystem;
        this. EffectSystem=args.EffectSystem;
        this.Audio=args.Audio;
        this.SpriteAnimation=args.SpriteAnimation;
        this. GameProfiler=args.GameProfiler;
        this.DebugText=args.DebugText;
        this.Streaming=args.Streaming;
    }
    Cancel(): void {
        
    }


    Services!: IServiceRegistry;
    graphicsDeviceService!: IGraphicsDeviceService;
    Game!: IGame;
    Content!: IContentManager;
    Input!: InputManager;
    SceneSystem!: SceneSystem;
    EffectSystem!: EffectSystem;
    Audio!: AudioSystem;
    SpriteAnimation!: SpriteAnimationSystem;
    GameProfiler!: GameProfilingSystem;
    DebugText!: DebugTextSystem;
    Streaming!: StreamingManager;
}