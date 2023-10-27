using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace CustomInterface
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Spell.SpellManager))]
    public class SpellManagerUI : Editor
    {
        public override void OnInspectorGUI()
        {
            Spell.SpellManager charaterInventory = (Spell.SpellManager)target;
            base.OnInspectorGUI();
            if (GUILayout.Button("Load Spells"))
            {
                charaterInventory.LoadSpells();
            }
        }
    }
}
