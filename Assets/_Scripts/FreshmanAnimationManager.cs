using UnityEngine;

public class FreshmanAnimationManager : MonoBehaviour
{
    private Animator animator;

    // Duygu durumlarını matematiksel olarak ifade edelim:
    // 0 = Idle, 1 = Sad, 2 = Happy, 3 = Dancing

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // MacBook klavyendeki 0, 1, 2 ve 3 tuşlarıyla test edebilirsin
        if (Input.GetKeyDown(KeyCode.Alpha0)) SetEmotion(0f);
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetEmotion(1f);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetEmotion(2f);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetEmotion(3f);
    }

    public void SetEmotion(float value)
    {
        // Blend Tree parametresini günceller
        if (animator != null)
        {
            animator.SetFloat("Emotion", value);
        }
    }
}