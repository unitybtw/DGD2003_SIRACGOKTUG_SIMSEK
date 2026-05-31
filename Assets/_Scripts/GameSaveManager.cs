using System.IO; // Dosya okuma/yazma işlemleri için şart!
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    // 1. KAYDEDİLECEK VERİ YAPISI
    // [System.Serializable] koymazsak Unity bu verileri JSON'a dönüştüremez!
    [System.Serializable]
    public class GameData
    {
        public int toplamSkor = 0;
        public float hayaletKorkutmaGucu = 10f;
        public int korkutulanOgrenciSayisi = 0;
        public string mevcutOda = "Giriş Atrium";
    }

    [Header("Mevcut Oyun Verileri")]
    public GameData aktifOyunVerisi = new GameData();

    private string dosyaYolu;

    void Awake()
    {
        // Her bilgisayarda (Mac/Windows) güvenle çalışacak gizli kayıt klasörü yolu
        dosyaYolu = Path.Combine(Application.persistentDataPath, "GhostOfKulturSave.json");
        
        // Oyun başlarken eski kaydı otomatik yükle
        KayıtYukle();
    }

    void Update()
    {
        // Test etmek için klavyeden S tuşuna basınca kaydetsin
        if (Input.GetKeyDown(KeyCode.S))
        {
            KayıtYap();
        }

        // Test etmek için klavyeden L tuşuna basınca yüklesin
        if (Input.GetKeyDown(KeyCode.L))
        {
            KayıtYukle();
        }
    }

    // 2. DISKE KAYDETME FONKSİYONU (SAVE)
    public void KayıtYap()
    {
        try
        {
            // Veri sınıfımızı okunaklı bir string (JSON) haline getiriyoruz
            string jsonVerisi = JsonUtility.ToJson(aktifOyunVerisi, true);
            
            // Bu string'i bilgisayardaki dosyaya yazıyoruz
            File.WriteAllText(dosyaYolu, jsonVerisi);
            
            Debug.Log("Oyun Başarıyla Kaydedildi! Yol: " + dosyaYolu);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Oyun kaydedilirken hata oluştu: " + e.Message);
        }
    }

    // 3. DISKTEN YÜKLEME FONKSİYONU (LOAD)
    public void KayıtYukle()
    {
        // Eğer daha önce kaydedilmiş bir dosya varsa oku
        if (File.Exists(dosyaYolu))
        {
            try
            {
                // Dosyadaki text'i çekiyoruz
                string jsonVerisi = File.ReadAllText(dosyaYolu);
                
                // Text'i tekrar Unity'nin anlayacağı değişkenlere çeviriyoruz
                aktifOyunVerisi = JsonUtility.FromJson<GameData>(jsonVerisi);
                
                Debug.Log("Eski Kayıt Başarıyla Yüklendi!");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Oyun yüklenirken hata oluştu: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Daha önce alınmış bir kayıt dosyası bulunamadı. Sıfır veriyle başlanıyor.");
            aktifOyunVerisi = new GameData(); // Sıfır veri oluştur
        }
    }

    // 4. KAYDI SIFIRLAMA FONKSİYONU (DELETE)
    public void KaydıSil()
    {
        if (File.Exists(dosyaYolu))
        {
            File.Delete(dosyaYolu);
            aktifOyunVerisi = new GameData();
            Debug.Log("Kayıt dosyası başarıyla silindi ve veriler sıfırlandı.");
        }
    }
}