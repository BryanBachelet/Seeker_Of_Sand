using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{
    [SerializeField] private bool m_activeEffect = true;
    [SerializeField] private float m_RadiusFloat;
    [SerializeField] private bool m_ShowGizmo = false;
    private Transform generatorPosition;
    [SerializeField] private float m_timeBtwnWind = 3f;
    [SerializeField] private GameObject[] windObject = new GameObject[3];
    [SerializeField] private Transform m_playerPosition = null;
    private Vector3 m_offSetPlayer = Vector3.zero;
    private float m_tempsEcouleSpawn = 0;
    private GameObject lastWindSpawned = null;
    // Start is called before the first frame update
    void Start()
    {
        generatorPosition = gameObject.transform;
        m_offSetPlayer = new Vector3(m_playerPosition.position.x, 0, m_playerPosition.position.z) + generatorPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_activeEffect) return;

        transform.position = m_playerPosition.position + m_offSetPlayer;
        if(m_tempsEcouleSpawn > m_timeBtwnWind)
        {
            int rndWindObject = Random.Range(0, 3);
            Vector2 rndPositionSpawnObject = Random.insideUnitCircle * m_RadiusFloat;
            lastWindSpawned = Instantiate(windObject[rndWindObject], new Vector3(generatorPosition.position.x + rndPositionSpawnObject.x, generatorPosition.position.y, generatorPosition.position.z + rndPositionSpawnObject.y), Quaternion.identity);
            m_tempsEcouleSpawn = 0;
            StartCoroutine(DestroyObject(m_timeBtwnWind -1));
        }
        else
        {
            m_tempsEcouleSpawn += Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        if(m_ShowGizmo)
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, m_RadiusFloat);
        }
    }

    IEnumerator DestroyObject(float temps)
    {
        yield return new WaitForSeconds(temps);
        Destroy(lastWindSpawned);
    }
}
