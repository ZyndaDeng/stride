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

        public void addType(string typeName,Type type)
        {
            engine.AddHostType(typeName, type);
        }


        public object Evaluate(string code)
        {
            return engine.Evaluate(new DocumentInfo { Category = ModuleCategory.Standard }, code);
        }

        public object createComponentScript(string name)
        {
            try
            {
                var ret = Evaluate("new __stride.Components." + name + ".ctor()");
                return ret;
            }catch(Exception e)
            {
                return null;
            }
        }

        public void loadSrc(string path)
        {
            var scriptFiles = VirtualFileSystem.ListFiles(path, "*" + engine.DocumentSettings.FileNameExtensions, VirtualSearchOption.AllDirectories).Result;
            foreach(var file in scriptFiles)
            {
                loadFile(file,true);
            }

        }

        public void loadFile(string fileName, bool isModule)
        {
            using var stream = VirtualFileSystem.OpenStream(fileName, VirtualFileMode.Open, VirtualFileAccess.Read);
            using var streamReader = new StreamReader(stream);
            //read the raw asset content
            try
            {
                string source =  streamReader.ReadToEnd();
                var path = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
                var doc = new DocumentInfo(new Uri(@"\\js" + path));

                doc.Category = isModule ? ModuleCategory.Standard : DocumentCategory.Script;
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
