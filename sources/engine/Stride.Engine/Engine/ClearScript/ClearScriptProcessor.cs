
using Stride.ClearScript;
using Stride.Core;
using Stride.Engine.Processors;
using Stride.Games;

namespace Stride.Engine.ClearScript
{
    /// <summary>
    /// Manage scripts
    /// </summary>
    public sealed class ClearScriptProcessor : EntityProcessor<ClearScriptComponent>
    {
        private ScriptSystem scriptSystem;
        private ClearScriptVM clearScriptVM;

        public ClearScriptProcessor()
        {
            // Script processor always running before others
            Order = -100000;
        }

        protected internal override void OnSystemAdd()
        {
            scriptSystem = Services.GetService<ScriptSystem>();
            clearScriptVM = Services.GetService<ClearScriptVM>();
            if (clearScriptVM == null)
            {
                clearScriptVM = new ClearScriptVM(Services);
                var gameSystems = Services.GetSafeServiceAs<IGameSystemCollection>();
                Services.AddService(clearScriptVM);
                gameSystems.Add(clearScriptVM);
                //clearScriptVM.loadFile("/roaming/src/bundle.js").ContinueWith((t) =>
                //{
                //    clearScriptVM.loadSrc("/roaming/src/components/");
                //});
                
                 
            }
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
