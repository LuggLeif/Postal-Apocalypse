using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPackageInventory : MonoBehaviour
{
    public event Action OnChanged;

    private readonly Dictionary<int, int> _counts = new();

    public void AddPackage(DestinationDefinition dest)
    {
        if (dest == null) return;
        if (_counts.ContainsKey(dest.id)) _counts[dest.id]++;
        else _counts[dest.id] = 1;
        OnChanged?.Invoke();
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
