using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Menu_MouvementCamera : MonoBehaviour
{
    public List<Vector3> position = new List<Vector3>();
    public List<Quaternion> rotation = new List<Quaternion>();

    public GameObject[] layoutInterface;
    public int lastLayout;
    public int currentLayout;
    public bool copyActivation = true;
    public int currentPosition = 0;
    public int nextPosition = 0;

    public bool activeRotation = false;
    public bool rotationEnCour = false;
    public bool debugActiveRotation = false;

    [Range(0,1)]
    public float progressTransition = 0;
    public GameObject selectionAimMode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activeRotation)
        {
            if(!rotationEnCour) { return; }
            if (progressTransition <= 0.99f)
            {
                progressTransition += Time.deltaTime;
                Debug.Log("current count [" + position.Count + "]");
                //if (currentPosition == position.Count - 1)
                //{
                 transform.SetPositionAndRotation(Vector3.Lerp(position[currentPosition], position[nextPosition], progressTransition), Quaternion.Lerp(rotation[currentPosition], rotation[nextPosition], progressTransition));
                //}
                //else
                //{
                //    transform.SetPositionAndRotation(Vector3.Lerp(position[currentPosition], position[currentPosition + 1], progressTransition), Quaternion.Lerp(rotation[currentPosition], rotation[currentPosition + 1], progressTransition));
                //}
            }
            else
            {
                progressTransition = 1;
            }
            if (progressTransition == 1)
            {
                currentPosition = nextPosition;
                rotationEnCour = false;
                activeRotation = false;
            }

        }
    }

    private void OnValidate()
    {
        if (copyActivation)
        {
            copyActivation = false;
            CopyPositionAndRotation(this.transform);
        }
        if (debugActiveRotation)
        {

            if (progressTransition <= 0.99f)
            {
                Debug.Log("current count [" + position.Count + "]");
                if (currentPosition == position.Count - 1)
                {
                    transform.SetPositionAndRotation(Vector3.Lerp(position[currentPosition], position[0], progressTransition), Quaternion.Lerp(rotation[currentPosition], rotation[0], progressTransition));
                }
                else
                {
                    transform.SetPositionAndRotation(Vector3.Lerp(position[currentPosition], position[currentPosition + 1], progressTransition), Quaternion.Lerp(rotation[currentPosition], rotation[currentPosition + 1], progressTransition));
                }
            }
            if (progressTransition == 1)
            {
                if (currentPosition + 1 > position.Count - 1)
                {
                    currentPosition = 0;
                }
                else
                {
                    currentPosition += 1;
                }
                for (int i = 0; i < layoutInterface.Length; i++)
                {
                    if (i == currentPosition)
                    {
                        layoutInterface[i].SetActive(true);
                    }
                    else
                    {
                        layoutInterface[i].SetActive(false);
                    }
                }
            }
        }

    }

    public void moveCamera(int MoveTo)
    {
        if (rotationEnCour) return;
        activeRotation = true;
        nextPosition = MoveTo;
        rotationEnCour = true;
        ChangeLayout();
        if(MoveTo == 3)
        {
            selectionAimMode.SetActive(true);
        }
        else
        {
            selectionAimMode.SetActive(false);
        }
        progressTransition = 0;
    }
    public void ChangeLayout()
    {
        for (int i = 0; i < layoutInterface.Length; i++)
        {
            if (i == nextPosition)
            {
                layoutInterface[i].SetActive(true);
            }
            else
            {
                layoutInterface[i].SetActive(false);
            }
        }
    }
    public void CopyPositionAndRotation(Transform currentTransform)
    {
        position.Add(currentTransform.position);
        rotation.Add(currentTransform.rotation);
    }

    public bool rotationActivation()
    {
        return true;
    }

}
