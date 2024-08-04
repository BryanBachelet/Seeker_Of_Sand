using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Save
{

    public struct GeneralSaveData
    {

    }



    public class SaveManager
    {

        public static void TestStructSave(string filePath, TestStruct data)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream fs = File.Create(filePath);
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(fs ,data);
            fs.Close();
        }

        public static void WriteEndStats(string filePath, EndInfoStats data )
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            FileStream fs = File.Create(filePath);
            BinaryWriter binaryWriter = new BinaryWriter(fs);
            binaryWriter.Write(data.SaveData());
            fs.Close();

        }

        public static EndInfoStats ReadEndStats(string filePath)
        {
            EndInfoStats stats = new EndInfoStats();

            if (!File.Exists(filePath)) return stats;

            StreamReader streamReader = new StreamReader(filePath);
            BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);

            stats.ReadData(binaryReader);
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
           return streamReader.ReadToEnd().Split('\n');
        }

    }
}
