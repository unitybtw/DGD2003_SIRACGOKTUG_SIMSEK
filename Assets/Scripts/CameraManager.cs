using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineCamera ghostCam;
    [SerializeField] private CinemachineCamera cctvCam;
    [SerializeField] private CinemachineCamera studentCam;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction ghostAction;
    private InputAction cctvAction;
    private InputAction studentAction;

    private void Awake()
    {
        if (inputActions != null)
        {
            var map = inputActions.FindActionMap("CameraControl");
            if (map != null)
            {
                ghostAction = map.FindAction("GhostCam");
                cctvAction = map.FindAction("CCTV_Cam");
                studentAction = map.FindAction("Student_Cam");
            }
        }
    }

    private void OnEnable()
    {
        if (ghostAction != null) ghostAction.performed += OnGhostPressed;
        if (cctvAction != null) cctvAction.performed += OnCCTVPressed;
        if (studentAction != null) studentAction.performed += OnStudentPressed;

        ghostAction?.Enable();
        cctvAction?.Enable();
        studentAction?.Enable();
    }

    private void OnDisable()
    {
        if (ghostAction != null) ghostAction.performed -= OnGhostPressed;
        if (cctvAction != null) cctvAction.performed -= OnCCTVPressed;
        if (studentAction != null) studentAction.performed -= OnStudentPressed;

        ghostAction?.Disable();
        cctvAction?.Disable();
        studentAction?.Disable();
    }

    private void OnGhostPressed(InputAction.CallbackContext context) => SwitchCamera(ghostCam);
    private void OnCCTVPressed(InputAction.CallbackContext context) => SwitchCamera(cctvCam);
    private void OnStudentPressed(InputAction.CallbackContext context) => SwitchCamera(studentCam);

    private void SwitchCamera(CinemachineCamera targetCam)
    {
        if (targetCam == null) return;

        // Reset all to low priority
        if (ghostCam != null) ghostCam.Priority = 0;
        if (cctvCam != null) cctvCam.Priority = 0;
        if (studentCam != null) studentCam.Priority = 0;

        // Set target to high priority
        targetCam.Priority = 10;
        
        Debug.Log($"Switched to: {targetCam.name}");
    }
}
