using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Engine;
using Stride.Core;
using Stride.Core.Diagnostics;
using Stride.Engine.Design;
using Stride.Engine.Processors;
using Stride.Graphics;
using Stride.Games;
using Stride.Core.Serialization.Contents;
using Stride.Input;
using Stride.Rendering;
using Stride.Audio;
using Stride.Rendering.Sprites;
using Stride.Profiling;
using Stride.Streaming;
using System.ComponentModel;
using Stride.Core.Collections;
using System.Dynamic;
using Stride.Core.MicroThreading;
using System.Threading;
using Stride.ClearScript;
using Stride.Engine.ClearScript;

namespace Stride.Engine
{
    [DataContract("ClearScriptComponent", Inherited = true)]
    [DefaultEntityComponentProcessor(typeof(ClearScriptProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [Display(Expand = ExpandRule.Once)]
    [AllowMultipleComponents]
    [ComponentOrder(1001)]
    [ComponentCategory("ClearScripts")]
    public abstract class ClearScriptComponent : BaseScriptComponent
    {
        
        /// <summary>
        /// The global profiling key for scripts. Activate/deactivate this key to activate/deactivate profiling for all your scripts.
        /// </summary>
        public static readonly ProfilingKey ScriptGlobalProfilingKey = new ProfilingKey("ClearScript");

        private static readonly Dictionary<Type, ProfilingKey> ScriptToProfilingKey = new Dictionary<Type, ProfilingKey>();

        protected dynamic scriptObj;
        protected string scriptCtorName;
        protected ClearScriptComponent()
        {

        }

        internal override void  Initialize(IServiceRegistry registry)
        {
            base.Initialize(registry);
            try
            {
                var Services = registry;
                ScriptVM = Services.GetSafeServiceAs<ClearScriptVM>();
                scriptObj = ScriptVM.createComponentScript(scriptCtorName);

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
            }
            catch (Exception e)
            {

            }

        }


        /// <summary>
        /// Gets the profiling key to activate/deactivate profiling for the current script class.
        /// </summary>
        [DataMemberIgnore]
        public override ProfilingKey ProfilingKey
        {
            get
            {
                if (profilingKey != null)
                    return profilingKey;

                var scriptType = GetType();
                if (!ScriptToProfilingKey.TryGetValue(scriptType, out profilingKey))
                {
                    profilingKey = new ProfilingKey(ScriptGlobalProfilingKey, scriptType.FullName);
                    ScriptToProfilingKey[scriptType] = profilingKey;
                }

                return profilingKey;
            }
        }


        [DataMemberIgnore]
        public IClearScriptVM ScriptVM { get; private set; }

        

        /// <summary>
        /// Called when the script's update loop is canceled.
        /// </summary>
        public override void Cancel()
        {
            scriptObj?.Cancel();
            base.Cancel();
        }


        protected bool ScriptHasProperty(string propertyName)
        {
            return true;
            //if (scriptObj is ExpandoObject)
            //    return ((IDictionary<string, object>)scriptObj).ContainsKey(propertyName);
            //return scriptObj.GetType().GetProperty(propertyName) != null;
        }

    }
}
