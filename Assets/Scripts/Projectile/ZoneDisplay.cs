using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneDisplay : MonoBehaviour
{
    [SerializeField] private float m_DestroyAfterTime = 3;
    private Vector3 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfter(m_DestroyAfterTime));
        initialPosition = transform.position;
    }

    private void Update()
    {
        transform.position = initialPosition;
    }
    public IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
