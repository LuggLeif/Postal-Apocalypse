using TMPro;
using UnityEngine;

public class XPGainPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Motion")]
    [SerializeField] private float lifetime = 1.0f;
    [SerializeField] private float floatUpPixels = 60f;

    [Header("Punch")]
    [SerializeField] private float startScale = 0.85f;
    [SerializeField] private float punchScale = 1.15f;
    [SerializeField] private float punchTime = 0.12f;

    private RectTransform _rt;
    private Vector2 _startPos;

    private void Awake()
    {
        _rt = transform as RectTransform;
        if (label == null) label = GetComponent<TMP_Text>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(int xpAmount)
    {
        label.text = $"+{xpAmount} XP";

        _startPos = _rt.anchoredPosition;
        _rt.localScale = Vector3.one * startScale;
        canvasGroup.alpha = 1f;

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

            // Scale punch then settle
            float s;
            if (t < punchTime)
            {
                float pn = t / punchTime;
                s = Mathf.Lerp(startScale, punchScale, EaseOutBack(pn));
            }
            else
            {
                float sn = Mathf.InverseLerp(punchTime, lifetime, t);
                s = Mathf.Lerp(punchScale, 1f, EaseOutCubic(sn));
            }
            _rt.localScale = Vector3.one * s;

            // Fade near the end
            float fadeStart = 0.55f;
            if (n <= fadeStart) canvasGroup.alpha = 1f;
            else canvasGroup.alpha = Mathf.Lerp(1f, 0f, (n - fadeStart) / (1f - fadeStart));

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
