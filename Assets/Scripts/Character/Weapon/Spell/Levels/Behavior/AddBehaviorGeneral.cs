using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpellSystem
{
    /// <summary>
    /// Type Behavior is regroup all behavior script type a spell can have
    /// </summary>
    public enum TypeBehavior
    {
        ReloadAreaBehavior = 0,
    }

    /// <summary>
    /// GeneralSpellBehavior is the base class for every spell behavior contains in TypeBehavior
    /// </summary>
    public class GeneralSpellBehavior :MonoBehaviour
    {
        public bool isActive;
        [HideInInspector] public bool isTemporary;
    }




    [CreateAssetMenu(fileName = "Add Behavior Level", menuName = "Spell/Add Behavior Level")]
    public class AddBehaviorGeneral : BehaviorLevel
    {
        public TypeBehavior type;

        public override void ActiveInstanceBehavior(GameObject instance, SpellProfil spellProfil)
        {
            string componentName = type.ToString();
            Type typeComponent1 = Type.GetType("SpellSystem."+componentName);
            Component componentAdd =  instance.GetComponent(typeComponent1);
            GeneralSpellBehavior spellBehavior = componentAdd as GeneralSpellBehavior;
            spellBehavior.isActive = true;
        }
    }
}
