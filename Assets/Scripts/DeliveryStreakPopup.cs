using TMPro;
using UnityEngine;

public class DeliveryStreakPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float lifetime = 1.25f;

    [Header("Motion")]
    [SerializeField] private float floatUpPixels = 80f;

    [Header("Punch")]
    [SerializeField] private float startScale = 0.7f;
    [SerializeField] private float peakScale = 1.25f;
    [SerializeField] private float punchTime = 0.14f;

    private RectTransform _rt;
    private Vector2 _startPos;

    private void Awake()
    {
        _rt = transform as RectTransform;
        if (label == null) label = GetComponent<TMP_Text>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(string message)
    {
        if (label) label.text = message;

        _startPos = _rt.anchoredPosition;
        _rt.localScale = Vector3.one * startScale;
        if (canvasGroup) canvasGroup.alpha = 1f;

        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    private System.Collections.IEnumerator Animate()
    {
        float t = 0f;

        while (t < lifetime)
        {
            t += Time.unscaledDeltaTime;
            float n = Mathf.Clamp01(t / lifetime);

            // Float up
            _rt.anchoredPosition = _startPos + Vector2.up * (floatUpPixels * EaseOutCubic(n));

            // Punch scale
            float s;
            if (t < punchTime)
            {
                float pn = t / punchTime;
                s = Mathf.Lerp(startScale, peakScale, EaseOutBack(pn));
            }
            else
            {
                float sn = Mathf.InverseLerp(punchTime, lifetime, t);
                s = Mathf.Lerp(peakScale, 1f, EaseOutCubic(sn));
            }
            _rt.localScale = Vector3.one * s;

            // Fade near end
            float fadeStart = 0.55f;
            if (canvasGroup)
            {
                if (n <= fadeStart) canvasGroup.alpha = 1f;
                else canvasGroup.alpha = Mathf.Lerp(1f, 0f, (n - fadeStart) / (1f - fadeStart));
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private static float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3f);

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
