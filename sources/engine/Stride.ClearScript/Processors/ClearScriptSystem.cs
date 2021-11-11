//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Stride.Core;
//using Stride.Core.Diagnostics;
//using Stride.Core.MicroThreading;
//using Stride.Games;

//namespace Stride.Engine.Processors
//{
//    public sealed class ClearScriptSystem : GameSystemBase
//    {
//        private const long UpdateBit = 1L << 32;

//        internal static readonly Logger Log = GlobalLogger.GetLogger("ScriptSystem");

//        /// <summary>
//        /// Contains all currently executed scripts
//        /// </summary>
//        private readonly HashSet<ClearScriptComponent> registeredScripts = new HashSet<ClearScriptComponent>();
//        private readonly HashSet<ClearScriptComponent> scriptsToStart = new HashSet<ClearScriptComponent>();
//        private readonly HashSet<ClearScriptComponent> syncScripts = new HashSet<ClearScriptComponent>();
//        private readonly List<ClearScriptComponent> scriptsToStartCopy = new List<ClearScriptComponent>();
//        private readonly List<ClearScriptComponent> syncScriptsCopy = new List<ClearScriptComponent>();

//        /// <summary>
//        /// Gets the scheduler.
//        /// </summary>
//        /// <value>The scheduler.</value>
//        public Scheduler Scheduler { get; private set; }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="GameSystemBase" /> class.
//        /// </summary>
//        /// <param name="registry">The registry.</param>
//        /// <remarks>The GameSystem is expecting the following services to be registered: <see cref="IGame" /> and <see cref="ContentManager" />.</remarks>
//        public ClearScriptSystem(IServiceRegistry registry)
//            : base(registry)
//        {
//            Enabled = true;
//            Scheduler = new Scheduler();
//            Scheduler.ActionException += Scheduler_ActionException;
//        }

//        protected override void Destroy()
//        {
//            Scheduler.ActionException -= Scheduler_ActionException;
//            Scheduler = null;

//            Services.RemoveService<ClearScriptSystem>();

//            base.Destroy();
//        }

//        public override void Update(GameTime gameTime)
//        {
//            // Copy scripts to process (so that scripts added during this frame don't affect us)
//            // TODO: How to handle scripts that we want to start during current frame?
//            scriptsToStartCopy.AddRange(scriptsToStart);
//            scriptsToStart.Clear();
//            syncScriptsCopy.AddRange(syncScripts);

//            // Schedule new scripts: StartupScript.Start() and AsyncScript.Execute()
//            foreach (var script in scriptsToStartCopy)
//            {
//                // Start the script
//                var startupScript = script;
//                if (startupScript != null)
//                {
//                    startupScript.StartSchedulerNode = Scheduler.Add(startupScript.Start, startupScript.Priority, startupScript, startupScript.ProfilingKey);
//                }
//                else
//                {
//                    // Start a microthread with execute method if it's an async script
//                    var asyncScript = script as AsyncScript;
//                    if (asyncScript != null)
//                    {
//                        asyncScript.MicroThread = AddTask(asyncScript.Execute, asyncScript.Priority | UpdateBit);
//                        asyncScript.MicroThread.ProfilingKey = asyncScript.ProfilingKey;
//                    }
//                }
//            }

//            // Schedule existing scripts: SyncScript.Update()
//            foreach (var syncScript in syncScriptsCopy)
//            {
//                // Update priority
//                var updateSchedulerNode = syncScript.UpdateSchedulerNode;
//                updateSchedulerNode.Value.Priority = syncScript.Priority | UpdateBit;

//                // Schedule
//                Scheduler.Schedule(updateSchedulerNode, ScheduleMode.Last);
//            }

//            // Run current micro threads
//            Scheduler.Run();

//            // Flag scripts as not being live reloaded after starting/executing them for the first time
//            foreach (var script in scriptsToStartCopy)
//            {
//                // Remove the start node after it got executed
//                var startupScript = script;//as StartupScript;
//                if (startupScript != null)
//                {
//                    startupScript.StartSchedulerNode = null;
//                }

//                if (script.IsLiveReloading)
//                    script.IsLiveReloading = false;
//            }

//            scriptsToStartCopy.Clear();
//            syncScriptsCopy.Clear();
//        }

//        /// <summary>
//        /// Allows to wait for next frame.
//        /// </summary>
//        /// <returns>ChannelMicroThreadAwaiter&lt;System.Int32&gt;.</returns>
//        public ChannelMicroThreadAwaiter<int> NextFrame()
//        {
//            return Scheduler.NextFrame();
//        }

//        /// <summary>
//        /// Adds the specified micro thread function.
//        /// </summary>
//        /// <param name="microThreadFunction">The micro thread function.</param>
//        /// <returns>MicroThread.</returns>
//        public MicroThread AddTask(Func<Task> microThreadFunction, long priority = 0)
//        {
//            var microThread = Scheduler.Create();
//            microThread.Priority = priority;
//            microThread.Start(microThreadFunction);
//            return microThread;
//        }

//        /// <summary>
//        /// Waits all micro thread finished their task completion.
//        /// </summary>
//        /// <param name="microThreads">The micro threads.</param>
//        /// <returns>Task.</returns>
//        public async Task WhenAll(params MicroThread[] microThreads)
//        {
//            await Scheduler.WhenAll(microThreads);
//        }

//        /// <summary>
//        /// Add the provided script to the script system.
//        /// </summary>
//        /// <param name="script">The script to add</param>
//        public void Add(ClearScriptComponent script)
//        {
//            script.Initialize(Services);
//            registeredScripts.Add(script);

//            // Register script for Start() and possibly async Execute()
//            scriptsToStart.Add(script);

//            // If it's a synchronous script, add it to the list as well
//            // var syncScript = script;//as SyncScript;
//            if (script.hasProperty("Update"))
//            {
//                script.UpdateSchedulerNode = Scheduler.Create(script.Update, script.Priority | UpdateBit);
//                script.UpdateSchedulerNode.Value.Token = script;
//                script.UpdateSchedulerNode.Value.ProfilingKey = script.ProfilingKey;
//                syncScripts.Add(script);
//            }
//        }

//        /// <summary>
//        /// Remove the provided script from the script system.
//        /// </summary>
//        /// <param name="script">The script to remove</param>
//        public void Remove(ClearScriptComponent script)
//        {
//            // Make sure it's not registered in any pending list
//            var startWasPending = scriptsToStart.Remove(script);
//            var wasRegistered = registeredScripts.Remove(script);

//            if (!startWasPending && wasRegistered)
//            {
//                // Cancel scripts that were already started
//                try
//                {
//                    script.Cancel();
//                }
//                catch (Exception e)
//                {
//                    HandleSynchronousException(script, e);
//                }

//                var asyncScript = script;// as AsyncScript;
//                asyncScript.MicroThread?.Cancel();
//            }

//            // Remove script from the scheduler, in case it was removed during scheduler execution
//            var startupScript = script;//as StartupScript;
//            if (startupScript != null)
//            {
//                if (startupScript.StartSchedulerNode != null)
//                {
//                    Scheduler?.Unschedule(startupScript.StartSchedulerNode);
//                    startupScript.StartSchedulerNode = null;
//                }

//                //var syncScript = script as SyncScript;
//                if (script.hasProperty("Update"))
//                {
//                    syncScripts.Remove(script);
//                    Scheduler?.Unschedule(script.UpdateSchedulerNode);
//                    script.UpdateSchedulerNode = null;
//                }
//            }
//        }

//        /// <summary>
//        /// Called by a live scripting debugger to notify the ScriptSystem about reloaded scripts.
//        /// </summary>
//        /// <param name="oldScript">The old script</param>
//        /// <param name="newScript">The new script</param>
//        public void LiveReload(ClearScriptComponent oldScript, ClearScriptComponent newScript)
//        {
//            // Set live reloading mode for the rest of it's lifetime
//            oldScript.IsLiveReloading = true;

//            // Set live reloading mode until after being started
//            newScript.IsLiveReloading = true;
//        }

//        private void Scheduler_ActionException(Scheduler scheduler, SchedulerEntry schedulerEntry, Exception e)
//        {
//            HandleSynchronousException((ClearScriptComponent)schedulerEntry.Token, e);
//        }

//        private void HandleSynchronousException(ClearScriptComponent script, Exception e)
//        {
//            Log.Error("Unexpected exception while executing a script.", e);

//            // Only crash if live scripting debugger is not listening
//            if (Scheduler.PropagateExceptions)
//                ExceptionDispatchInfo.Capture(e).Throw();

//            // Remove script from all lists
//            //var syncScript = script as SyncScript;
//            if (script.hasProperty("Update"))
//            {
//                syncScripts.Remove(script);
//            }

//            registeredScripts.Remove(script);
//        }

//        private class PriorityScriptComparer : IComparer<ClearScriptComponent>
//        {
//            public static readonly PriorityScriptComparer Default = new PriorityScriptComparer();

//            public int Compare(ClearScriptComponent x, ClearScriptComponent y)
//            {
//                return x.Priority.CompareTo(y.Priority);
//            }
//        }
//    }
//}
