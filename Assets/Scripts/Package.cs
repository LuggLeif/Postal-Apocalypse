using UnityEngine;

public class Package : MonoBehaviour
{
    [SerializeField] private DestinationDefinition destination;

    public DestinationDefinition Destination => destination;
    public int DestinationId => destination != null ? destination.id : -1;
    public string DestinationName => destination != null ? destination.displayName : "Unknown";

    public void SetDestination(DestinationDefinition def) => destination = def;
}
