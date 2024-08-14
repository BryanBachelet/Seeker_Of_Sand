using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;
using GuerhoubaGames.SaveData;

namespace GuerhoubaTools
{
    public static class LogSystem
    {
        private static bool m_isLogFileOpen;
        private static string path = Application.dataPath + "/SeekerOfSand_Log.txt";
        private static StreamWriter writer;
        private static bool isFirstTime = true;

        public static void CreateLogFile()
        {

#if UNITY_EDITOR
#else
            path = Application.dataPath + "/SeekerOfSand_Log " + GameData.m_playerData.runCount.ToString() + ".txt";
            Debug.LogError("Path = "+ path);
#endif
            writer = new StreamWriter(path);
            isFirstTime = false;
        }

        public static void LogMsg(object msg, bool showInConsole = false, [CallerFilePath] string filePath = "", [CallerLineNumber] int line = 0, [CallerMemberName] string memberName = "")
        {
            if (isFirstTime)
            {
                CreateLogFile();
            }
            else
            {
                if (writer == null)
                    writer = new StreamWriter(path, true);
            }

            int index = filePath.LastIndexOf('\\');
            filePath = filePath.Substring(index + 1);

            string hourMinute = DateTime.Now.ToString("HH:mm::ss");
            writer.WriteLine(hourMinute + " (" + filePath + ":" + line + ")-->" + memberName + ": " + msg);
            if (showInConsole) Debug.Log("(" + filePath + ":" + line + ")-- > " + memberName + ": " + msg);
            writer.Close();
            writer = null;
        }

        public static void Close()
        {
            LogMsg("Close Log");
            writer = null;
            isFirstTime = true;

        }
    }
}