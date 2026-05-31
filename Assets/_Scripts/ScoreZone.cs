using UnityEngine;
using TMPro;
using UnityEngine.AI; // NavMeshAgent (Yapay Zeka) kontrolü için eklendi

public class ScoreZone : MonoBehaviour
{
    [Header("Puan Sistemi")]
    public int toplamPuan = 0;
    public int ogrenciBasinaPuan = 10;
    public TextMeshProUGUI skorYazisiObjesi;

    [Header("Çekim Gücü (Mıknatıs Alanı)")]
    public float cekimAlaniYaricapi = 10f; // Öğrenci bu mesafeye girince hedefe kilitlenir

    void Start()
    {
        SkorTablosunuGuncelle();
    }

    void Update()
    {
        // Sahnede "Student" etiketli tüm öğrencileri bul
        GameObject[] ogrenciler = GameObject.FindGameObjectsWithTag("Student");

        foreach (GameObject ogrenci in ogrenciler)
        {
            // Öğrenci ile bu yeşil bölge arasındaki mesafeyi ölç
            float mesafe = Vector3.Distance(transform.position, ogrenci.transform.position);

            // Eğer öğrenci çekim alanına girdiyse (yeterince yaklaştıysa)
            if (mesafe <= cekimAlaniYaricapi)
            {
                // Öğrencinin yürüyüş motorunu (NavMeshAgent) bul
                NavMeshAgent ajan = ogrenci.GetComponentInParent<NavMeshAgent>();
                
                if (ajan != null)
                {
                    // Kendi devriyesini boşverip doğrudan yeşil alanın merkezine yürümesini emret
                    ajan.SetDestination(transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eğer yeşil bölgeye temas eden objenin etiketi "Student" ise
        if (other.CompareTag("Student"))
        {
            toplamPuan += ogrenciBasinaPuan;
            SkorTablosunuGuncelle();

            // DİKKAT: Sadece çarpan parçayı değil, hiyerarşideki en üst Ana Objeyi (Root) tamamen sil
            Destroy(other.transform.root.gameObject);
        }
    }

    void SkorTablosunuGuncelle()
    {
        if (skorYazisiObjesi != null)
        {
            skorYazisiObjesi.text = "Kurtarılan Öğrenciler: " + toplamPuan;
        }
    }
}