using System.Collections;
using UnityEngine;

public class FloatingMouvement : MonoBehaviour
{

    public Vector3 offsetPosition;
    public Vector3 maxPositionPosition;
    public float durationMouvement;

    private float m_timerMouvement;
    private bool m_isGoUp;
    private Vector3 m_basePosition;

    // Start is called before the first frame update
    void Start()
    {
        transform.position += offsetPosition;
        m_basePosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(m_basePosition, m_basePosition + maxPositionPosition, m_timerMouvement);
        if(m_isGoUp)
        {
            m_timerMouvement += Time.deltaTime;
            if (m_timerMouvement > durationMouvement) m_isGoUp = !m_isGoUp;
        }
        else
        {
            m_timerMouvement -= Time.deltaTime;
            if (m_timerMouvement < 0) m_isGoUp = !m_isGoUp;
        }
    }
}
