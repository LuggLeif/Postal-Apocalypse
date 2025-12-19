using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Postal Apocalypse/Destination Database")]
public class DestinationDatabase : ScriptableObject
{
    public List<DestinationDefinition> destinations = new();

    private Dictionary<int, DestinationDefinition> _byId;

    private void OnEnable()
    {
        _byId = new Dictionary<int, DestinationDefinition>();
        foreach (var d in destinations)
        {
            if (d == null) continue;
            _byId[d.id] = d;
        }
    }

    public DestinationDefinition GetById(int id)
    {
        if (_byId == null) OnEnable();
        return _byId.TryGetValue(id, out var d) ? d : null;
    }

    public DestinationDefinition GetRandom()
    {
        if (destinations == null || destinations.Count == 0) return null;
        return destinations[UnityEngine.Random.Range(0, destinations.Count)];
    }

    public string GetName(int id)
    {
        var d = GetById(id);
        return d != null ? d.displayName : $"Destination {id}";
    }
}
