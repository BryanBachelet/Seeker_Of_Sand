using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
//using ExcelLibrary;

namespace Tracker
{
    public class StatsTracker
    {
        public static void CreateStatSheet(string dataPath, string destinationPath, ref string feedbackInfo, bool hasDebugInfo = false)
        {
            feedbackInfo = "";
            string[] paths = Directory.GetFiles(dataPath, "*.sost");

            if (paths.Length == 0)
            {
                feedbackInfo = "No file found";
                return;
            }

            if (hasDebugInfo)
            {
                Debug.Log("Files Found is " + paths.Length.ToString());
                for (int i = 0; i < paths.Length; i++)
                {
                    Debug.Log("File name is " + paths[i]);
                }
            } // File info debug

            // Find all stats data
            EndInfoStats[] endInfoStats = new EndInfoStats[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                StreamReader streamReader = new StreamReader(paths[i]);
                BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream);

                endInfoStats[i].ReadData(binaryReader);
                streamReader.Close();
            }

            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            FileStream fs = File.Create(destinationPath+"\\Stats.csv");
            for (int i = 0; i < endInfoStats.Length; i++)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(TransfomInfoStatsIntoString(endInfoStats[i]) + "\n");
                fs.Write(bytes,0,bytes.Length);
            }
            for(int i = 0; i < endInfoStats.Length; i++)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(ParseInfoStats(endInfoStats[i]) + "\n");
                fs.Write(bytes, 0, bytes.Length);
            }

            fs.Close();

            //System.Data.DataSet ds =  ExcelLibrary.DataSetHelper.CreateDataSet(destinationPath );
            //ExcelLibrary.DataSetHelper.CreateWorkbook(destinationPath + "\\Stats.xls", ds);
            //ExcelLibrary.SpreadSheet.Workbook wb = ExcelLibrary.SpreadSheet.Workbook.Load(destinationPath + "\\Stats.xls");
            

            feedbackInfo = "File created";

        }


        private static string TransfomInfoStatsIntoString(EndInfoStats endInfo)
        {
            string dataText = "";

            dataText = endInfo.durationGame.ToString("F0")+',';
            dataText += endInfo.nightValidate.ToString() + ',';
            dataText += endInfo.enemyKill.ToString() + ',';
            dataText += endInfo.roomCount.ToString("F0") + ',';
            dataText += endInfo.altarSuccessed.ToString() + ',';
            dataText += endInfo.altarRepeated.ToString() + ',';
           
            return dataText;
        }

        private static string[] ParseInfoStats(EndInfoStats endInfo)
        {
            string[] stringInfoStats = new string[6];
            stringInfoStats[0] = endInfo.durationGame.ToString("F0") + ',';
            stringInfoStats[1] = endInfo.nightValidate.ToString() + ',';
            stringInfoStats[2] = endInfo.enemyKill.ToString() + ',';
            stringInfoStats[3] = endInfo.roomCount.ToString("F0") + ',';
            stringInfoStats[4] = endInfo.altarSuccessed.ToString() + ',';
            stringInfoStats[5] = endInfo.maxCombo.ToString() + ','; 

            return stringInfoStats;
        }
    }

}
