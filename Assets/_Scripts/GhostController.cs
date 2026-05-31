using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))] // Rigidbody'i zorunlu kıldık
public class GhostController : MonoBehaviour
{
    [Header("Omnipresence (Movement)")]
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Telekinesis")]
    public float pickupRange = 10f;
    public float throwForce = 15f;
    public Transform holdPosition;
    
    private Rigidbody heldObject;
    private Rigidbody ghostRb; // Hayaletin kendi fiziksel bedeni

    void Start()
    {
        // Fare imlecini kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Kendi Rigidbody'mizi alıyoruz
        ghostRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleLook();
        HandleTelekinesis();
    }

    // Fiziksel hareketleri FixedUpdate içine almak duvar çarpmalarını kusursuz yapar
    void FixedUpdate() 
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;

        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) moveY = 1f;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftShift)) moveY = -1f;

        // Hareket yönünü belirle
        Vector3 moveDirection = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;
        
        // Işınlanmak yerine fiziksel ivme (Velocity) uyguluyoruz
        ghostRb.linearVelocity = moveDirection * moveSpeed; 
    }

    private void HandleTelekinesis()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null) TryPickupObject();
            else ReleaseObject();
        }

        if (Input.GetMouseButtonDown(1) && heldObject != null)
        {
            ThrowObject();
        }

        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }

    private void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f; 
                heldObject.angularDamping = 10f;
                heldObject.constraints = RigidbodyConstraints.FreezeRotation; 
            }
        }
    }

    private void ReleaseObject()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 0f;
        heldObject.angularDamping = 0.05f;
        heldObject.constraints = RigidbodyConstraints.None;
        heldObject = null;
    }

    private void ThrowObject()
    {
        Rigidbody objToThrow = heldObject;
        ReleaseObject();
        objToThrow.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    private void UpdateHeldObjectPosition()
    {
        if (holdPosition != null)
        {
            Vector3 moveDirection = holdPosition.position - heldObject.position;
            heldObject.linearVelocity = moveDirection * 10f; 
        }
    }
}