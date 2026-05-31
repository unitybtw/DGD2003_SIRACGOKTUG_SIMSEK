using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attached to interactable objects in the scene.
/// </summary>
public class GhostItem : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Event triggered when the GhostRaycaster interacts with this object.")]
    public UnityEvent OnGhostInteract;

    /// <summary>
    /// Called by the GhostRaycaster script.
    /// </summary>
    public void Interact()
    {
        Debug.Log($"Interacting with {gameObject.name}");
        OnGhostInteract?.Invoke();
    }
}
