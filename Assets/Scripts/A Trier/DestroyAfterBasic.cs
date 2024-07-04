using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterBasic : MonoBehaviour
{
    private ObjectState state;
    [HideInInspector] public bool isNotDestroying;
    [SerializeField] public float m_DestroyAfterTime = 3;
    // Start is called before the first frame update
    void Start()
    {
        state = new ObjectState();
        GameState.AddObject(state);
        StartCoroutine(DestroyAfter(m_DestroyAfterTime));
    }


    public void LaunchTimer( )
    {
        StartCoroutine(DestroyAfter(m_DestroyAfterTime));
    }
    public IEnumerator DestroyAfter(float time)
    {
        float duration = 0;
        while (duration<time)
        {
            yield return Time.deltaTime;

            if(state.isPlaying)
            {
                duration += Time.deltaTime;
            }
        }

        if (!isNotDestroying)
            Destroy(this.gameObject);
        else
            gameObject.SetActive(false);
    }


}
