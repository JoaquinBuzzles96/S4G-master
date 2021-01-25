#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor;
using UnityEngine;
using System.IO;

public class BM_AndroidBuildPrepartion : IPreprocessBuild
{
    public int callbackOrder { get { return 0; } }
    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        // Do the preprocessing here
        string[] fileEntries = Directory.GetFiles("Assets/Resources/Cases", "*.prefab");
        System.IO.Directory.CreateDirectory("Assets/StreamingAssets/");
        using (StreamWriter sw = new StreamWriter("Assets/StreamingAssets/alphabet.txt", false))
        {

            foreach (string filename in fileEntries)
            {
                sw.WriteLine(Path.GetFileNameWithoutExtension(filename));
                Debug.Log($"Guardamos el filename {filename} en Assets/StreamingAssets/alphabet.txt");
            }

        }
    }
}
#endif
