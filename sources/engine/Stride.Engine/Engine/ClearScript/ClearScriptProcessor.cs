using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Engine.Processors;

namespace Stride.Engine.ClearScript
{
    /// <summary>
    /// Manage scripts
    /// </summary>
    public sealed class ClearScriptProcessor : EntityProcessor<ClearScriptComponent>
    {
        private ScriptSystem scriptSystem;

        public ClearScriptProcessor()
        {
            // Script processor always running before others
            Order = -100000;
        }

        protected internal override void OnSystemAdd()
        {
            scriptSystem = Services.GetService<ScriptSystem>();
        }

        /// <inheritdoc/>
        protected override void OnEntityComponentAdding(Entity entity, ClearScriptComponent component, ClearScriptComponent associatedData)
        {
            // Add current list of scripts
            scriptSystem.Add(component);
        }

        /// <inheritdoc/>
        protected override void OnEntityComponentRemoved(Entity entity, ClearScriptComponent component, ClearScriptComponent associatedData)
        {
            scriptSystem.Remove(component);
        }
    }
}
