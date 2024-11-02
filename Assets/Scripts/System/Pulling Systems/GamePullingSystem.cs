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
        public bool isDebugDeactivate;

        private Vector3 position;
        private Transform parent;
        private List<GameObject> bufferObjectList = new List<GameObject>();

        public PullObjects(GameObject baseObject, int quantity, int idGive, Vector3 Position, Transform parent)
        {
            bool isPullingMetaErrorActive = false;

            this.id = idGive;
            this.quantity = quantity;
            this.parent = parent;
            arrayQuantity = quantity;
            position = Position;

            gameObjectsArray = new GameObject[quantity];
            for (int i = 0; i < quantity; i++)
            {
                gameObjectsArray[i] = GameObject.Instantiate(baseObject, Position, Quaternion.identity, parent);
                SettingUpObject(gameObjectsArray[i], isPullingMetaErrorActive, baseObject.name, idGive);
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

                if (!isDebugDeactivate) Debug.Log("Pulling System:  Creation of new array for " + instance.name + ". Previous Size: " + gameObjectsArray.Length + " and new size is : " + newArray.Length);
                newArray[quantity] = instance;
                gameObjectsArray = newArray;
            }
            else
            {
                if (!isDebugDeactivate) Debug.Log("Register a new instance of " + instance.name);
                gameObjectsArray[quantity] = instance;
            }

            quantity++;
        }

        public void UpdadePullSize(int newQuantity, GameObject originObject)
        {
            bool isPullingMetaErrorActive = false;
            bool isAddingObject = newQuantity > quantity;
            bool isAlreadyHaveGoodSize =  arrayQuantity > newQuantity;

            GameObject[] newArray = new GameObject[newQuantity];
            if(isAddingObject && !isAlreadyHaveGoodSize) gameObjectsArray.CopyTo(newArray, 0);
            if (!isDebugDeactivate) Debug.Log("Pulling System:  Creation of new array for " + originObject.name + ". Previous Size: " + gameObjectsArray.Length + " and new size is : " + newArray.Length);

            if (isAddingObject)
            {
            
                int deltaQuantity = newQuantity - quantity;
                for (int i = 0; i < deltaQuantity; i++)
                {
                    GameObject instance = GameObject.Instantiate(originObject, position, Quaternion.identity, parent);
                    SettingUpObject(instance, isPullingMetaErrorActive, originObject.name, id);

                    if (!isAlreadyHaveGoodSize)
                        newArray[quantity + i] = instance;
                    else
                        gameObjectsArray[quantity + i] = instance; 

                }
            }
            else
            {
                // For the pull reductionn, this is very stiff version that do what intend to do without consideration of 
                // what will happen on the screen if a object is active. 

                int deltaQuantity = quantity - newQuantity;
                for (int i = newQuantity; i < quantity; i++)
                {
                    GameObject.Destroy(gameObjectsArray[i]);
                }
                arrayQuantity = gameObjectsArray.Length;
                currentlyUse = 0;
            }

            quantity = newQuantity;
  

            if (isAddingObject &&!isAlreadyHaveGoodSize)
            {
                gameObjectsArray = newArray;
                arrayQuantity = newQuantity;
            }
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

        private void SettingUpObject(GameObject instanceClone, bool isPullingMetaErrorActive, string baseObjectName, int id)
        {
            instanceClone.SetActive(false);

            PullingMetaData pullingMetaData = instanceClone.GetComponent<PullingMetaData>();
            if (pullingMetaData == null && !isPullingMetaErrorActive)
            {
                Debug.LogError("Add Pulling Meta Data component on " + baseObjectName);
                isPullingMetaErrorActive = true;
            }
            pullingMetaData.isActive = true;

            if(id == -1 )
            {
                Debug.LogError("Not a valid id for the object" + instanceClone.name);
            }
            pullingMetaData.id = id;
            pullingMetaData.bIsExtraSpawn = false;
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
        public bool isDebugRemove;
        public bool activeDetailDebug;

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
            pullObjects.isDebugDeactivate = isDebugRemove;
        }

        public void CreatePull(GameObject instance, int quantity = 10)
        {
            int id = GetID(instance.name);

            PullObjects pullObjects = new PullObjects(instance, quantity, id, transform.position, parent);

            m_listObjectToPull.Add(pullObjects);
            m_idList.Add(id);
            m_originalPullObjectList.Add(instance);
            pullObjects.isDebugDeactivate = isDebugRemove;
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
                if (activeDetailDebug) Debug.Log(instanceObj.name + " has been pull");
                return instanceObj;
            }
            else
            {
               
                GameObject instance = GameObject.Instantiate(m_originalPullObjectList[indexPull], Vector3.zero, Quaternion.identity, parent);
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
            if(id == -1)
            {
                if (!isErrorRemove) Debug.LogError("This object" + instance.name + " don't have a valid id");
            }
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
                pullObjects.currentlyUse = Mathf.Clamp(pullObjects.currentlyUse, 0,int.MaxValue);
            }
        }


        /// <summary>
        /// Allow to update the pull quantity size.
        /// Warning : This function can be costful so use it carefully
        /// </summary>
        /// <param name="newPullSize"> Setting up the new pull size </param>
        /// <param name="id"> Pull ID number</param>
        public void UpdatePullQuantity(int newPullSize, int id)
        {
            if (newPullSize <= 0)
            {
                if (isErrorRemove) Debug.LogError("Pulling System : the updating quantity isn't valid. Set a quantity above zero");
            }

            PullObjects pullObjects = m_listObjectToPull[m_idList.IndexOf(id)];
            if (newPullSize == pullObjects.quantity)
            {
                if (!activeDetailDebug) Debug.Log("Pulling System: Try to updating pull quantity with the same quantity");
                return;
            }
            pullObjects.UpdadePullSize(newPullSize, m_originalPullObjectList[m_idList.IndexOf(id)]); ;
        }



        #region Static Functions

        public static GameObject SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (GamePullingSystem.instance != null && GamePullingSystem.instance.isObjectPoolExisting(gameObject))
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
