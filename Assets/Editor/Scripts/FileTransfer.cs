using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class FileTransfer 
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string pathFile = "Assets\\Resources\\UpgradeTable.csv";
        int lastIndex = pathToBuiltProject.LastIndexOf('/');
        string destination = pathToBuiltProject.Substring(0, lastIndex);
        destination = destination + "\\Seekers Of Sand_Data\\Resources" + "\\UpgradeTable.csv";

        File.Copy(pathFile, destination,true) ;

        pathFile = "Assets\\Game data use\\Progression Demo - SpawnSheet.csv";
        destination = pathToBuiltProject.Substring(0, lastIndex);
        destination = destination + "\\Seekers Of Sand_Data\\" + "\\Progression Demo -SpawnSheet.csv";
        File.Copy(pathFile, destination, true);

    }
}
