using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWater : MonoBehaviour
{
    public LayerMask ennemiColl;
    public LayerMask waterNear;
    [Range(1, 200)] public float radiusDetectionWater;
    [Range(1,10)] public int refreshDetection = 10;
    private float lastTimeRefreshed = 1;

    public GameObject prefabDragonWater;
    public List<GameObject> dragonList = new List<GameObject> ();
    RaycastHit hitBack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastTimeRefreshed + refreshDetection < Time.time)
        {
            RaycastHit hit;
            Vector2 rnd = Random.insideUnitCircle * radiusDetectionWater;
            Vector3 newRdvPosition = transform.position + new Vector3(rnd.x, 30, rnd.y);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(newRdvPosition, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, waterNear))

            {
                Collider[] col = Physics.OverlapSphere(this.transform.position, radiusDetectionWater, ennemiColl);
                int rndEnnemy = Random.Range(0, col.Length -1);
                Debug.DrawRay(newRdvPosition, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                Debug.Log("Did Hit");
                lastTimeRefreshed = Time.time;
                GameObject lastDragon = Instantiate(prefabDragonWater, hit.point, Quaternion.identity);
                lastDragon.GetComponent<DestroyAfterBasic>().enabled = true;
                SmoothFollow experienceMove = lastDragon.GetComponent<SmoothFollow>();
                MoveUp moveUp = lastDragon.GetComponent<MoveUp>();
                moveUp.target = col[rndEnnemy].transform;
                experienceMove.target = col[rndEnnemy].transform;
                //experienceMove.InitComponent();

                dragonList.Add(lastDragon);
                lastDragon.SetActive(true);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
            hitBack = hit;


        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, radiusDetectionWater);
        if(hitBack.point != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, 30, 0), hitBack.point);
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.DrawLine(transform.position + new Vector3(0, 30, 0), transform.TransformDirection(Vector3.down) * 100);
            Gizmos.color = Color.gray;
        }

    }
}
