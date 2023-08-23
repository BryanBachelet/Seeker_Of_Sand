using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] private float radiusInteraction;
    [SerializeField] private LayerMask InteractibleObject;

    private float lastInteractionCheck;
    public float intervalCheckInteraction;
    public GameObject currentInteractibleObject;

    public GameObject ui_HintInteractionObject;
    // Start is called before the first frame update
    void Start()
    {
        lastInteractionCheck = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > lastInteractionCheck + intervalCheckInteraction)
        {
            NearPossibleInteraction();
            lastInteractionCheck = Time.time;
        }

    }

    public void NearPossibleInteraction()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, radiusInteraction, InteractibleObject);
        if(col.Length > 0)
        {
            if (ui_HintInteractionObject != null) { ui_HintInteractionObject.SetActive(true); };
            currentInteractibleObject = col[0].transform.gameObject;
            float nearest = Vector3.Distance(transform.position, col[0].transform.position);
            if(col.Length > 1)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    float newDistance = Vector3.Distance(transform.position, col[i].transform.position);
                    if (nearest >= newDistance)
                    {
                        currentInteractibleObject = col[i].transform.gameObject;
                        nearest = newDistance;
                    }
                }
            }

        }
        else if(col.Length == 0)
        {
            currentInteractibleObject = null;
            if(ui_HintInteractionObject != null) { ui_HintInteractionObject.SetActive(false); }
        }
    }

    public void ActionInteraction()
    {
        if(currentInteractibleObject != null) { currentInteractibleObject.GetComponent<AlatarHealthSysteme>().ActiveEvent(); }

    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radiusInteraction);
    }

}
