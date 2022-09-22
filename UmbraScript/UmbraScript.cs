using System;
using System.Collections.Generic;
using System.Reflection;

namespace UmbraScript
{
    /* Wrapper for IScript abstraction */
    public class UmbraScript
    {
        private Assembly m_assembly; /* Assembly of our script */
        private string[] m_references; /* TODO */
        private string m_scriptLocation; /* Path to script */
        private string m_sourceCode; /* Source code of script */
        private bool m_loadedSource = false;
        public Assembly Assembly { get { return this.m_assembly; } }
        public string Source { get { return this.m_sourceCode; } }
        public string Location { get { return this.m_scriptLocation; } }

        private Dictionary<string, IScript> m_classes;

        /*
            Usage example: 
            var script = new UmbraScript(".\\Scripts\\test.cs");
            To access a class, simply script[$scriptName]
            or
            script["Test"]
         */
        public IScript this[string key]
        {
            get
            {
                return this.m_classes.ContainsKey(key) ? this.m_classes[key] : null;
            }
        }

        public UmbraScript(string sourceFile)
        {
            this.m_classes = new Dictionary<string, IScript>();

            this.m_scriptLocation = sourceFile;

            /* Read file and store source. */
            try
            {
                this.m_sourceCode = Helper.ReadFileS(this.m_scriptLocation);
                this.m_loadedSource = true;
            }
            catch (Exception ex)
            {
                this.m_sourceCode = "";
            }

            Logger.WriteLine(string.Format("[+] Compiling {0} ...", this.m_scriptLocation));
            this.m_assembly = ScriptStack.CompileScript(this);

            var classes = ScriptStack.CreateClassOBJs(this);
            Logger.WriteLine(string.Format("[+] Found {0} IScript classes.\n", classes.Count));

            /* Each UmbraScript can have multiple IScript classes in one file. */
            foreach (IScript script in classes)
            {
                /* This is to handle name resolution conflicts on script names. */
                while (this.m_classes.ContainsKey(script.Name))
                {
                    var name = script.Name + "_";
                    Logger.WriteLine(string.Format("[!] Name Resolution: {0} Error: Key already exists. Renaming to {1} ...", script.Name, name));
                    script.Name = name;
                }

                this.m_classes.Add(script.Name, script);
                Logger.WriteLine(string.Format("[-] [{0}] by {1} [OK]", script.Name, script.Author));
                /*
                Console.WriteLine("[-] Script Name: {0}", script.Name);
                Console.WriteLine("[-] Script Author: {0}", script.Author);
                Console.WriteLine("[-] Description: {0}", script.Description);
                script.Load();
                */
            }
        }

        /* Pushes a variable onto each script's VarStore stack */
        public void PushAll(string key, object value)
        {
            foreach (KeyValuePair<string, IScript> kvp in this.m_classes)
            {
                kvp.Value.Push(key, value);
            }
        }
        /* Loads all scripts in stored in this.m_classes */
        public void LoadAll()
        {
            foreach (KeyValuePair<string, IScript> kvp in this.m_classes)
            {
                kvp.Value.Load();
            }
        }
        /* Unloads all scripts in stored in this.m_classes */
        public void UnloadAll()
        {
            foreach (KeyValuePair<string, IScript> kvp in this.m_classes)
            {
                kvp.Value.Unload();
            }
        }

    }
}
