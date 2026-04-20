using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public float gravity = -9.81f; 

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; 
    
    // Hangi duyguya "yumuşakça" geçeceğimizi aklında tutması için yeni bir değişken
    private float targetEmotion = 0f; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Klavyeden gelen komutlar artık direkt Animator'ı değil, hedefimizi (targetEmotion) değiştiriyor
        if (Input.GetKeyDown(KeyCode.Alpha0)) targetEmotion = 0f;
        if (Input.GetKeyDown(KeyCode.Alpha1)) targetEmotion = 1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) targetEmotion = 2f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) targetEmotion = 3f;

        // 2. Hareket girdilerini al (WASD)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Yürümeye başladığında hedefi otomatik olarak Idle/Yürüyüş (0) yap
            targetEmotion = 0f;

            // Baktığı yöne dön ve ilerle
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        // 3. Yerçekimi
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 4. SİHİRLİ DOKUNUŞ: Unity'nin kendi yumuşatma (Damping) sistemi!
        // Bu kod, anlık atlamak yerine 0.15 saniye içinde akıcı bir şekilde diğer animasyona süzülür.
        animator.SetFloat("Emotion", targetEmotion, 0.15f, Time.deltaTime);
    }
}