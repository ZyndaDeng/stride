//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Stride.Engine;
//using Stride.Core;
//using Stride.Core.Diagnostics;
//using Stride.Engine.Design;
//using Stride.Engine.Processors;
//using Stride.Graphics;
//using Stride.Games;
//using Stride.Core.Serialization.Contents;
//using Stride.Input;
//using Stride.Rendering;
//using Stride.Audio;
//using Stride.Rendering.Sprites;
//using Stride.Profiling;
//using Stride.Streaming;
//using System.ComponentModel;
//using Stride.Core.Collections;
//using System.Dynamic;
//using Stride.Core.MicroThreading;
//using System.Threading;

//namespace Stride.Engine
//{
//    [DataContract("ClearScriptComponent", Inherited = true)]
//    [DefaultEntityComponentProcessor(typeof(ScriptProcessor), ExecutionMode = ExecutionMode.Runtime)]
//    [Display(Expand = ExpandRule.Once)]
//    [AllowMultipleComponents]
//    [ComponentOrder(1000)]
//    [ComponentCategory("ClearScripts")]
//    public class ClearScriptComponent : EntityComponent, ICollectorHolder
//    {
//        public const uint LiveScriptingMask = 128;

//        /// <summary>
//        /// The global profiling key for scripts. Activate/deactivate this key to activate/deactivate profiling for all your scripts.
//        /// </summary>
//        public static readonly ProfilingKey ScriptGlobalProfilingKey = new ProfilingKey("Script");

//        private static readonly Dictionary<Type, ProfilingKey> ScriptToProfilingKey = new Dictionary<Type, ProfilingKey>();

//        private ProfilingKey profilingKey;

//        //private IGraphicsDeviceService graphicsDeviceService;
//        private Logger logger;

//        private dynamic scriptObj;

//        protected ClearScriptComponent()
//        {

//        }

//        internal void Initialize(IServiceRegistry registry)
//        {
//            try
//            {
//                var Services = registry;
//                ScriptSystem = Services.GetSafeServiceAs<IClearScriptSystem>();
//                scriptObj = ScriptSystem.Evaluate("new TestScript()");

//                dynamic args = new
//                {
//                    Services = Services,
//                    graphicsDeviceService = Services.GetSafeServiceAs<IGraphicsDeviceService>(),
//                    Game = Services.GetSafeServiceAs<IGame>(),
//                    Content = (ContentManager)Services.GetSafeServiceAs<IContentManager>(),
//                    Input = Services.GetSafeServiceAs<InputManager>(),
//                    Script = Services.GetSafeServiceAs<ScriptSystem>(),
//                    SceneSystem = Services.GetSafeServiceAs<SceneSystem>(),
//                    EffectSystem = Services.GetSafeServiceAs<EffectSystem>(),
//                    Audio = Services.GetSafeServiceAs<AudioSystem>(),
//                    SpriteAnimation = Services.GetSafeServiceAs<SpriteAnimationSystem>(),
//                    GameProfiler = Services.GetSafeServiceAs<GameProfilingSystem>(),
//                    DebugText = Services.GetSafeServiceAs<DebugTextSystem>(),
//                    Streaming = Services.GetSafeServiceAs<StreamingManager>(),
//                };
//                scriptObj.Initialize(args);
//            }
//            catch (Exception e)
//            {

//            }

//        }

//        protected Entity entity;
//        public Entity Entity
//        {
//            get => entity; internal set
//            {
//                entity = value;
//                scriptObj.Entity = value;
//            }
//        }

//        /// <summary>
//        /// Gets the profiling key to activate/deactivate profiling for the current script class.
//        /// </summary>
//        [DataMemberIgnore]
//        public ProfilingKey ProfilingKey
//        {
//            get
//            {
//                if (profilingKey != null)
//                    return profilingKey;

//                var scriptType = GetType();
//                if (!ScriptToProfilingKey.TryGetValue(scriptType, out profilingKey))
//                {
//                    profilingKey = new ProfilingKey(ScriptGlobalProfilingKey, scriptType.FullName);
//                    ScriptToProfilingKey[scriptType] = profilingKey;
//                }

//                return profilingKey;
//            }
//        }



//        /// <summary>
//        /// Gets the streaming system.
//        /// </summary>
//        /// <value>The streaming system.</value>
//        [DataMemberIgnore]
//        public StreamingManager Streaming { get; private set; }

//        [DataMemberIgnore]
//        public IClearScriptSystem ScriptSystem { get; private set; }

//        [DataMemberIgnore]
//        protected Logger Log
//        {
//            get
//            {
//                if (logger != null)
//                {
//                    return logger;
//                }

//                var className = GetType().FullName;
//                logger = GlobalLogger.GetLogger(className);
//                return logger;
//            }
//        }

//        private int priority;

//        /// <summary>
//        /// The priority this script will be scheduled with (compared to other scripts).
//        /// </summary>
//        /// <userdoc>The execution priority for this script. It applies to async, sync and startup scripts. Lower values mean earlier execution.</userdoc>
//        [DefaultValue(0)]
//        [DataMember(10000)]
//        public int Priority
//        {
//            get { return priority; }
//            set { priority = value; PriorityUpdated(); }
//        }

//        /// <summary>
//        /// Determines whether the script is currently undergoing live reloading.
//        /// </summary>
//        public bool IsLiveReloading { get; internal set; }

//        /// <summary>
//        /// The object collector associated with this script.
//        /// </summary>
//        [DataMemberIgnore]
//        public ObjectCollector Collector
//        {
//            get
//            {
//                collector.EnsureValid();
//                return collector;
//            }
//        }

//        private ObjectCollector collector;



//        /// <summary>
//        /// Called when the script's update loop is canceled.
//        /// </summary>
//        public void Cancel()
//        {
//            scriptObj.Cancel();
//            collector.Dispose();
//        }

//        public void Start()
//        {
//            scriptObj.Start();
//        }

//        internal PriorityQueueNode<SchedulerEntry> UpdateSchedulerNode;

//        public bool hasProperty(string propertyName)
//        {
//            if (scriptObj is ExpandoObject)
//                return ((IDictionary<string, object>)scriptObj).ContainsKey(propertyName);
//            return scriptObj.GetType().GetProperty(propertyName) != null;
//        }
//        /// <summary>
//        /// Called every frame.
//        /// </summary>
//        public void Update()
//        {
//            scriptObj.Update();
//        }

//        [DataMemberIgnore]
//        internal MicroThread MicroThread;

//        [DataMemberIgnore]
//        internal CancellationTokenSource CancellationTokenSource;

//        /// <summary>
//        /// Gets a token indicating if the script execution was canceled.
//        /// </summary>
//        public CancellationToken CancellationToken => MicroThread.CancellationToken;

//        /// <summary>
//        /// Called once, as a microthread
//        /// </summary>
//        /// <returns></returns>
//        //public Task Execute()
//        //{

//        //}

//        protected internal void PriorityUpdated()
//        {
//            scriptObj.PriorityUpdated();

//            // Update micro thread priority
//            if (MicroThread != null)
//                MicroThread.Priority = Priority;
//        }
//    }
//}
