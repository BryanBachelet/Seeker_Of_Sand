using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class FileTransfer 
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string pathFile = "Assets\\Resources\\UpgradesTable.csv";
        string destination = pathToBuiltProject + "\\Seekers Of Sand_Data\\Resources";
        File.Copy(pathFile,destination);
        pathFile = "Assets\\Game data use\\Progression Demo - SpawnSheet.csv";

        destination = pathToBuiltProject + "\\Seekers Of Sand_Data\\";
        File.Copy(pathFile, destination);
        Debug.Log(pathToBuiltProject);
    }
}
