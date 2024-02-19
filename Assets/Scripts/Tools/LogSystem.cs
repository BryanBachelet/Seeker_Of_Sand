using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LogSystem 
{
    private static bool m_isLogFileOpen;
    private static string path = Application.dataPath + "/Assets/testLog.txt";

    public static void CreateLogFile(params object[] testObj)
    {
        StreamWriter writer = new StreamWriter(path);
        for (int i = 0; i < testObj.Length; i++)
        {
            writer.WriteLine(testObj[i]);
        }
        writer.Close();
    }
}
