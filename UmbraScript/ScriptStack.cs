using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UmbraScript
{
    public class ScriptStack
    {
        public static string WorkingDirectory = ".\\Scripts\\";
        public static List<UmbraScript> Scripts = new List<UmbraScript>(); /**/
        public static void SetWorkingDir(string workingDir)
        {
            WorkingDirectory = workingDir;
        }
        public static FileInfo[] ScanScripts()
        {
            Logger.WriteLine(string.Format("[+] Scanning `{0}` for scripts...", ScriptStack.WorkingDirectory));

            if (!Directory.Exists(ScriptStack.WorkingDirectory))
            {
                Logger.WriteLine("[+] Scripts directory does not exist. Creating now...");
                Directory.CreateDirectory(ScriptStack.WorkingDirectory);
            }

            /* Finds all .cs source files. */
            var _dir = new DirectoryInfo(ScriptStack.WorkingDirectory);
            var scripts = _dir.GetFiles("*.cs", SearchOption.AllDirectories);

            Logger.WriteLine(string.Format("[+] `{0}` contains {1} scripts.", ScriptStack.WorkingDirectory, scripts.Length));
            return scripts;
        }

        public static Assembly CompileScript(UmbraScript script)
        {
            /* 
             * Create instance of C# code provider. 
             * We could also use VBCodeProvider for Visual Basic Scripts
             */
            var provider = new CSharpCodeProvider();

            /* 
             * We want the scripts to have access to the compiler assembly. 
             */
            var thisAssembly = Process.GetCurrentProcess().MainModule.FileName;
            Logger.WriteLine(string.Format("[!] Compiler Path: {0}", thisAssembly));

            /* TODO, each individual script should have it's own imports. Work this in later in reference to UmbraScript() */
            var references = new[] { thisAssembly, "mscorlib.dll", "System.Core.dll", "System.dll" };

            var parameters = new CompilerParameters(references, "output.tmp", false);

            parameters.GenerateExecutable = false; /* We do not want to generate any executable or .dll */
            parameters.GenerateInMemory = true; /* We want to run the scripts from in-memory. */
            parameters.TreatWarningsAsErrors = false; /* Ignore Warnings */

            CompilerResults _result = provider.CompileAssemblyFromSource(parameters, new[] { script.Source });

            /* Print any script errors. */
            if (_result.Errors.HasErrors)
            {

                foreach (CompilerError err in _result.Errors)
                {
                    Logger.WriteLine(err.ToString());
                }

                return null;
            }

            /*
            Module _module = _result.CompiledAssembly.GetModules()[0];

            Console.WriteLine("FQ Name: {0}", _module.FullyQualifiedName);
            Console.WriteLine("Name: {0}", _module.Name) ;
            Console.WriteLine("Scope Name: {0}",_module.ScopeName);
            */
            //ExploreAssembly(_module.Assembly);
            return _result.CompiledAssembly;
        }

        /*
            Finds all instances of IScript from an Assembly
         */
        public static List<IScript> CreateClassOBJs(UmbraScript script)
        {
            var _classes = new List<IScript>(); /* Create an object to store our IScript instances. */
            var assembly = script.Assembly; /* Select our working assembly */

            /* Use reflection to pull any classes implementing the IScript type. */
            foreach (Module m in assembly.GetModules())
            {
                foreach (Type t in m.GetTypes())
                {
                    /* Create an instance of the script for later usage. */
                    var handle = typeof(IScript).IsAssignableFrom(t)
                        ? (IScript)Activator.CreateInstance(t, script.Location, script.Source)
                        : null;

                    _classes.Add(handle); /* add the script instance to our list */
                }
            }
            return _classes; /* returns a list of scripts obtained from the assembly.*/
        }
    }
}
