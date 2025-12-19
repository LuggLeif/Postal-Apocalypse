using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Slider progressSlider;

    public void Show(string destinationName)
    {
        if (root) root.SetActive(true);
        if (titleText) titleText.text = $"Delivering to: {destinationName}";
        SetProgress01(0f);
    }

    public void Hide()
    {
        if (root) root.SetActive(false);
        SetProgress01(0f);
    }

    public void SetProgress01(float t)
    {
        if (progressSlider) progressSlider.value = Mathf.Clamp01(t);
    }
}
