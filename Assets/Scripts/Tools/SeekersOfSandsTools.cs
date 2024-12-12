using ExcelLibrary.BinaryFileFormat;
using GuerhoubaGames.GameEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        public static int GetElementalArrayIndex(GameElement element , bool isNoneAdd = false)
        {


            int indexElement = 0;
            if(isNoneAdd)
                indexElement = 1;
            switch (element)
            {
                case GameElement.NONE:
                    indexElement += -1;
                    Debug.LogWarning("This state is :" + element.ToString() );
                    break;
                case GameElement.WATER:
                    indexElement += 0;
                    break;
                case GameElement.AIR:
                    indexElement += 1;
                    break;
                case GameElement.FIRE:
                    indexElement += 2;
                    break;
                case GameElement.EARTH:
                    indexElement += 3;
                    break;
                default:
                    indexElement = -1;
                    Debug.LogWarning("This state is not handle by function :" + element.ToString() );
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
            int indexSpecifixElement = (int)specificElement;
            return ((int)objEnum & indexSpecifixElement) > 0;
        }


        //Temp, will need better sytem
        public static GameElement GetFirstBaseElement(GameElement objEnum)
        {
            if(IsThisElementPresent(objEnum,GameElement.WATER)) return GameElement.WATER;
            if(IsThisElementPresent(objEnum,GameElement.AIR)) return GameElement.AIR;
            if(IsThisElementPresent(objEnum,GameElement.FIRE)) return GameElement.FIRE;
            if(IsThisElementPresent(objEnum,GameElement.EARTH)) return GameElement.EARTH;

            return GameElement.NONE;
        }
       

        #endregion
    }
}
