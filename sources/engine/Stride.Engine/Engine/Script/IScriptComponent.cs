using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Core.Collections;
using Stride.Core.Diagnostics;
using Stride.Core.MicroThreading;

namespace Stride.Engine
{
    public interface IScriptComponent
    {
        int Priority { get;  }
        ProfilingKey ProfilingKey { get;  }
        bool IsLiveReloading { get; internal set; }
       internal  void Initialize(IServiceRegistry registry);
        void Cancel();
    }

    public interface IStartupScript: IScriptComponent
    {
        internal PriorityQueueNode<SchedulerEntry> StartSchedulerNode { get; set; }
        void Start();
    }

    public interface ISyncScript:IStartupScript
    {
        internal PriorityQueueNode<SchedulerEntry> UpdateSchedulerNode { get; set; }
        bool ShouldUpdate { get; }
        void Update();
    }

    public interface IASyncScript
    {

    }
}
