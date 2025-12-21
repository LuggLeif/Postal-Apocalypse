using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public event Action OnChanged;

    [Header("Refs")]
    [SerializeField] private PlayerSkills skills;

    [Header("Runtime")]
    [SerializeField] private int maxHearts = 1;
    [SerializeField] private int currentHearts = 1;

    public int MaxHearts => maxHearts;
    public int CurrentHearts => currentHearts;

    private void Awake()
    {
        if (skills == null) skills = GetComponent<PlayerSkills>();
        RecomputeMaxFromSkills();
        currentHearts = Mathf.Clamp(currentHearts, 1, maxHearts);
    }

    private void OnEnable()
    {
        if (skills != null) skills.OnSkillLevelChanged += OnSkillChanged;
        RecomputeMaxFromSkills();
    }

    private void OnDisable()
    {
        if (skills != null) skills.OnSkillLevelChanged -= OnSkillChanged;
    }

    private void OnSkillChanged(SkillType skill, int newLevel)
    {
        if (skill != SkillType.Health) return;
        RecomputeMaxFromSkills();
    }

    private void RecomputeMaxFromSkills()
    {
        maxHearts = skills != null ? Mathf.Max(1, skills.MaxHearts) : Mathf.Max(1, maxHearts);
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        OnChanged?.Invoke();
    }

    public void TakeDamage(int amount = 1)
    {
        if (amount <= 0) return;
        currentHearts = Mathf.Max(0, currentHearts - amount);
        OnChanged?.Invoke();

        // Later: if currentHearts == 0 -> lose game
    }

    public void Heal(int amount = 1)
    {
        if (amount <= 0) return;
        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        OnChanged?.Invoke();
    }

    public void SetFullHealth()
    {
        currentHearts = maxHearts;
        OnChanged?.Invoke();
    }
}
