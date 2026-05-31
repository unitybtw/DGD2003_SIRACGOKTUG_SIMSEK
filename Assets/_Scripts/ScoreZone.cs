using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets; // Addressables kütüphanesini ekledik!

public class ScoreZone : MonoBehaviour
{
    [Header("Puan Sistemi")]
    public int toplamPuan = 0;
    public int ogrenciBasinaPuan = 10;
    public TextMeshProUGUI skorYazisiObjesi;

    [Header("Çekim Gücü (Mıknatıs Alanı)")]
    public float cekimAlaniYaricapi = 10f; 

    void Start()
    {
        SkorTablosunuGuncelle();
    }

    void Update()
    {
        GameObject[] ogrenciler = GameObject.FindGameObjectsWithTag("Student");

        foreach (GameObject ogrenci in ogrenciler)
        {
            float mesafe = Vector3.Distance(transform.position, ogrenci.transform.position);

            if (mesafe <= cekimAlaniYaricapi)
            {
                FreshmanAI ai = ogrenci.GetComponentInParent<FreshmanAI>();
                if (ai != null)
                {
                    ai.ForcePullToZone(transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Çarpan objenin kendisinde veya üst nesnelerinde FreshmanAI kodu var mı?
        FreshmanAI ai = other.GetComponentInParent<FreshmanAI>();

        // Eğer kod bulunduysa, bu kesinlikle bizim öğrencimizdir!
        if (ai != null)
        {
            toplamPuan += ogrenciBasinaPuan;
            SkorTablosunuGuncelle();

            // ADDRESSABLES OPTİMİZASYONU: 
            // Normal Destroy yerine Addressables kullanarak objeyi hafızadan (RAM) tamamen temizliyoruz.
            Addressables.ReleaseInstance(ai.gameObject);
        }
    }

    void SkorTablosunuGuncelle()
    {
        if (skorYazisiObjesi != null)
        {
            // SADECE SAYIYI YAZDIRAN KUSURSUZ KOD BURASI
            skorYazisiObjesi.text = toplamPuan.ToString();
        }
    }
}