using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeliveryZone : MonoBehaviour
{
    [SerializeField] private DestinationDefinition destination;

    public DestinationDefinition Destination => destination;
    public int DestinationId => destination != null ? destination.id : -1;
    public string DestinationName => destination != null ? destination.displayName : "Unknown";

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }
}
