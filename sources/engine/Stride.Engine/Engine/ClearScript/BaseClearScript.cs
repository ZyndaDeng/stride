
using Stride.Core.Collections;
using Stride.Core.MicroThreading;


namespace Stride.Engine
{
    public class BaseClearScript : ClearScriptComponent,IStartupScript,ISyncScript
    {
        public bool ShouldUpdate => scriptObj != null && ScriptHasProperty("Update");

        PriorityQueueNode<SchedulerEntry> IStartupScript.StartSchedulerNode { get; set; }
        PriorityQueueNode<SchedulerEntry> ISyncScript.UpdateSchedulerNode { get; set; }

        public void Start()
        {
            scriptObj?.Start();
        }

        /// <summary>
        /// Called every frame.
        /// </summary>
        public void Update()
        {
            scriptObj?.Update();
        }

       
    }
}
