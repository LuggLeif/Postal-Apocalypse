using UnityEngine;

[CreateAssetMenu(menuName = "Postal Apocalypse/Destination Definition")]
public class DestinationDefinition : ScriptableObject
{
    [Min(0)] public int id;
    public string displayName;
}
