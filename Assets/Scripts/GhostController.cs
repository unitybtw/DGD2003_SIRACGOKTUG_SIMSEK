using UnityEngine;

[RequireComponent(typeof(Camera))]
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

    void Start()
    {
        // Fare imlecini kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
        HandleTelekinesis();
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

        // E veya Space ile yukarı, Q veya Sol Shift ile aşağı uçma (Yerçekimsiz hayalet hareketi)
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) moveY = 1f;
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftShift)) moveY = -1f;

        Vector3 moveDirection = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleTelekinesis()
    {
        // Sol tık: Obje tut veya bırak
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else
            {
                ReleaseObject();
            }
        }

        // Sağ tık: Tutulan objeyi fırlat
        if (Input.GetMouseButtonDown(1) && heldObject != null)
        {
            ThrowObject();
        }

        // Tutulan obje varsa pozisyonunu kameranın önüne (HoldPosition) güncelle
        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }

    private void TryPickupObject()
    {
        RaycastHit hit;
        // Kameranın tam ortasından ileriye doğru bir ışın gönder
        if (Physics.Raycast(transform.position, transform.forward, out hit, pickupRange))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f; // Unity 6'da drag yerine linearDamping kullanılıyor
                heldObject.angularDamping = 10f;
                heldObject.constraints = RigidbodyConstraints.FreezeRotation; // Tutarken dönmesini engelle
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
        // İleriye doğru fırlatma kuvveti uygula
        objToThrow.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    }

    private void UpdateHeldObjectPosition()
    {
        if (holdPosition != null)
        {
            // Unity 6 fizik motoruna uygun şekilde objeyi hedefe doğru yumuşakça çek
            Vector3 moveDirection = holdPosition.position - heldObject.position;
            heldObject.linearVelocity = moveDirection * 10f; 
        }
    }
}