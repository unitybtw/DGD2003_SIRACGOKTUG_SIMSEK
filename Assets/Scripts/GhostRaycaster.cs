using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attached to the Main Camera. Handles raycasting and interaction input.
/// </summary>
[RequireComponent(typeof(Camera))]
public class GhostRaycaster : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Fallback: If the layer isn't set in the inspector, try to find 'Interactable'
        if (interactableLayer == 0)
        {
            interactableLayer = LayerMask.GetMask("Interactable");
        }
    }

    private void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        // Calculate the ray from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            // Check if the 'E' key was pressed this frame
            // Note: Requires the Input System package (com.unity.inputsystem)
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Attempt to get the GhostItem component from the hit object
                GhostItem ghostItem = hit.collider.GetComponent<GhostItem>();
                if (ghostItem != null)
                {
                    ghostItem.Interact();
                }
            }
        }
        
        // Debug visualization of the ray in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green);
    }
}
