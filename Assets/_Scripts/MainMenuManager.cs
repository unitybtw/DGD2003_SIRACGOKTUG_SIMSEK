using System.IO;
using UnityEngine;
using UnityEngine.UI;
// Dikkat: SceneManagement yerine Addressables kütüphanelerini ekledik
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menü Butonları")]
    public Button continueButton; // Kayıttan devam etme butonu

    [Header("Yüklenecek Oyun Sahnesi (Addressables)")]
    [Tooltip("Giriş yapacağınız ana oyun sahnesinin Addressable referansı.")]
    public AssetReference gameSceneReference; // string yerine AssetReference kullanıyoruz!

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

        Debug.Log("Yeni Oyun Başlatılıyor (Addressables)...");
        LoadGameScene();
    }

    // 2. KAYITTAN DEVAM ET BUTONU FONKSİYONU
    public void ContinueGame()
    {
        Debug.Log("Kayıtlı Oyundan Devam Ediliyor (Addressables)...");
        LoadGameScene();
    }

    // Sahneleri asenkron yüklemek için ortak fonksiyon
    private void LoadGameScene()
    {
        if (gameSceneReference != null)
        {
            // Sahneyi arka planda yükler. Single modu eski sahneyi kapatıp bunu açar.
            Addressables.LoadSceneAsync(gameSceneReference, LoadSceneMode.Single).Completed += OnSceneLoaded;
        }
        else
        {
            Debug.LogError("Oyun sahnesi referansı (gameSceneReference) script üzerinde boş bırakılmış!");
        }
    }

    // Sahne yüklemesi bittiğinde tetiklenecek olan fonksiyon (İsteğe bağlı kontrol için)
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Oyun sahnesi başarıyla yüklendi.");
        }
        else
        {
            Debug.LogError("Sahne yüklenirken hata oluştu: " + handle.OperationException);
        }
    }

    // 3. OYUNDAN ÇIKIŞ BUTONU FONKSİYONU
    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkılıyor...");
        Application.Quit(); // Derlenmiş (Build) oyunda çalışır
    }
}