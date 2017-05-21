
using System.IO;
using UnityEditor;

namespace EditorService
{
    [InitializeOnLoad]
    static class Main
    {
        static Main()
        {
            Debug.Print("Main::ctor");

            EditorApplication.delayCall += initialize;
        }

        private static void initialize()
        {
            Debug.Print("Main::initialize");

            if (!File.Exists(_path))
            {
                File.Create(_path);

                var id = Server.instance.GetInstanceID();
                Debug.PrintFormat("Server InstanceID = {0}", id);
            }
        }

        private const string _path = "Temp/EditorService";
    }
}
