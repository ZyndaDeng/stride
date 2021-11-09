

 interface GameTime{

   readonly Elapsed:TimeSpan
   readonly Total:TimeSpan
   readonly FrameCount:int
   readonly FramePerSecond:float
   readonly TimePerFrame:TimeSpan
   readonly FramePerSecondUpdated:boolean
   readonly WarpElapsed:TimeSpan
   Factor:double
}

 interface IGame{
   readonly UpdateTime:GameTime
   readonly DrawTime:GameTime
   readonly DrawInterpolationFactor:float
   readonly Content:IContentManager
}

declare class GameSystemBase extends ComponentBase{
   
}