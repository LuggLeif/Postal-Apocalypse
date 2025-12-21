using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCapacityUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerPackageInventory inventory;

    [Header("UI")]
    [SerializeField] private Slider radialSlider;               // value 0..1
    [SerializeField] private RectTransform backpackIcon;        // image on top of slider

    [Header("Shake")]
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakePixels = 8f;
    [SerializeField] private float shakeFrequency = 28f;        // how “vibey” it is

    private Coroutine _shakeRoutine;
    private Vector2 _backpackBasePos;

    private void Awake()
    {
        if (inventory == null) inventory = FindFirstObjectByType<PlayerPackageInventory>();
        if (backpackIcon != null) _backpackBasePos = backpackIcon.anchoredPosition;
    }

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.OnChanged += Refresh;
            inventory.OnPickupFailedFull += ShakeBackpack;
        }
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.OnChanged -= Refresh;
            inventory.OnPickupFailedFull -= ShakeBackpack;
        }
    }

    private void Refresh()
    {
        if (inventory == null || radialSlider == null) return;

        int max = Mathf.Max(1, inventory.GetMaxCapacity());
        int cur = Mathf.Clamp(inventory.GetTotalCount(), 0, max);

        float normalized = (float)cur / max;
        radialSlider.minValue = 0f;
        radialSlider.maxValue = 1f;
        radialSlider.value = normalized;
    }

    private void ShakeBackpack()
    {
        if (backpackIcon == null) return;

        if (_shakeRoutine != null)
            StopCoroutine(_shakeRoutine);

        _shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float t = 0f;
        Vector2 basePos = _backpackBasePos;

        while (t < shakeDuration)
        {
            t += Time.unscaledDeltaTime;

            float phase = t * shakeFrequency;
            float strength = Mathf.Lerp(shakePixels, 0f, t / shakeDuration);

            float x = Mathf.Sin(phase) * strength;
            float y = Mathf.Cos(phase * 0.9f) * (strength * 0.35f);

            backpackIcon.anchoredPosition = basePos + new Vector2(x, y);
            yield return null;
        }

        backpackIcon.anchoredPosition = basePos;
        _shakeRoutine = null;
    }
}
