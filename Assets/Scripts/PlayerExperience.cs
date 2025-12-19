using System;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public event Action OnChanged;
    public event Action<int> OnXpGained;

    [SerializeField] private int level = 1;
    [SerializeField] private int currentXp = 0;
    [SerializeField] private int xpToNext = 100;

    public int Level => level;
    public int CurrentXp => currentXp;
    public int XpToNext => xpToNext;

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        OnXpGained?.Invoke(amount);

        currentXp += amount;

        while (currentXp >= xpToNext)
        {
            currentXp -= xpToNext;
            level++;
            xpToNext = ComputeXpToNext(level);
        }

        OnChanged?.Invoke();
    }

    public float GetNormalized() => xpToNext <= 0 ? 0f : (float)currentXp / xpToNext;

    private int ComputeXpToNext(int lvl) => 100 + (lvl - 1) * 25;
}
