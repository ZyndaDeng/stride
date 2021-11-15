using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Stride.ClearScript
{
    public interface IClearScriptVM
    {
        object Evaluate(string code);
        object createComponentScript(string name);
        Task loadSrc(string path);
        Task loadFile(string fileName);
        //object CreatePromiseForTask<T>(Task<T> task);
        //object CreatePromiseForTask(Task task);

        //Task<object> CreateTaskForPromise(object promise);
    }
}
