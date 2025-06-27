using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PlayerInput))]
public class CameraGodMode : MonoBehaviour
{
    [Header("Camera God Mode Variables")]
    [SerializeField] private float m_liftSpeed = 1.0f;
    [SerializeField] private float m_cameraMovementSpeed = 1.0f;
    [SerializeField] private float m_cameraAngleSpeed = 360;

    private Camera m_Camera;
    private PlayerInput m_PlayerInput;

    private float m_thesholdLevel = 3.0f;
    private bool m_rotationInputActive = false;
    private Vector2 m_mouseDeltaInputValue = Vector2.zero;
    private int m_cameraHeightInputValue = 0;
    private Vector2 m_camerarMoveInputValue = Vector2.zero;
  



    #region Unity Functions

    public void Start()
    {
        InitComponent();
    }

    public void Update()
    {
        LiftCamera();
        MoveCamera();
        RotationCamera();
    }

    #endregion

    private void InitComponent()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_Camera = GetComponent<Camera>();
    }

    #region Mouvement Functions
    private void LiftCamera()
    {
        transform.position += Vector3.up * m_liftSpeed * m_cameraHeightInputValue * Time.deltaTime;
    }

    private void MoveCamera()
    {
        transform.position += transform.forward * m_camerarMoveInputValue.y * m_cameraMovementSpeed * Time.deltaTime;
        transform.position += transform.right * m_camerarMoveInputValue.x * m_cameraMovementSpeed * Time.deltaTime;
    }

    private void RotationCamera()
    {
        if (!m_rotationInputActive) return;

        transform.rotation *= Quaternion.Euler(0f, m_mouseDeltaInputValue.x * m_cameraAngleSpeed* Time.deltaTime, 0f);
        transform.rotation *= Quaternion.Euler(-m_mouseDeltaInputValue.y * m_cameraAngleSpeed* Time.deltaTime, 0f, 0f);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0f);
    }


    #endregion

    #region Inputs Functions

    public void CameraMouvementInput(InputAction.CallbackContext ctx)
    {
        m_camerarMoveInputValue = ctx.ReadValue<Vector2>();
    }

    public void ActiveCameraRotationInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed || ctx.started)
        {
            m_rotationInputActive = true;
         
        }
        else
        {
            m_rotationInputActive = false;
        }
    }

    public void CameraRotationInput(InputAction.CallbackContext ctx)
    {
        m_mouseDeltaInputValue = ctx.ReadValue<Vector2>();

        if(Mathf.Abs(m_mouseDeltaInputValue.x) <= m_thesholdLevel)
        {
            m_mouseDeltaInputValue.x = 0;   
        }
        if (Mathf.Abs(m_mouseDeltaInputValue.y) <= m_thesholdLevel)
        {
            m_mouseDeltaInputValue.y = 0;
        }
    }


    public void CameraHeightInput(InputAction.CallbackContext ctx)
    {
        m_cameraHeightInputValue =  Mathf.RoundToInt( ctx.ReadValue<float>());
    }


    #endregion

}
