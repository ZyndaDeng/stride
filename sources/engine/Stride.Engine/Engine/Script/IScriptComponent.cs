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

     interface IStartupScript
    {
         PriorityQueueNode<SchedulerEntry> StartSchedulerNode { get; set; }
        void Start();
    }

     interface ISyncScript:IStartupScript
    {
         PriorityQueueNode<SchedulerEntry> UpdateSchedulerNode { get; set; }
        bool ShouldUpdate { get; }
        void Update();
    }

    public interface IASyncScript
    {

    }
}
