using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace GuerhoubaGames.SaveData
{

    public struct PlayerData
    {
        public int runCount;
        public int farestRoom;
        public TimeSpan gameTime;

        // TODO : 
        // 1. Read Function 
        // 2. Write Function 

        public static PlayerData ReadData(string[] lineData, bool isDebugActive = false)
        {
            PlayerData playerData = new PlayerData();

            string value = lineData[0].Split(":")[1];
            playerData.runCount = int.Parse(value);
            value = lineData[1].Split(":")[1];
            playerData.farestRoom = int.Parse(value);

            string[] timeValue = lineData[2].Split(":");
            int days, hours, min, sec;

            sec = int.Parse(timeValue[timeValue.Length - 1]);
            min = int.Parse(timeValue[timeValue.Length - 2]);
            hours = int.Parse(timeValue[timeValue.Length - 3]);
  
            playerData.gameTime = new TimeSpan(hours, min, sec);

            if (isDebugActive) 
                Debug.Log("Update Game data file");

            return playerData;
        }

        public string[] WriteData()
        {
            string[] lineData = new string[3];

            lineData[0] = "Run Count :" + runCount.ToString();
            lineData[1] = "Farest Room:" + farestRoom.ToString();
            lineData[2] = "Play Time :" + gameTime.ToString("h':'m':'s");

            return lineData;
        }

        public void ShowDebug()
        {
            Debug.Log("Run Count :" + runCount.ToString());
            Debug.Log("Farest Room:" + farestRoom.ToString());
            Debug.Log("Play Time :" + gameTime.ToString());

        }


    }

    public class GameData : MonoBehaviour
    {
        public bool isDebugActive;

        static private DateTime m_startSessionTime;
        static private DateTime m_endSessionTime;
        static private TimeSpan m_timeSpanSession;

        static private PlayerData m_playerData;

        private const string m_playerDataFileName = "/PlayerData_";
        private const string m_playerDataFileExtention = ".sost";

        private static bool m_isDebugActive;

        public void Start()
        {
            m_startSessionTime = DateTime.Now;
            RetriveGameData();
            m_isDebugActive = isDebugActive;
        }

        private static string GetFilePath()
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath + "/Temp" + m_playerDataFileName + GameState.profileName + m_playerDataFileExtention;
            if (!Directory.Exists(Application.dataPath + "\\Temp"))
            {
                Debug.LogError("The folder temp need to be create");
                return "" ;
            }
#else
            string filePath = Application.dataPath + fileStatsName + GameState.profileName + ".txt";
#endif

            return filePath;
        }

        public void RetriveGameData()
        {
#if UNITY_EDITOR
            string filePath = Application.dataPath + "/Temp" + m_playerDataFileName + GameState.profileName + m_playerDataFileExtention;
            if (!Directory.Exists(Application.dataPath + "/Temp"))
            {
                Debug.LogError("The folder temp need to be create");
                return;
            }
#else
            string filePath = Application.dataPath + fileStatsName + GameState.profileName + ".txt";
#endif


            if (!File.Exists(filePath))
            {
                m_playerData = new PlayerData();
                Save.SaveManager.WriteGameData(filePath, m_playerData.WriteData());

            }
            else
            {
                m_playerData = PlayerData.ReadData(Save.SaveManager.ReadGameData(filePath), m_isDebugActive);
                m_playerData.ShowDebug();
            }

        }

        public static void UpdateRunCount() { m_playerData.runCount++; }
        public static void UpdateFarestRoom(int value)
        {
            if (value < m_playerData.farestRoom) return;

            m_playerData.farestRoom = value;
        }

        public static void UpdateGameDataAtQuit()
        {
            m_endSessionTime = DateTime.Now;

            m_timeSpanSession = m_endSessionTime - m_startSessionTime;

            m_playerData.gameTime += m_timeSpanSession;
            Debug.Log(m_timeSpanSession.ToString());

           if(m_isDebugActive) m_playerData.ShowDebug();

            Save.SaveManager.WriteGameData(GetFilePath(), m_playerData.WriteData());
        }
    }

}