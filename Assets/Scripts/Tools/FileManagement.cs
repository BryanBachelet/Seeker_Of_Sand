using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GuerhoubaTools
{


    public class FileManagement
    {
        public static void WriteFileData(string filePath, byte[] data)
        {
            FileStream fs = File.OpenWrite(filePath);
            bool isExisting = File.Exists(filePath);

            if (!isExisting)
                fs = File.Create(filePath);

            BinaryWriter binaryWriter = new BinaryWriter(fs);
            binaryWriter.Write(data);
            fs.Close();


        }

        public static BinaryReader ReadBinaryFileData(string filepath)
        {
            byte[] data;
            if (!File.Exists(filepath))
            {
                data = new byte[0];
                Debug.LogError("The file you tried to read doesn't exist. Filepath : " + filepath);
                return null;
            }

            StreamReader streamReader = new StreamReader(filepath);
            BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);

            streamReader.Close();

            return binaryReader;


        }
        public static string[] GetLineCSV(StreamReader streamReader)
        {
            string line = streamReader.ReadLine();
            return line.Split(',');
        }

        public static string[,] ReadCSVFile(string filePath)
        {
            FileStream fs = File.OpenRead(filePath);
            StreamReader streamReader = new StreamReader(fs);

            string line = streamReader.ReadLine();
            string all = streamReader.ReadToEnd();
            int lineCount = all.Split('\n').Length;
            int itemCount = line.Split(',').Length;
            streamReader.BaseStream.Position = 0;
            streamReader.ReadLine();

            string[,] data = new string[lineCount - 1, itemCount];

            for (int i = 0; i < lineCount - 1; i++)
            {
                string[] dataLine = GuerhoubaTools.FileManagement.GetLineCSV(streamReader);
                for (int j = 0; j < itemCount; j++)
                {
                    data[i, j] = dataLine[j];
                    data[i, j] = dataLine[j];
                }

            }
            fs.Close();
            return data;
        }
    }


}
