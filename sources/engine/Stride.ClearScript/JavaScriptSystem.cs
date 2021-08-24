using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
using Stride.Games;
using Stride.Core.Serialization.Contents;
using Stride.Core.IO;
using System.IO;
using Microsoft.ClearScript.V8;
using Microsoft.ClearScript;
using System.Diagnostics;
using System.Net;
using Microsoft.ClearScript.JavaScript;

namespace Stride.ClearScript
{

    

    public class JavaScriptSystem : ComponentBase, IGameSystemBase,IJavaScriptSystem
    {
        protected V8ScriptEngine engine;

        public IServiceRegistry Services { get; private set; }
        public ContentManager Content { get; private set; }
        public JavaScriptSystem(IServiceRegistry registry)
            : base()
        {
            Services = registry;
         
            
        }

        protected override void Destroy()
        {
            if (engine != null) engine.Dispose();
            base.Destroy();
        }
        public void Initialize()
        {
            engine = new V8ScriptEngine();
            var loader = new StrideJavaScriptLoader();
            engine.DocumentSettings.Loader = loader;
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

            engine.AddHostType("Console", typeof(Console));
        }

        public object Execute()
        {
           return engine.Evaluate(new DocumentInfo { Category = ModuleCategory.Standard }, @"
                import * as Arithmetic from 'JavaScript/StandardModule/Arithmetic/Arithmetic.js';
                Arithmetic.Add(123, 456);
            ");
        }

        public async Task loadFile(string fileName)
        {
            using (var stream = VirtualFileSystem.OpenStream(fileName, VirtualFileMode.Open, VirtualFileAccess.Read))
            using (var streamReader = new StreamReader(stream))
            {
                //read the raw asset content
                try
                {
                    string source = await streamReader.ReadToEndAsync();
                    
                 var script=   engine.Evaluate(new DocumentInfo (){ 
                        Category = ModuleCategory.Standard 
                    }, source);
                    Console.WriteLine("------javascript c={0}", script.c);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

       
    }
}
