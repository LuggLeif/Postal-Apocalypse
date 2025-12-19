using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceUI : MonoBehaviour
{
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        if (experience != null) experience.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (experience != null) experience.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (experience == null) return;
        if (xpSlider) xpSlider.value = experience.GetNormalized();
        if (levelText) levelText.text = $"Lv {experience.Level}";
    }
}
