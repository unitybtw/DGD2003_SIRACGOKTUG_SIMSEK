using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FreshmanAI : MonoBehaviour
{
    [Header("Navigation")]
    [Tooltip("Öğrencinin sırayla gideceği oryantasyon noktaları (örn: Giriş, Kayıt, Amfi)")]
    public Transform[] orientationCheckpoints;
    private int currentCheckpointIndex = 0;
    private NavMeshAgent agent;

    [Header("Curiosity vs Fear Logic")]
    [Range(0, 100)] public float fearMeter = 0f;
    public float fearThreshold = 100f;
    
    public enum AIState { HeadingToCheckpoint, Investigating, Panicking }
    public AIState currentState = AIState.HeadingToCheckpoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        if (orientationCheckpoints.Length > 0)
        {
            GoToNextCheckpoint();
        }
        else
        {
            Debug.LogWarning("Oryantasyon noktaları atanmadı! Lütfen Inspector'dan Checkpoint ekleyin.");
        }
    }

    void Update()
    {
        switch (currentState)
        {
            case AIState.HeadingToCheckpoint:
                CheckCheckpointProgress();
                break;
            case AIState.Investigating:
                // Araştırılan noktaya varıldıysa (veya çok yaklaşıldıysa)
                if (!agent.pathPending && agent.remainingDistance < 1f)
                {
                    ReturnToCheckpointRoutine();
                }
                break;
            case AIState.Panicking:
                // Panik durumunda sürekli koşma/kaçma mantığı çalışır (Game Over)
                break;
        }

        DecreaseFearOverTime();
    }

    private void CheckCheckpointProgress()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            currentCheckpointIndex++;
            GoToNextCheckpoint();
        }
    }

    private void GoToNextCheckpoint()
    {
        if (currentCheckpointIndex < orientationCheckpoints.Length)
        {
            agent.SetDestination(orientationCheckpoints[currentCheckpointIndex].position);
            agent.speed = 3.5f; // Normal yürüme hızı
        }
        else
        {
            Debug.Log("VICTORY! Öğrenci tüm oryantasyon noktalarına başarıyla ulaştı!");
        }
    }

    // Bu fonksiyonu Hayalet (oyuncu) bir obje fırlattığında veya kapı açtığında çağıracağız
    public void ReactToEvent(Vector3 eventPosition, bool isAggressive)
    {
        if (currentState == AIState.Panicking) return; // Zaten paniklediyse başka şeye tepki vermez

        if (isAggressive)
        {
            fearMeter += 35f; // Üstüne eşya atılması gibi agresif olaylar korkuyu hızla artırır
            Debug.Log("Öğrenci korktu! Güncel Korku Seviyesi: " + fearMeter);

            if (fearMeter >= fearThreshold)
            {
                TriggerPanic();
            }
        }
        else
        {
            // Merak (Curiosity) - Olay yerine gidip bakma
            fearMeter += 10f; // Bilinmezlik ufak bir gerginlik yaratır
            currentState = AIState.Investigating;
            agent.SetDestination(eventPosition);
            agent.speed = 2f; // Temkinli ve yavaş adımlarla araştırır
            Debug.Log("Öğrenci bir tıkırtı duydu ve araştırmaya gidiyor...");
        }
    }

    private void DecreaseFearOverTime()
    {
        // Zamanla sakinleşme mekaniği
        if (fearMeter > 0 && currentState != AIState.Panicking)
        {
            fearMeter -= Time.deltaTime * 3f; // Saniyede 3 birim sakinleşir
        }
    }

    private void ReturnToCheckpointRoutine()
    {
        Debug.Log("Görünürde bir şey yokmuş. Yola devam edeyim...");
        currentState = AIState.HeadingToCheckpoint;
        GoToNextCheckpoint();
    }

    private void TriggerPanic()
    {
        currentState = AIState.Panicking;
        agent.speed = 8f; // Hızla koşmaya başlar
        
        // Rastgele bir yere doğru kaçması için
        Vector3 randomDirection = Random.insideUnitSphere * 20f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 20f, 1);
        agent.SetDestination(hit.position);

        Debug.Log("GAME OVER! Öğrenci dayanamadı ve okulu terk ediyor!");
    }
}