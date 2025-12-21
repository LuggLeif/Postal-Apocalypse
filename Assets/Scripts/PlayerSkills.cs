using System;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public event Action<SkillType, int> OnSkillLevelChanged;

    [Header("Starting Levels")]
    [SerializeField] private int capacityLevel = 0;
    [SerializeField] private int deliveryLevel = 0;
    [SerializeField] private int healthLevel = 0;
    [SerializeField] private int movementLevel = 0;
    [SerializeField] private int knowledgeLevel = 0;

    [Header("Capacity")]
    [SerializeField] private int baseCapacity = 15;
    [SerializeField] private int capacityPerLevel = 3;

    [Header("Delivery")]
    [SerializeField] private float baseDeliverySeconds = 2f;
    [SerializeField, Range(0.1f, 0.99f)] private float deliveryMultiplierPerLevel = 0.9f; // 10% faster each level

    [Header("Health")]
    [SerializeField] private int baseHearts = 1;

    [Header("Movement")]
    [Tooltip("Example: 0.10 = +10% walk & run per Movement level.")]
    [SerializeField, Range(0f, 1f)] private float movementPercentPerLevel = 0.10f;

    [Header("Knowledge")]
    [SerializeField] private int xpBonusPerLevel = 2;

    public int GetLevel(SkillType skill)
    {
        return skill switch
        {
            SkillType.Capacity => capacityLevel,
            SkillType.Delivery => deliveryLevel,
            SkillType.Health => healthLevel,
            SkillType.Movement => movementLevel,
            SkillType.Knowledge => knowledgeLevel,
            _ => 0
        };
    }

    public void LevelUp(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.Capacity: capacityLevel++; OnSkillLevelChanged?.Invoke(skill, capacityLevel); break;
            case SkillType.Delivery: deliveryLevel++; OnSkillLevelChanged?.Invoke(skill, deliveryLevel); break;
            case SkillType.Health: healthLevel++; OnSkillLevelChanged?.Invoke(skill, healthLevel); break;
            case SkillType.Movement: movementLevel++; OnSkillLevelChanged?.Invoke(skill, movementLevel); break;
            case SkillType.Knowledge: knowledgeLevel++; OnSkillLevelChanged?.Invoke(skill, knowledgeLevel); break;
        }
    }

    // Derived stats
    public int MaxCapacity => baseCapacity + (capacityLevel * capacityPerLevel);

    public float DeliverySecondsPerPackage =>
        baseDeliverySeconds * Mathf.Pow(deliveryMultiplierPerLevel, deliveryLevel);

    public int MaxHearts => baseHearts + healthLevel;

    public float MoveSpeedMultiplier => 1f + (movementLevel * movementPercentPerLevel);

    public int KnowledgeXpBonusPerDelivery => knowledgeLevel * xpBonusPerLevel;

    // UI helpers
    public string GetDisplayName(SkillType skill) => skill.ToString();

    public string GetDescription(SkillType skill)
    {
        int lvl = GetLevel(skill);

        return skill switch
        {
            SkillType.Capacity =>
                $"Max packages: {MaxCapacity}  (Base {baseCapacity}, +{capacityPerLevel}/lvl)\nCurrent level: {lvl}",

            SkillType.Delivery =>
                $"Seconds/package: {DeliverySecondsPerPackage:0.00}s  (Base {baseDeliverySeconds:0.00}s, -10%/lvl)\nCurrent level: {lvl}",

            SkillType.Health =>
                $"Hearts: {MaxHearts}\nCurrent level: {lvl}",

            SkillType.Movement =>
                $"Move multiplier: x{MoveSpeedMultiplier:0.00}  (+{movementPercentPerLevel * 100f:0}%/lvl)\nCurrent level: {lvl}",

            SkillType.Knowledge =>
                $"Bonus XP per delivered package: +{KnowledgeXpBonusPerDelivery}\n(also boosts streak deliveries)\nCurrent level: {lvl}",

            _ => ""
        };
    }
}
