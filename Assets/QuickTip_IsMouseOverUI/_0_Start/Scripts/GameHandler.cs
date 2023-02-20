using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using CodeMonkey.Utils;

namespace CodeMonkey_TestMouseOverUI {

    public class GameHandler : MonoBehaviour {

        public IUnitHandler unitHandler;


        private void Update() {
            if (Input.GetMouseButtonDown(0) && !IsMouseOverUIWithIgnores()) {
                SetUnitTargetPosition(UtilsClass.GetMouseWorldPositionWithZ());
            }
        }

        private bool IsMouseOverUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }

        private bool IsMouseOverUIWithIgnores() {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
            for (int i = 0; i < raycastResultList.Count; i++) {
                if (raycastResultList[i].gameObject.GetComponent<MouseUIClickthrough>() != null) {
                    raycastResultList.RemoveAt(i);
                    i--;
                }
            }

            return raycastResultList.Count > 0;
        }




        // Unit Functions
        private void SetUnitTargetPosition(Vector3 targetPosition) {
            unitHandler.SetTargetPosition(targetPosition);
        }
        // ----------------

        // Unit Interface
        public interface IUnitHandler {

            void SetTargetPosition(Vector3 targetPosition);

        }

    }

}