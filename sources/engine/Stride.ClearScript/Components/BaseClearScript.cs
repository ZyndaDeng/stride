using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Audio;
using Stride.Core;
using Stride.Core.Serialization.Contents;
using Stride.Engine;
using Stride.Engine.Processors;
using Stride.Games;
using Stride.Graphics;
using Stride.Input;
using Stride.Profiling;
using Stride.Rendering;
using Stride.Rendering.Sprites;
using Stride.Streaming;

namespace Stride.ClearScript.Components
{
    public class BaseClearScript : SyncScript
    {
        private dynamic scriptObj;

        [DataMemberIgnore]
        public IClearScriptSystem ScriptSystem { get; private set; }
        public override void Start()
        {
            base.Start();

            ScriptSystem = Services.GetSafeServiceAs<IClearScriptSystem>();
            scriptObj = ScriptSystem.Evaluate("new TestScript()");
            dynamic args = new
            {
                Services = Services,
                graphicsDeviceService = Services.GetSafeServiceAs<IGraphicsDeviceService>(),
                Game = Services.GetSafeServiceAs<IGame>(),
                Content = (ContentManager)Services.GetSafeServiceAs<IContentManager>(),
                Input = Services.GetSafeServiceAs<InputManager>(),
                Script = Services.GetSafeServiceAs<ScriptSystem>(),
                SceneSystem = Services.GetSafeServiceAs<SceneSystem>(),
                EffectSystem = Services.GetSafeServiceAs<EffectSystem>(),
                Audio = Services.GetSafeServiceAs<AudioSystem>(),
                SpriteAnimation = Services.GetSafeServiceAs<SpriteAnimationSystem>(),
                GameProfiler = Services.GetSafeServiceAs<GameProfilingSystem>(),
                DebugText = Services.GetSafeServiceAs<DebugTextSystem>(),
                Streaming = Services.GetSafeServiceAs<StreamingManager>(),
            };
            scriptObj.Entity = Entity;
            scriptObj.Initialize(args);
            scriptObj.Start();
        }
        public override void Update()
        {
            scriptObj?.Update();
        }

       
    }
}
