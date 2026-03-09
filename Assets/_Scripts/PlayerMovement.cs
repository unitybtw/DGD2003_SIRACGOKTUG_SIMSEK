using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public float gravity = -9.81f; // Karakterin havada süzülmesini engeller

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity; // Yerçekimi hızını tutmak için

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Klavyeden gelen duygu komutları (0, 1, 2, 3)
        if (Input.GetKeyDown(KeyCode.Alpha0)) animator.SetFloat("Emotion", 0f);
        if (Input.GetKeyDown(KeyCode.Alpha1)) animator.SetFloat("Emotion", 1f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) animator.SetFloat("Emotion", 2f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) animator.SetFloat("Emotion", 3f);

        // 2. Hareket girdilerini al (WASD veya Yön Tuşları)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Karakter yürümeye başladığında emot'u iptal edip bekleme/yürüme moduna dönsün
            animator.SetFloat("Emotion", 0f);

            // Baktığı yöne dön
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // İleri doğru git
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        // 3. Yerçekimi Uygula
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Yerdeyken sabit tut
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}