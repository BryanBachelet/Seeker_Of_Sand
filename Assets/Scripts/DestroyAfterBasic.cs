using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterBasic : MonoBehaviour
{
    [SerializeField] public float m_DestroyAfterTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfter(m_DestroyAfterTime));
    }

    public IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
