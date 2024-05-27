using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    public bool changeSpot = false;
    private bool changeSpotRecently = false;

    public Vector4 nextPosition;

    public GameObject[] activeBirdChild;
    private Animator[] m_activeBirdChildAnimator;
    private BirdMouvement[] m_activeBirdChildMouvement;

    public static Transform playerRef;
    public float distancePlayer = 0;
    // Start is called before the first frame update
    void Start()
    {

        if(playerRef == null)
        {
            playerRef = GameObject.Find("Player").transform;
        }
        activeBirdChild = new GameObject[transform.childCount];
        m_activeBirdChildAnimator = new Animator[activeBirdChild.Length];
        m_activeBirdChildMouvement = new BirdMouvement[activeBirdChild.Length];
        for (int i = 0; i < activeBirdChild.Length; i++)
        {
            activeBirdChild[i] = transform.GetChild(i).gameObject;
            m_activeBirdChildAnimator[i] = activeBirdChild[i].GetComponent<Animator>();
            m_activeBirdChildMouvement[i] = activeBirdChild[i].GetComponent<BirdMouvement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(changeSpot)
        {
            changeSpot = false;
            StartCoroutine(ActiveChangement());
            ChangeSpotFunction();
        }
        if(!changeSpotRecently)
        {
            distancePlayer = Vector3.Distance(nextPosition, playerRef.position);
            if(distancePlayer < 50)
            {
                changeSpot = true;
            }
        }
    }
    
    public void ChangeSpotFunction()
    {
        nextPosition = EventHolder.NewSpot((int)nextPosition.w);
        for(int i = 0; i < activeBirdChild.Length; i++)
        {
            m_activeBirdChildMouvement[i].GetNewPositionToGo(nextPosition);
            m_activeBirdChildAnimator[i].SetBool("flying", true);
        }
    }

    IEnumerator ActiveChangement()
    {
        changeSpotRecently = true;
        yield return new WaitForSeconds(5);
        changeSpotRecently = false;
    }
}
