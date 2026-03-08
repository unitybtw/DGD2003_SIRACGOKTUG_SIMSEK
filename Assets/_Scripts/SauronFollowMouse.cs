using UnityEngine;

// Sınıf ismi (SauronFollowMouse) dosya ismiyle birebir aynı olmalı!
public class SauronFollowMouse : MonoBehaviour 
{
    [Header("Göz Ayarları")]
    public float rotationSpeed = 10f; // Dönüşün yumuşaklığı
    public float zDepth = 15f;       // Fare imlecinin derinliği

    void Update()
    {
        // 1. Fare pozisyonunu ekrandan al
        Vector3 mousePos = Input.mousePosition;
        
        // 2. Kameraya göre bir derinlik (Z) ata
        mousePos.z = zDepth; 

        // 3. Ekran koordinatını dünya koordinatına çevir
        Vector3 targetWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 4. Hedef yöne bakacak rotasyonu hesapla
        Vector3 direction = targetWorldPos - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // 5. Yumuşak bir şekilde o yöne dön
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}