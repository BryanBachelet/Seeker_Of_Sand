using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbleWeedGenerator : MonoBehaviour
{
    [SerializeField] private bool m_activeEffect = true;

    [SerializeField] [Range(1, 6)] private float m_RadiusFloat;
    [SerializeField] private bool m_ShowGizmo = false;
    private Transform generatorPosition;
    [SerializeField] private int m_maxWeed = 10;
    [SerializeField] private GameObject[] WeedObject = new GameObject[3];
    [SerializeField] private Transform m_playerPosition = null;
    private float m_tempsEcouleSpawn = 0;
    public List<WeedID> weedSpawnedList = new List<WeedID>();
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
        if (m_tempsEcouleSpawn > 10f && weedSpawnedList.Count < m_maxWeed)
        {
            int rndWeedObject = Random.Range(0, 3);
            Vector2 rndPositionSpawnObject = Random.insideUnitCircle * m_RadiusFloat;
        
            GameObject lastWeedGenerated = Instantiate(WeedObject[rndWeedObject], new Vector3(generatorPosition.position.x + rndPositionSpawnObject.x * 15, generatorPosition.position.y + 50, generatorPosition.position.z + rndPositionSpawnObject.y * 15), Quaternion.identity);
            WeedID lastWeedIDGenerated = lastWeedGenerated.GetComponent<WeedID>();
            lastWeedIDGenerated.m_playerTransform = m_playerPosition;
            lastWeedIDGenerated.m_weedGenerator = this;
            weedSpawnedList.Add(lastWeedIDGenerated);
            m_tempsEcouleSpawn = 0;
        }
        else
        {
            m_tempsEcouleSpawn += Time.deltaTime;
        }
    }

    public void DestroyListObject(WeedID weedObject)
    {
        weedSpawnedList.Remove(weedObject);
        Destroy(weedObject.gameObject);
    }
    private void OnDrawGizmos()
    {
        if (m_ShowGizmo)
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, m_RadiusFloat * 15);
        }
    }
}
