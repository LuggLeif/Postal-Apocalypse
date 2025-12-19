using System.Collections;
using UnityEngine;

public class PackageSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private DestinationDatabase destinationDatabase;
    [SerializeField] private GameObject packagePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnAnchors;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float extraHeight = 40f;

    [Header("Batch Rules")]
    [SerializeField] private float batchIntervalSeconds = 30f;
    [SerializeField] private int minPerBatch = 5;
    [SerializeField] private int maxPerBatch = 15;
    [SerializeField] private int maxPackagesInWorld = 50;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(batchIntervalSeconds);
            TrySpawnBatch();
        }
    }

    private void TrySpawnBatch()
    {
        if (destinationDatabase == null || packagePrefab == null || spawnAnchors == null || spawnAnchors.Length == 0)
            return;

        int existing = CountExistingPackages();
        if (existing >= maxPackagesInWorld) return;

        int available = maxPackagesInWorld - existing;
        int amount = UnityEngine.Random.Range(minPerBatch, maxPerBatch + 1);
        amount = Mathf.Min(amount, available);

        for (int i = 0; i < amount; i++)
            SpawnOne();
    }

    private int CountExistingPackages()
    {
        // Unity 6-safe API.
        var packages = Object.FindObjectsByType<Package>(FindObjectsSortMode.None);
        return packages != null ? packages.Length : 0;
    }

    private void SpawnOne()
    {
        var anchor = spawnAnchors[UnityEngine.Random.Range(0, spawnAnchors.Length)];
        if (anchor == null) return;

        Vector2 circle = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 pos = anchor.position + new Vector3(circle.x, extraHeight, circle.y);

        var go = Instantiate(packagePrefab, pos, UnityEngine.Random.rotation);
        if (go.TryGetComponent<Package>(out var pkg))
        {
            var dest = destinationDatabase.GetRandom();
            pkg.SetDestination(dest);
        }
    }
}
