using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.ClearScript.V8;
using Microsoft.ClearScript;
using Microsoft.ClearScript.JavaScript;
using Stride.Core.IO;
using Stride.Games;
using Stride.Core;

namespace Stride.ClearScript
{

    public class ClearScriptVM :  GameSystemBase, IClearScriptVM
    {
        protected V8ScriptEngine engine;

       
        public ClearScriptVM(IServiceRegistry registry) : base(registry)
        {
            
        }

        public override void Initialize()
        {
            engine = new V8ScriptEngine();
            var loader = new StrideJavaScriptLoader();
            engine.DocumentSettings.Loader = loader;
            engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading;

            engine.AddHostType("MSConsole", typeof(Console));
        }


        public object Evaluate(string code)
        {
            return engine.Evaluate(new DocumentInfo { Category = ModuleCategory.Standard }, code);
        }

        public object createComponentScript(string name)
        {
            try
            {
                var ret = Evaluate("new __Stride.Components." + name + ".ctor()");
                return ret;
            }catch(Exception e)
            {
                return null;
            }
        }

        public async Task loadSrc(string path)
        {
            var scriptFiles = VirtualFileSystem.ListFiles(path, "*" + engine.DocumentSettings.FileNameExtensions, VirtualSearchOption.AllDirectories).Result;
            foreach(var file in scriptFiles)
            {
               await loadFile(file);
            }

        }

        public async Task loadFile(string fileName)
        {
            using var stream = VirtualFileSystem.OpenStream(fileName, VirtualFileMode.Open, VirtualFileAccess.Read);
            using var streamReader = new StreamReader(stream);
            //read the raw asset content
            try
            {
                string source = await streamReader.ReadToEndAsync();
                var path = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
                var doc = new DocumentInfo(new Uri(@"\\js" + path));

                doc.Category = ModuleCategory.Standard;
                var script = engine.Evaluate(doc, source);
                // Console.WriteLine("------javascript c={0}", script.c);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

       
        protected override void Destroy()
        {
            if (engine != null) engine.Dispose();
        }
        

        //public object CreatePromiseForTask<T>(Task<T> task)
        //{
        //    return engine.CreatePromiseForTask(task);
        //}

        //public object CreatePromiseForTask(Task task)
        //{
        //    return engine.CreatePromiseForTask(task);
        //}

        //public Task<object> CreateTaskForPromise(object promise)
        //{
        //    return engine.CreateTaskForPromise(promise);
        //}
    }
}
