using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool_UiMovement : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    [SerializeField] private float timeToMoveOn = 2;
    [HideInInspector] public UIDispatcher dispatcher;
    [HideInInspector] public Vector3 positionStart;
    [HideInInspector] public Vector3 positionEnd;
    [HideInInspector] public bool activeMovement = false;
    [SerializeField] public AnimationCurve scale;
    // Start is called before the first frame update
    void Start()
    {
        lifeTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeMovement) return;
        lifeTime += Time.deltaTime;
        float progress = lifeTime / timeToMoveOn;
        if (progress < 1)
        {
            transform.position = Vector3.Lerp(positionStart, positionEnd, progress);
            transform.localScale = Vector3.one * scale.Evaluate(progress);
        }
        else
        {
            transform.position = positionEnd;
            dispatcher.lastObjectCreated.Remove(this);
            Destroy(this.gameObject);
        }
    }

    public void SetupSpeed()
    {

    }
    
    private void OnDestroy()
    {

    }
}
