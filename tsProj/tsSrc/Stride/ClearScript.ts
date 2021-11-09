

 interface IClearScriptConfig{
   readonly Services:IServiceRegistry
   readonly graphicsDeviceService:IGraphicsDeviceService
   readonly  Game:IGame
   readonly Content:IContentManager
   readonly Input:InputManager
   readonly  SceneSystem:SceneSystem
   readonly  EffectSystem:EffectSystem
   readonly  Audio:AudioSystem
   readonly  SpriteAnimation:SpriteAnimationSystem
   readonly  GameProfiler:GameProfilingSystem
   readonly   DebugText:DebugTextSystem
   readonly  Streaming:StreamingManager

}

 interface IClearScript extends IClearScriptConfig{
    readonly Entity:Entity
    Initialize(args:IClearScriptConfig):void
    Cancel():void
    Update?:()=>void
}

