using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleContainer : MonoBehaviour
{
    public int capsuleIndex = 7;

    [Header("Mouvement Parameters")]
    [SerializeField] private float m_feedbackTiming = 1;
    [SerializeField] private float m_moveHeight = 1;
    [SerializeField] private float m_angularSpeed = 30;

    private Vector3 m_startPosition;
    private float m_ratioTimer = 0;
    private float m_countdownTimer = 0;
    private bool m_inverseCountdown;


    #region Unity Functions

    private void Start()
    {
        m_startPosition = transform.position;
    }

    private void Update()
    {
       // CapsuleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Character.CharacterShoot>().AddSpell(capsuleIndex);
            Destroy(gameObject);

        }
    }

    #endregion

    #region Feedback Functions
    private void CapsuleMovement()
    {
        transform.position = Vector3.Lerp(m_startPosition, m_startPosition + Vector3.up * m_moveHeight, m_ratioTimer);
        transform.rotation *= Quaternion.Euler(0, m_angularSpeed * Time.deltaTime, 0);


        // Timer Conditions
        if (m_countdownTimer > m_feedbackTiming)
        {
            m_inverseCountdown = true;
        }

        if (m_countdownTimer < 0)
        {
            m_inverseCountdown = false;
        }

        // Feedback timer countdown
        if (m_inverseCountdown)
        {
            m_countdownTimer -= Time.deltaTime;
        }
        else
        {
            m_countdownTimer += Time.deltaTime;
        }

        m_ratioTimer = m_countdownTimer / m_feedbackTiming;
    }
    #endregion

    #region Constructeur Functions
    // Constructer
    CapsuleContainer(int index)
    {
        capsuleIndex = index;
    }
    #endregion
}
