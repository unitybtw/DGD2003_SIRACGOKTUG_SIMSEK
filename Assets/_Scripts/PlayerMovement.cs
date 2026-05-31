using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarlar??")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 700f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    // Hangi duyguya "yumu??ak??a" ge??ece??imizi akl??nda tutmas?? i??in yeni bir de??i??ken
    private float targetEmotion = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector3 direction = GetMovementDirection();

        if (Keyboard.current != null)
        {
            // 1. Klavyeden gelen komutlar art??k direkt Animator'?? de??il, hedefimizi (targetEmotion) de??i??tiriyor
            if (Keyboard.current.digit0Key.wasPressedThisFrame) targetEmotion = 0f;
            if (Keyboard.current.digit1Key.wasPressedThisFrame) targetEmotion = 1f;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) targetEmotion = 2f;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) targetEmotion = 3f;
        }

        if (direction.magnitude >= 0.1f)
        {
            // Y??r??meye ba??lad??????nda hedefi otomatik olarak Idle/Y??r??y???? (0) yap
            targetEmotion = 0f;

            // Bakt?????? y??ne d??n ve ilerle
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }

        // 3. Yer??ekimi
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 4. S??H??RL?? DOKUNU??: Unity'nin kendi yumu??atma (Damping) sistemi!
        // Bu kod, anl??k atlamak yerine 0.15 saniye i??inde ak??c?? bir ??ekilde di??er animasyona s??z??l??r.
        if (animator != null)
        {
            animator.SetFloat("Emotion", targetEmotion, 0.15f, Time.deltaTime);
        }
    }

    private Vector3 GetMovementDirection()
    {
        if (Keyboard.current == null)
        {
            return Vector3.zero;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            horizontal -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            horizontal += 1f;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            vertical -= 1f;
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            vertical += 1f;
        }

        return new Vector3(horizontal, 0f, vertical).normalized;
    }
}