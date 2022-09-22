using System.Collections.Generic;

namespace UmbraScript
{
    public interface IScript
    {
        string Author { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        string Source { get; set; }
        string Location { get; set; }
        bool Loaded { get; set; } /* Indicates if script has been loaded yet. */
        Dictionary<string, object> VarStore { get; set; }

        /*
            Push should be used to push variables from the
            main assembly. (In this case assembly_valheim.dll)
         */
        void Push(string key, object value);
        /* Used to retrieve values from VarStore. */
        object Get(string key);

        /* This is called when the script is compiled and loaded. */
        void Load();

        /* Put any shutdown logic here. */
        void Unload();
    }
}
