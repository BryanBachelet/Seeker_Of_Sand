using System;
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
            bool isPullingMetaErrorActive = false;
            this.id = idGive;
            this.quantity = quantity;
            arrayQuantity = quantity;
            gameObjectsArray = new GameObject[quantity];
            for (int i = 0; i < quantity; i++)
            {
                gameObjectsArray[i] = GameObject.Instantiate(baseObject, Position, Quaternion.identity, parent);
                gameObjectsArray[i].SetActive(false);

                PullingMetaData pullingMetaData = gameObjectsArray[i].GetComponent<PullingMetaData>();
                if (pullingMetaData == null && !isPullingMetaErrorActive)
                {
                    Debug.LogError("Add Pulling Meta Data component on " + baseObject.name);
                    isPullingMetaErrorActive = true;
                }
                pullingMetaData.isActive = true;
                pullingMetaData.id = idGive;
                pullingMetaData.bIsExtraSpawn = false;
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

                Debug.Log("Pulling System:  Creation of new array for " + instance.name + ". Previous Size: " + gameObjectsArray.Length + " and new size is : " + newArray.Length);
                newArray[quantity] = instance;
                gameObjectsArray = newArray;
            }
            else
            {
                Debug.Log("Register a new instance of " + instance.name);
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

        public GameObject GetFirstObjectAvailable()
        {
            for (int i = 0; i < quantity; i++)
            {
                if (!gameObjectsArray[i].activeSelf)
                {
                    return gameObjectsArray[i];
                }

            }

            return null;
        }
    }


    public class GamePullingSystem : MonoBehaviour
    {
        // Setup Object to be create
        // Need to be able to identify the object to pull;
        public GameObject[] PrefabToInstance;
        public int[] QuantityToInstance;
        private List<PullObjects> m_listObjectToPull = new List<PullObjects>();
        public List<int> m_idList = new List<int>();
        public List<GameObject> m_originalPullObjectList = new List<GameObject>();

        public static GamePullingSystem instance;
        public Transform parent;

        [Header("Pulling Info")]
        public bool isWarningRemove;
        public bool isErrorRemove;
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

        public void CreatePull(PullConstrutionData pullConstrutionData)
        {
            int id = pullConstrutionData.idInstance;

            PullObjects pullObjects = new PullObjects(pullConstrutionData.instance, pullConstrutionData.count, id, transform.position, parent);

            m_listObjectToPull.Add(pullObjects);
            m_idList.Add(id);
            m_originalPullObjectList.Add(pullConstrutionData.instance);
        }

        public void CreatePull(GameObject instance, int quantity = 10)
        {
            int id = GetID(instance.name);

            PullObjects pullObjects = new PullObjects(instance, quantity, id, transform.position, parent);

            m_listObjectToPull.Add(pullObjects);
            m_idList.Add(id);
            m_originalPullObjectList.Add(instance);
        }

        public void DeletePull(int id)
        {
            int indexPull = m_idList.IndexOf(id);
            if (indexPull == -1)
            {
                if (!isErrorRemove) Debug.LogError("Pulling System : No pull of object with that id " + id.ToString() + " To find the object, go up in call stack ");
            }
            PullObjects pullObjects = m_listObjectToPull[indexPull];
            pullObjects.CleanPull();
            m_listObjectToPull.Remove(pullObjects);
            m_originalPullObjectList.RemoveAt(indexPull);
        }

        public bool isObjectPoolExisting(GameObject objectInstance)
        {
            int id = GetID(objectInstance.name);
            return m_idList.Contains(id);
        }

        public GameObject SpawnObject(GameObject objectInstance)
        {
            int id = GetID(objectInstance.name);
            int indexPull = m_idList.IndexOf(id);
            if (indexPull == -1)
            {
                if (!isErrorRemove) Debug.LogError("Pulling System : No pull of object with that id " + id + " and name :" + objectInstance.name + " To find the object, go up in call stack ");
                return null;
            }

            PullObjects pullObjects = m_listObjectToPull[indexPull];
            if (pullObjects.currentlyUse < pullObjects.quantity)
            {

                GameObject instanceObj = pullObjects.GetFirstObjectAvailable();
                PullingMetaData pullingMetaData = instanceObj.GetComponent<PullingMetaData>();


                instanceObj.SetActive(true);
                pullObjects.currentlyUse++;
                pullingMetaData.ResetOnSpawn();
                if (activeDebug) Debug.Log(instanceObj.name + " has been pull");
                return instanceObj;
            }
            else
            {
                int index = m_idList.IndexOf(indexPull);
                GameObject instance = GameObject.Instantiate(m_originalPullObjectList[index], Vector3.zero, Quaternion.identity, parent);
                PullingMetaData pullingMetaData = instance.GetComponent<PullingMetaData>();
                pullingMetaData.id = indexPull;
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

            if (pullingMetaData.bIsExtraSpawn)
            {
                pullObjects.RegisterNewInstance(instance);
                pullingMetaData.bIsExtraSpawn = false;
            }
            else
            {
                pullObjects.currentlyUse--;
            }
        }


        #region Static Functions

        public static GameObject SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (GamePullingSystem.instance != null)
            {
                GameObject instance = GamePullingSystem.instance.SpawnObject(gameObject);
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                return instance;
            }
            else
            {
                GameObject instance = Instantiate(gameObject, position, rotation, parent);
                return instance;
            }

        }

        public static int GetID(string name)
        {
            return GetDeterministicHashCode(name);
        }

        public static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        #endregion
    }
}
