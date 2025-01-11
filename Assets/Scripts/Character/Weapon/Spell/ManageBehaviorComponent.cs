using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SpellSystem
{

    public class ManageBehaviorComponent : MonoBehaviour
    {
        public Component Component;
        public Type type; 

        public void OnEnable()
        {
           Component newInstance = gameObject.AddComponent(type);
            newInstance = Component;
        }
    }
}
