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
        // Any collision (typically ground) enables pickup.
        _canPickup = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canPickup) return;

        if (other.TryGetComponent<PlayerPackageInventory>(out var inv))
        {
            if (_pkg.Destination != null)
            {
                inv.AddPackage(_pkg.Destination);
                Destroy(gameObject);
            }
        }
    }
}
