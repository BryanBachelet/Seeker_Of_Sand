using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CustomInterface
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Character.CharacterInventory))]
    public class CharacterInventoryUI : Editor
    {
        public override void OnInspectorGUI()
        {
            Character.CharacterInventory charaterInventory = (Character.CharacterInventory)target;
            base.OnInspectorGUI();
            //if (GUILayout.Button("Load Spells"))
            //{
            //    charaterInventory.LoadSpells();
            //}
        }
    }
}
