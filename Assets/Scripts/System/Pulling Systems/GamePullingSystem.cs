using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GuerhoubaGames.Resources
{
    [System.Serializable]
    public class PullObjects
    {
        public int id;
        public int quantity;
        public int currentlyUse;
        public int arrayQuantity;
        public GameObject[] gameObjectsArray;
        public bool isDebugActive;

        private List<GameObject> bufferObjectList = new List<GameObject>();
        public PullObjects(GameObject baseObject, int quantity, int idGive, Vector3 Position, Transform parent)
        {
            this.id = idGive;
            this.quantity = quantity;
            arrayQuantity = quantity;
            gameObjectsArray = new GameObject[quantity];
            for (int i = 0; i < quantity; i++)
            {
                gameObjectsArray[i] = GameObject.Instantiate(baseObject, Position, Quaternion.identity, parent);
                gameObjectsArray[i].SetActive(false);

                PullingMetaData pullingMetaData = gameObjectsArray[i].GetComponent<PullingMetaData>();
                pullingMetaData.isActive = true;
                pullingMetaData.id = idGive;

             

            }
        }

        public void AddObjectAtRuntime(GameObject instance)
        {
            bufferObjectList.Add(instance);
        }

        public void RegisterNewInstance(GameObject instance)
        {
            bufferObjectList.Remove(instance);
            if (quantity == arrayQuantity)
            {
                arrayQuantity += 10;
                GameObject[] newArray = new GameObject[arrayQuantity];
                gameObjectsArray.CopyTo(newArray, 0);

                Debug.Log("Pulling System: Creation of new array. Previous Size: "+gameObjectsArray.Length + " and new size is : " +  newArray.Length );
                newArray[quantity] = instance;
                gameObjectsArray = newArray;
            }
            else
            {
                Debug.Log("Register a new instance");
                gameObjectsArray[quantity] = instance;
            }
          
            quantity++;
        }

        public void CleanPull()
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject.Destroy(gameObjectsArray[i]);
            }
        }
    }


    public class GamePullingSystem : MonoBehaviour
    {
        // Setup Object to be create
        // Need to be able to identify the object to pull;
        public GameObject[] PrefabToInstance;
        public int[] QuantityToInstance;
        private List<PullObjects> m_listObjectToPull = new List<PullObjects>();
        private List<int> m_idList = new List<int>();
        private List<GameObject> m_originalPullObjectList = new List<GameObject>();

        public static GamePullingSystem instance;
        public Transform parent;

        [Header("Pulling Info")]
        public bool isWarningRemove;
        public bool activeDebug;

        #region Unity Functions
        public void Awake()
        {
            instance = this;

            for (int i = 0; i < PrefabToInstance.Length; i++)
            {
                CreatePull(PrefabToInstance[i], QuantityToInstance[i]);
            }
        }
        #endregion

        public void CreatePull(GameObject instance, int quantity = 10)
        {
            int id = instance.GetInstanceID();

            PullObjects pullObjects = new PullObjects(instance, quantity, id, transform.position, parent);

            m_listObjectToPull.Add(pullObjects);
            m_idList.Add(id);
            m_originalPullObjectList.Add(instance);
        }

        public void DeletePull(int id)
        {
            PullObjects pullObjects = m_listObjectToPull[m_idList.IndexOf(id)];
            pullObjects.CleanPull();
            m_listObjectToPull.Remove(pullObjects);
            m_originalPullObjectList.RemoveAt(m_idList.IndexOf(id));
        }
        
        public bool isObjectPoolExisting(int id)
        {
            return m_idList.Contains(id);
        }

        public GameObject SpawnObject(int id)
        {
            PullObjects pullObjects = m_listObjectToPull[m_idList.IndexOf(id)];
            if (pullObjects.currentlyUse < pullObjects.quantity)
            {
                GameObject instanceObj = pullObjects.gameObjectsArray[pullObjects.currentlyUse];
                PullingMetaData pullingMetaData = instanceObj.GetComponent<PullingMetaData>();

                instanceObj.SetActive(true);
                pullObjects.currentlyUse++;
                pullingMetaData.ResetOnSpawn();
                if (activeDebug) Debug.Log(instanceObj.name + " has been pull");
                return instanceObj;
            }
            else
            {
                int index = m_idList.IndexOf(id);
                GameObject instance = GameObject.Instantiate(m_originalPullObjectList[index], Vector3.zero, Quaternion.identity, parent);
                PullingMetaData pullingMetaData = instance.GetComponent<PullingMetaData>();
                pullingMetaData.id = id;
                pullingMetaData.bIsExtraSpawn = true;
                pullingMetaData.isActive = true;

                if (!isWarningRemove) Debug.LogWarning("Pull System : Object is create");

                pullObjects.AddObjectAtRuntime(instance);
                pullingMetaData.ResetOnSpawn();
                return instance;
            }


        }

        public void ResetObject(GameObject instance, int id)
        {
            PullingMetaData pullingMetaData = instance.GetComponent<PullingMetaData>();
            PullObjects pullObjects = m_listObjectToPull[m_idList.IndexOf(id)];
            instance.SetActive(false);
         
            if(pullingMetaData.bIsExtraSpawn)
            {
                pullObjects.RegisterNewInstance(instance);
                pullingMetaData.bIsExtraSpawn = false;
            }
            else
            {
                pullObjects.currentlyUse--;
            }
        }

    }
}
