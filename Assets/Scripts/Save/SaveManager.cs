using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UIElements;

namespace Save
{

    public class GeneralSaveData
    {
        public bool IsFirstTime;
    }


    public class SaveManager
    {

        public GeneralSaveData LoadData(string filePath)
        {
            GeneralSaveData generalSaveData = null ;

            if(File.Exists(filePath))
            {
                try
                {

                    string dataToLoad = "";
                    using(FileStream stream = new FileStream(filePath,FileMode.Open))
                    {
                        using(StreamReader streamReader = new StreamReader(stream))
                        {
                            dataToLoad = streamReader.ReadToEnd();
                        }
                    }

                    generalSaveData = JsonUtility.FromJson<GeneralSaveData>(dataToLoad);
                }
                catch(Exception e)
                {
                    Debug.LogError("Error of loading : " + e);
                }
            }

            return generalSaveData;
        }

        public void SaveData(string filePath, GeneralSaveData generalSaveData)
        {
            string dataText = JsonUtility.ToJson(generalSaveData, true);

            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                outputFile.Write(dataText);
                outputFile.Close();
            }
        }

    public static void TestStructSave(string filePath, TestStruct data)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream fs = File.Create(filePath);
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(fs, data);
            fs.Close();
        }

        public static void WriteEndStats(string filePath, EndInfoStats data)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using (StreamWriter outputFile = new StreamWriter(filePath))
            {
                string[] lines = data.SaveDataString();

                for (int i = 0; i < lines.Length; i++)
                {
                    outputFile.WriteLine(lines[i]);
                }

                outputFile.Close();
            }

            //BinaryWriter binaryWriter = new BinaryWriter(fs);
            //binaryWriter.Write(data.SaveData());
            //fs.Close();



        }

        public static EndInfoStats ReadEndStats(string filePath)
        {
            EndInfoStats stats = new EndInfoStats();
            bool isExisting = File.Exists(filePath);
            if (!isExisting) return stats;

            StreamReader streamReader = new StreamReader(filePath);
            BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);

            stats.ReadData(streamReader);
            streamReader.Close();
            return stats;
        }


        public static void WriteGameData(string filePath, string[] lines)
        {
            StreamWriter outputFile = new StreamWriter(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                outputFile.WriteLine(lines[i]);
            }

            outputFile.Close();
        }

        public static string[] ReadGameData(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string[] lines = streamReader.ReadToEnd().Split('\n');
            streamReader.Close();
            return lines;
        }

    }
}
