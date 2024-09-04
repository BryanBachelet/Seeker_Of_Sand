using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GuerhoubaGames.UI
{

    public class UITools : MonoBehaviour
    {
        public static UITools instance;
        public EventSystem eventSystem;
        public void Awake()
        {
            instance = this;
        }

        public void SetUIObjectSelect(GameObject gameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}