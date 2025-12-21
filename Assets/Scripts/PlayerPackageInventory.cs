using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackageInventory : MonoBehaviour
{
    public event Action OnChanged;

    /// <summary> Fired when a pickup attempt fails due to inventory full. </summary>
    public event Action OnPickupFailedFull;

    [Header("Refs")]
    [SerializeField] private PlayerSkills skills;

    private readonly Dictionary<int, int> _counts = new();
    private int _maxCapacityCached = 9999;

    private void Awake()
    {
        if (skills == null) skills = GetComponent<PlayerSkills>();
        RecomputeCapacity();
    }

    private void OnEnable()
    {
        if (skills != null) skills.OnSkillLevelChanged += OnSkillChanged;
        RecomputeCapacity();
    }

    private void OnDisable()
    {
        if (skills != null) skills.OnSkillLevelChanged -= OnSkillChanged;
    }

    private void OnSkillChanged(SkillType skill, int newLevel)
    {
        if (skill != SkillType.Capacity) return;
        RecomputeCapacity();
    }

    private void RecomputeCapacity()
    {
        _maxCapacityCached = skills != null ? skills.MaxCapacity : _maxCapacityCached;
        OnChanged?.Invoke();
    }

    public int GetMaxCapacity() => _maxCapacityCached;

    public int GetTotalCount()
    {
        int total = 0;
        foreach (var kv in _counts) total += kv.Value;
        return total;
    }

    public bool IsFull() => GetTotalCount() >= _maxCapacityCached;

    public bool TryAddPackage(DestinationDefinition dest)
    {
        if (dest == null) return false;

        if (IsFull())
        {
            OnPickupFailedFull?.Invoke();
            return false;
        }

        if (_counts.ContainsKey(dest.id)) _counts[dest.id]++;
        else _counts[dest.id] = 1;

        OnChanged?.Invoke();
        return true;
    }

    public int GetCount(int destinationId) => _counts.TryGetValue(destinationId, out var c) ? c : 0;

    public bool RemoveOne(int destinationId)
    {
        if (!_counts.TryGetValue(destinationId, out var c) || c <= 0) return false;

        c--;
        if (c <= 0) _counts.Remove(destinationId);
        else _counts[destinationId] = c;

        OnChanged?.Invoke();
        return true;
    }

    public IReadOnlyDictionary<int, int> GetAllCounts() => _counts;
}
