
using Xunit;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Graphics.Regression;
using Stride.ClearScript;
using Stride.Core.Serialization.Contents;
using Stride.Core.IO;
using System.Threading.Tasks;

namespace Stride.Engine.Tests
{

    public class ClearScriptTest : GameTestBase
    {

        protected ClearScriptSystem scriptSystem;
        [Fact]
        public void Constructor1Tests()
        {


            scriptSystem = new ClearScriptSystem(Services);
            scriptSystem.Initialize();
            scriptSystem.loadFile("/local/src/demo/testModule.js");
            //scriptSystem.loadFile("//local/src/modules/add.js");

        }

        protected override async Task LoadContent()
        {
            await base.LoadContent();

            scriptSystem = new ClearScriptSystem(Services);
            scriptSystem.Initialize();
            //scriptSystem.loadFile("/local/src/main.js");
            scriptSystem.loadFile("//local/src/modules/add.js");
        }

        [Fact]
        public void RunTestGame()
        {
            RunGameTest(new ClearScriptTest());
        }
    }
}
