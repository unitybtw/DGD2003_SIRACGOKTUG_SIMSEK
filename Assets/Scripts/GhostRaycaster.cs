using UnityEngine;

public class GhostRaycaster : MonoBehaviour
{
    [Header("Işının Ulaşabileceği Maksimum Mesafe")]
    public float isinMenzili = 20f;

    void Update()
    {
        // Farenin sol tıkına basıldığında lazeri ateşle
        if (Input.GetMouseButtonDown(0))
        {
            AtesEt();
        }
    }

    void AtesEt()
    {
        // 1. TEST: Tıklamayı algıladı mı?
        Debug.Log("Tetiğe basıldı, lazer yola çıktı!");

        // Ekranın tam ortasından ileri doğru bir çizgi (Ray) oluştur
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 2. TEST: Işın bir şeye çarptı mı? 
        if (Physics.Raycast(ray, out hit, isinMenzili))
        {
            Debug.Log("Işın Şuna Çarptı: " + hit.collider.name);

            GhostTarget hedef = hit.collider.GetComponent<GhostTarget>();
            
            if (hedef != null)
            {
                hedef.onHit.Invoke();
            }
        }
    }
}