using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Öğrencinin bu objenin çıkardığı sesi duyabileceği maksimum mesafe")]
    public float hearingRadius = 15f;
    [Tooltip("Bu şiddetin üzerinde bir hızla çarparsa öğrenciyi korkutur, altındaysa meraklandırır.")]
    public float aggressiveForceThreshold = 5f; 

    [Header("Audio")]
    public AudioClip impactSound;
    private AudioSource audioSource;

    private Rigidbody rb;
    private bool isHeldByGhost = false;
    private float lastImpactTime = 0f;
    private float impactCooldown = 1f; // Obje yerde sekerken sürekli event tetiklenmesini engeller

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
        // Sesin 3D olarak (uzaklığa göre azalan) ayarlanması
        if (audioSource != null)
        {
            audioSource.spatialBlend = 1f; 
            audioSource.maxDistance = hearingRadius;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // GhostController'da objeyi tutarken useGravity = false yapıyorduk.
        // Bunu kullanarak objenin şu an havada tutulup tutulmadığını anlıyoruz.
        if (rb != null)
        {
            isHeldByGhost = !rb.useGravity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Eğer hayalet objeyi duvarlara sürterek taşıyorsa ses/event çıkarma
        if (isHeldByGhost) return;

        // Cooldown kontrolü (aynı obje saniyede 10 kere ses çıkarmasın)
        if (Time.time - lastImpactTime < impactCooldown) return;

        // Çarpışma şiddetini hesapla
        float impactForce = collision.relativeVelocity.magnitude;

        // Yerdeki çok hafif kaymaları ve sürtünmeleri yoksay
        if (impactForce < 1f) return;

        lastImpactTime = Time.time;

        // Ses efektini çal (şiddete göre ses seviyesini ayarla)
        if (audioSource != null && impactSound != null)
        {
            audioSource.volume = Mathf.Clamp01(impactForce / 10f);
            audioSource.PlayOneShot(impactSound);
        }

        // Çevredeki Öğrenciyi (Freshman) uyar
        NotifyFreshman(impactForce);
    }

    private void NotifyFreshman(float impactForce)
    {
        // Belirlediğimiz yarıçap içindeki tüm fiziksel alanları tarar
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, hearingRadius);
        
        foreach (Collider hitCollider in hitColliders)
        {
            // KRİTİK DÜZELTME: Sadece FreshmanAI bileşeni olan objeleri bulmaya çalışıyoruz.
            // Diğer harita objelerine çarptığında döngüyü kırmasını (break) engelledik.
            FreshmanAI npc = hitCollider.GetComponent<FreshmanAI>();
            if (npc != null)
            {
                // Çarpışma şiddeti eşiği geçtiyse "Agresif" (Korkutucu) sayılır
                bool isAggressive = impactForce >= aggressiveForceThreshold;
                
                // NPC'nin scriptine olayın konumunu ve agresif olup olmadığını gönderiyoruz
                npc.ReactToEvent(transform.position, isAggressive);
            }
        }
    }

    // Unity Editor'de objeyi seçtiğinde sesin/duyulma alanının sınırlarını sarı bir küre olarak çizer
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}