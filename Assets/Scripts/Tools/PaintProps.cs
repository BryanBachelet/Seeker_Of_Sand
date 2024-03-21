using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PaintProps : MonoBehaviour
{
    public bool activePainting;
    public bool savePosToPaint;
    public Vector3 newPaint;
    public Vector3 nextPaint;
    public float range;
    public GameObject[] prefabProps;
    public List<bool> isRotatingProps;
    public LayerMask groundLayer;
    public int density;
    public bool activeDebug;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (savePosToPaint)
        {
            nextPaint = newPaint;
            savePosToPaint = false;
        }
        if (activePainting)
        {
            GenerateNewThings(nextPaint);
            //activePainting = false;
            nextPaint = newPaint;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(nextPaint, range);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(newPaint, range);
    }

    public void GenerateNewThings(Vector3 position)
    {
        float posY = position.y;

        GameObject parent = new GameObject();
        parent.name = "new PropsHolder";
        parent.transform.parent = this.transform;
        for (int i = 0; i < density; i++)
        {
            Vector3 newPosition = Random.insideUnitCircle * range;
            position = new Vector3(position.x + newPosition.x, posY, position.z + newPosition.y);
            Vector3 origineRaycast = position + new Vector3(0, 10, 0);
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(origineRaycast, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, groundLayer))
            {
                Debug.DrawRay(origineRaycast, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                if (!activeDebug)
                {
                    int rnd = Random.Range(0, prefabProps.Length);
                    Quaternion rotationProp = Quaternion.identity;
                    if (isRotatingProps[rnd]) rotationProp = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    Instantiate(prefabProps[rnd], hit.point, rotationProp, parent.transform);
                }

                Debug.Log("Did Hit");
            }
            else
            {
                Debug.DrawRay(origineRaycast, transform.TransformDirection(Vector3.down) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    public void OnValidate()
    {
        if (isRotatingProps.Count == prefabProps.Length) return;

        if (isRotatingProps.Count < prefabProps.Length)
        {
            for (int i = isRotatingProps.Count; i < prefabProps.Length; i++)
            {
                isRotatingProps.Add(false);
            }
        }
        else
        {
            int count = isRotatingProps.Count;
            for (int i = count-1; i >= prefabProps.Length; i--)
            {
                isRotatingProps.RemoveAt(i);
            }
        }
    }
}
