using UnityEngine;

[RequireComponent(typeof(Package))]
public class PackagePickup : MonoBehaviour
{
    [Tooltip("Seconds after spawn before pickup is allowed (even if it hasn't collided yet).")]
    [SerializeField] private float minSecondsBeforePickup = 0.5f;

    private Package _pkg;
    private bool _canPickup;
    private float _spawnTime;

    private void Awake()
    {
        _pkg = GetComponent<Package>();
        _spawnTime = Time.time;
    }

    private void Update()
    {
        if (_canPickup) return;
        if (Time.time - _spawnTime >= minSecondsBeforePickup) _canPickup = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _canPickup = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canPickup) return;

        if (other.TryGetComponent<PlayerPackageInventory>(out var inv))
        {
            if (_pkg.Destination != null)
            {
                bool added = inv.TryAddPackage(_pkg.Destination);
                if (added)
                {
                    Destroy(gameObject);
                }
                // If not added: inventory fires OnPickupFailedFull and package stays.
            }
        }
    }
}
