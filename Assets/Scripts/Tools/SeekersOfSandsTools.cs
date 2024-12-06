using ExcelLibrary.BinaryFileFormat;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Rendering.FilterWindow;

namespace SeekerOfSand.Tools
{

    public static class GeneralTools
    {

        #region Elemental Functions

        /// <summary>
        /// This will get you the index position on array of 4, only the base element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetElementalArrayIndex(GameElement element)
        {

            int indexElement = 0;
            switch (element)
            {
                case GameElement.NONE:
                    indexElement = -1;
                    Debug.LogWarning("This state is :" + element.ToString() );
                    break;
                case GameElement.WATER:
                    indexElement = 0;
                    break;
                case GameElement.AIR:
                    indexElement = 1;
                    break;
                case GameElement.FIRE:
                    indexElement = 2;
                    break;
                case GameElement.EARTH:
                    indexElement = 3;
                    break;
                default:
                    indexElement = -1;
                    Debug.LogError("This state is not handle by function :" + element.ToString() );
                    break;
            }
           

            return indexElement;
        }

        public static GameElement GetElementalFromIndex(int index)
        {

            int binaryNumber = 1 << index;
            return (GameElement)binaryNumber;
        }


        /// <summary>
        /// Get a random base element (Fire, Air, Water, Earth)
        /// </summary>
        /// <returns></returns>
        public static GameElement GetRandomBaseElement()
        {
            int randomIndex = Random.Range(0, 4);
            int binaryNumber  = 1 << randomIndex;
            return (GameElement)binaryNumber;
        }

        public static bool IsThisElementPresent( GameElement objEnum, GameElement specificElement )
        {
            return ((int)objEnum & (1 << (int)specificElement)) > 0;
        }

       

        #endregion
    }
}
