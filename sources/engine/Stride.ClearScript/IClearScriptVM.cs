using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace Stride.ClearScript
{
    public interface IClearScriptVM
    {
        object Evaluate(string code);
        object createComponentScript(string name);
        void loadSrc(string path);
        void loadFile(string fileName, bool isModule);
        void addType(string typeName, Type type);
        //object CreatePromiseForTask<T>(Task<T> task);
        //object CreatePromiseForTask(Task task);

        //Task<object> CreateTaskForPromise(object promise);
    }
}
