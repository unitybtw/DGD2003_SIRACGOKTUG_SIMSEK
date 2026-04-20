using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Objects")]
    [SerializeField] private GameObject ghostCam;
    [SerializeField] private GameObject cctvCam;
    [SerializeField] private GameObject studentCam;

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction ghostAction;
    private InputAction cctvAction;
    private InputAction studentAction;

    private void Awake()
    {
        if (inputActions == null)
        {
            return;
        }

        InputActionMap map = inputActions.FindActionMap("CameraControl");
        if (map == null)
        {
            return;
        }

        ghostAction = map.FindAction("GhostCam");
        cctvAction = map.FindAction("CCTV_Cam");
        studentAction = map.FindAction("Student_Cam");
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

    private void SwitchCamera(GameObject targetCam)
    {
        if (targetCam == null)
        {
            return;
        }

        if (ghostCam != null) ghostCam.SetActive(false);
        if (cctvCam != null) cctvCam.SetActive(false);
        if (studentCam != null) studentCam.SetActive(false);

        targetCam.SetActive(true);
        Debug.Log($"Switched to: {targetCam.name}");
    }
}
