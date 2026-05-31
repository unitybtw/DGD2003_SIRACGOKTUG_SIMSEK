using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Visuals")]
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float hoverDuration = 0.12f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(1f, 0.92f, 0.6f, 1f);

    private Button button;
    private Image targetImage;
    private Vector3 baseScale;
    private Coroutine scaleRoutine;
    private Coroutine colorRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        targetImage = GetComponent<Image>();
        baseScale = transform.localScale;

        if (targetImage != null)
        {
            normalColor = targetImage.color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Animate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Animate(false);
    }

    private void Animate(bool hovered)
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }

        if (colorRoutine != null)
        {
            StopCoroutine(colorRoutine);
        }

        Vector3 targetScale = hovered ? baseScale * hoverScale : baseScale;
        Color targetColor = hovered ? hoverColor : normalColor;

        scaleRoutine = StartCoroutine(ScaleRoutine(targetScale));

        if (targetImage != null)
        {
            colorRoutine = StartCoroutine(ColorRoutine(targetColor));
        }

        if (button != null)
        {
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = normalColor;
            colors.highlightedColor = hoverColor;
            colors.selectedColor = hoverColor;
            colors.pressedColor = new Color(targetColor.r * 0.85f, targetColor.g * 0.85f, targetColor.b * 0.85f, 1f);
            button.colors = colors;
        }
    }

    private IEnumerator ScaleRoutine(Vector3 targetScale)
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, hoverDuration);
            transform.localScale = Vector3.Lerp(start, targetScale, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator ColorRoutine(Color targetColor)
    {
        Color start = targetImage.color;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, hoverDuration);
            targetImage.color = Color.Lerp(start, targetColor, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        targetImage.color = targetColor;
    }
}
