using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    
    [Header("Bakış Açıları")]
    public Vector3 backOffset = new Vector3(0, 2.5f, -4f); // Arkadan bakış
    public Vector3 frontOffset = new Vector3(0, 1.5f, 3f); // Önden bakış (Emote sırasında)
    
    public float smoothSpeed = 5f;
    private bool isEmoting = false;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Emote tuşlarını kontrol et (0=Idle, 1=Sad, 2=Happy, 3=Dancing)
        // Eğer 0'dan büyük bir tuşa basılırsa 'isEmoting' doğru olur.
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
            isEmoting = true;
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
            isEmoting = false;

        // 2. Hedef pozisyonu seç (isEmoting'e göre önden veya arkadan)
        Vector3 currentOffset = isEmoting ? frontOffset : backOffset;
        
        // Offset'i karakterin baktığı yöne göre hesapla
        Vector3 desiredPosition = target.position + (target.rotation * currentOffset);
        
        // 3. Yumuşak geçiş
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 4. Her zaman karaktere bak (Bakış yüksekliğini biraz yukarı alıyoruz)
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}