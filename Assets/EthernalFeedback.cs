using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
public class EthernalFeedback : MonoBehaviour
{
    public LineRenderer[] lineRender;
    public Transform[] refPosTransform = new Transform[3];
    public Transform elecObject;
    public float currentWidth;
    public float initialWidth = 0.1f;
    public float widthToGo;
    public float lerpDuration = 1;
    public bool currentlyAttacking = false;
    public bool activation = false;
    private float timeSpend = 0;

    public GameObject player;
    public AnimationCurve scaleYInTime;
    private VisualEffect vfx;

    public GameObject explosionLaser;
    public LayerMask lm;
    // Start is called before the first frame update
    void Start()
    {
        vfx = elecObject.GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 300 && !currentlyAttacking) { activation = true; }
        if(activation && !currentlyAttacking)
        {
            activation = false;
            currentlyAttacking = true;
            timeSpend = 0;
            currentWidth = 0.1f;

        }
        if(currentlyAttacking)
        {
            UpdateLineRender();
        }
        //else
        //{
        //    refPosTransform[3].position = player.transform.position;
        //}
    }

    public void UpdateLineRender()
    {

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        Vector3 position = player.transform.position;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        Vector3 direction = (position - elecObject.transform.position).normalized;
        if (Physics.Raycast(elecObject.position, elecObject.TransformDirection(direction), out hit, Mathf.Infinity, lm))
        {
            Debug.DrawRay(elecObject.position, elecObject.TransformDirection(direction) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }

        timeSpend += Time.deltaTime;
        float percent = timeSpend / lerpDuration;
        //currentWidth = Mathf.Lerp(initialWidth, widthToGo, percent);
        Vector3 newPosition = Vector3.Lerp(transform.position, refPosTransform[3].position, percent);
        //if(percent >= 0.99f)


        //for (int i = 0; i < lineRender.Length; i++)

        //{

        //    lineRender[i].widthMultiplier = initialWidth;

        //}


        //elecObject.localScale = new Vector3(1, scaleYInTime.Evaluate(percent), 1);
        vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(percent));
        vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(percent) * 0.9f);
        vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(percent) * 0.65f);
        if (percent <= 0.75f)
        {
            refPosTransform[3].position = hit.point;
        }
        else if (percent >= 0.99f)
        {
            //elecObject.localScale = new Vector3(1, scaleYInTime.Evaluate(0), 1);
            //refPosTransform[0].position = elecObject.position;
            //refPosTransform[3].position = position;
            vfx.SetFloat("Thickness_I", scaleYInTime.Evaluate(0));
            vfx.SetFloat("Thickness_II", scaleYInTime.Evaluate(0) * 0.9f);
            vfx.SetFloat("Thickness_III", scaleYInTime.Evaluate(0) * 0.65f);
            Instantiate(explosionLaser, refPosTransform[3].position, Quaternion.identity);
            currentlyAttacking = false;
        }

    }
}
