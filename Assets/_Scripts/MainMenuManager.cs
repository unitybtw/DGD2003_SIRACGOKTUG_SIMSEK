using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için şart!
using UnityEngine.UI; // Butonları kontrol etmek için şart!

public class MainMenuManager : MonoBehaviour
{
    [Header("Menü Butonları")]
    public Button continueButton; // Kayıttan devam etme butonu

    [Header("Yüklenecek Oyun Sahnesi")]
    [Tooltip("Giriş yapacağınız ana oyun sahnesinin tam adı.")]
    public string gameSceneName = "GameplayScene"; 

    private string saveFilePath;

    void Start()
    {
        // JSON dosyasının var olup olmadığını kontrol etmek için yolu alıyoruz
        saveFilePath = Path.Combine(Application.persistentDataPath, "GhostOfKulturSave.json");

        // Eğer daha önce alınmış bir kayıt (Save) dosyası yoksa "Devam Et" butonunu tıklanamaz yap
        if (continueButton != null)
        {
            if (File.Exists(saveFilePath))
            {
                continueButton.interactable = true; // Kayıt var, buton aktif
            }
            else
            {
                continueButton.interactable = false; // Kayıt yok, buton sönük/pasif
            }
        }
    }

    // 1. YENİ OYUN BUTONU FONKSİYONU
    public void NewGame()
    {
        // Eğer yeni oyun açılıyorsa eski ilerlemeyi sıfırlamak iyi bir pratik olabilir
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        Debug.Log("Yeni Oyun Başlatılıyor...");
        SceneManager.LoadScene(gameSceneName);
    }

    // 2. KAYITTAN DEVAM ET BUTONU FONKSİYONU
    public void ContinueGame()
    {
        Debug.Log("Kayıtlı Oyundan Devam Ediliyor...");
        // Oyun sahnesini yükler, sahne içindeki GameSaveManager uyanınca otomatik olarak JSON'ı okur
        SceneManager.LoadScene(gameSceneName);
    }

    // 3. OYUNDAN ÇIKIŞ BUTONU FONKSİYONU
    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkılıyor...");
        Application.Quit(); // Derlenmiş (Build) oyunda çalışır
    }
}