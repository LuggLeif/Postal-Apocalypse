using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillChoiceUI : MonoBehaviour
{
    [System.Serializable]
    public class ChoiceSlot
    {
        public Button button;
        public TMP_Text titleText;
        public TMP_Text descriptionText;
    }

    [Header("Slots (3)")]
    [SerializeField] private ChoiceSlot[] slots = new ChoiceSlot[3];

    [Header("Root")]
    [SerializeField] private GameObject root;

    private System.Action<SkillType> _onChosen;

    public void Show(PlayerSkills skills, SkillType[] choices, System.Action<SkillType> onChosen)
    {
        _onChosen = onChosen;

        if (root != null) root.SetActive(true);
        else gameObject.SetActive(true);

        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (slot == null || slot.button == null) continue;

            SkillType choice = choices[i];

            if (slot.titleText != null)
                slot.titleText.text = skills.GetDisplayName(choice);

            if (slot.descriptionText != null)
                slot.descriptionText.text = skills.GetDescription(choice);

            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => _onChosen?.Invoke(choice));
            slot.button.interactable = true;
        }
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
        else gameObject.SetActive(false);
    }
}
