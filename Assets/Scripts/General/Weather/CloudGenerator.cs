using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    [SerializeField] private bool m_activeEffect = true;
    [SerializeField] private float m_RadiusFloat;
    [SerializeField] private bool m_ShowGizmo = false;
    private Transform generatorPosition;
    [SerializeField] private int m_maxCloud = 20;
    [SerializeField] private GameObject[] cloudObject = new GameObject[3];
    [SerializeField] private Transform m_playerPosition = null;
    private float m_tempsEcouleSpawn = 0;
    public List<CloudID> cloudSpawnList = new List<CloudID>();
    // Start is called before the first frame update
    void Start()
    {
        generatorPosition = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_activeEffect) return;

        //transform.position = m_playerPosition.position + new Vector3(0, 60, 0);
        if (m_tempsEcouleSpawn > 2f && cloudSpawnList.Count < m_maxCloud)
        {
            int rndCloudObject = Random.Range(0, 3);
            Vector2 rndPositionSpawnObject = Random.insideUnitCircle * m_RadiusFloat;
            int rndY = Random.Range(0, 16);
            GameObject lastCloudGenerated = Instantiate(cloudObject[rndCloudObject], new Vector3(generatorPosition.position.x + rndPositionSpawnObject.x, generatorPosition.position.y + rndY, generatorPosition.position.z + rndPositionSpawnObject.y), Quaternion.identity);
            CloudID lastCloudIDGenerated = lastCloudGenerated.GetComponent<CloudID>();
            lastCloudIDGenerated.m_playerTransform = m_playerPosition;
            lastCloudIDGenerated.m_CloudGenerator = this;
            cloudSpawnList.Add(lastCloudIDGenerated);
            m_tempsEcouleSpawn = 0;            
        }
        else
        {
            m_tempsEcouleSpawn += Time.deltaTime;
        }
    }

    public void DestroyListObject(CloudID cloudObject)
    {
        cloudSpawnList.Remove(cloudObject);
        Destroy(cloudObject.gameObject);
    }
    private void OnDrawGizmos()
    {
        if (m_ShowGizmo)
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, m_RadiusFloat);
        }
    }
}
