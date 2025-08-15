using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuerhoubaGames
{
    public class PedestalRoomBehavior : InteractionInterface
    {
        [HideInInspector] public RoomManager roomManager;

        public override void OnInteractionEnd(GameObject player)
        {
            return;
        }

        public override void OnInteractionStart(GameObject player)
        {
            roomManager.ResetEvents();
            isInteractable = false;
            isOpen = false;
        }

        public void Start()
        {
            isInteractable = false;
            RunManager runManagerInstance = RunManager.instance;
            runManagerInstance.OnNightStart += DeactivePedestal;
        }

        public void OnDestroy()
        {
            RunManager runManagerInstance = RunManager.instance;
            runManagerInstance.OnNightStart -= DeactivePedestal;
        }

        public void ActivatePedestal()
        {
            if (RunManager.instance.dayStep == DayStep.DAY)
                isInteractable = true;
        }

        public void DeactivePedestal()
        {
            isInteractable = false;
            isOpen = false;

        }
    }
}
