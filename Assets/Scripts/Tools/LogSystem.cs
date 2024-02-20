using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace GuerhoubaTools
{
    public static class LogSystem
    {   
        private static bool m_isLogFileOpen;
        private static string path = Application.dataPath + "/SeekerOfSand_Log.txt";
        private static StreamWriter writer;
        private static bool isFirstTime = false;
        public static void CreateLogFile()
        {
            writer = new StreamWriter(path);
            isFirstTime = false;
        }

        public static void LogMsg(object msg, bool showInConsole = false, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string memberName = "")
        {
            if (isFirstTime)
                CreateLogFile();

            int index = filePath.LastIndexOf('\\');
            filePath = filePath.Substring(index + 1);
            
            writer.WriteLine("("+filePath + ":" + line + ")-->" + memberName +": "+msg);
            if (showInConsole) Debug.Log("(" + filePath + ":" + line + ")-- > " + memberName + ": " + msg);
        }

        public static void Close()
        {
            writer.Close();
        }
    }
}