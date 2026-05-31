using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Ayarları")]
    [Tooltip("Sahnede doğmasını istediğin Öğrenci Prefab'ı.")]
    public GameObject studentPrefab;

    [Tooltip("Toplamda kaç adet öğrenci doğmasını istiyorsun?")]
    public int totalNPCsToSpawn = 5;

    [Header("Doğma Noktaları")]
    [Tooltip("Öğrenci karakterlerin rastgele seçip doğabileceği konumlar (Sınıflar, Koridorlar vb.).")]
    public Transform[] spawnPoints;

    [Header("Devriye Rotaları (İsteğe Bağlı)")]
    [Tooltip("Doğan öğrencilerin otomatik devriye atması için Freshman_Waypoints veya Freshman_TestRoute objesini buraya koyabilirsin.")]
    public Transform waypointsRoot;

    void Start()
    {
        if (studentPrefab == null)
        {
            Debug.LogError("NPCSpawner: Lütfen bir Student Prefab'ı atayın!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("NPCSpawner: Hiç doğma noktası (Spawn Point) belirlenmedi!");
            return;
        }

        SpawnNPCs();
    }

    void SpawnNPCs()
    {
        // Belirlediğin sayı kadar öğrenciyi döngüyle yaratıyoruz
        for (int i = 0; i < totalNPCsToSpawn; i++)
        {
            // Doğma noktaları listesinden rastgele bir indeks seçiyoruz
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform selectedSpawnPoint = spawnPoints[randomIndex];

            // NPC'yi seçilen rastgele noktada ve o noktanın bakış yönünde yarat (Spawn)
            GameObject spawnedNPC = Instantiate(studentPrefab, selectedSpawnPoint.position, selectedSpawnPoint.rotation);
            
            // Sahne hiyerarşisi karmaşık olmasın diye doğan çocukları bu spawner'ın altına toplar
            spawnedNPC.transform.parent = this.transform;

            // Spawner üzerinden rota aktarma kontrolü
            if (waypointsRoot != null)
            {
                FreshmanAI aiScript = spawnedNPC.GetComponent<FreshmanAI>();
                if (aiScript != null && (aiScript.waypoints == null || aiScript.waypoints.Length == 0))
                {
                    // WaypointsRoot altındaki tüm çocukları (hedef noktaları) toplar ve AI'a teslim eder
                    List<Transform> collectedWaypoints = new List<Transform>();
                    foreach (Transform child in waypointsRoot)
                    {
                        collectedWaypoints.Add(child);
                    }
                    aiScript.waypoints = collectedWaypoints.ToArray();
                }
            }
        }

        Debug.Log($"{totalNPCsToSpawn} adet öğrenci haritaya rastgele başarıyla dağıtıldı!");
    }
}