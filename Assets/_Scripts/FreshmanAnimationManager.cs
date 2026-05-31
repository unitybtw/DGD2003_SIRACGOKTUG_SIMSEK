using UnityEngine;
using UnityEngine.InputSystem;

public class FreshmanAnimationManager : MonoBehaviour
{
    private Animator animator;

    // Duygu durumlar??n?? matematiksel olarak ifade edelim:
    // 0 = Idle, 1 = Sad, 2 = Happy, 3 = Dancing

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // MacBook klavyendeki 0, 1, 2 ve 3 tu??lar??yla test edebilirsin
        if (Keyboard.current.digit0Key.wasPressedThisFrame) SetEmotion(0f);
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SetEmotion(1f);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SetEmotion(2f);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SetEmotion(3f);
    }

    public void SetEmotion(float value)
    {
        // Blend Tree parametresini g??nceller
        if (animator != null)
        {
            animator.SetFloat("Emotion", value);
        }
    }
}