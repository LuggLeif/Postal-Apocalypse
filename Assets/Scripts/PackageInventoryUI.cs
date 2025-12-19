using System.Linq;
using UnityEngine;

public class PackageInventoryUI : MonoBehaviour
{
    [SerializeField] private PlayerPackageInventory inventory;
    [SerializeField] private DestinationDatabase destinationDatabase;

    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private InventoryRowUI rowPrefab;

    private void OnEnable()
    {
        if (inventory != null) inventory.OnChanged += Rebuild;
        Rebuild();
    }

    private void OnDisable()
    {
        if (inventory != null) inventory.OnChanged -= Rebuild;
    }

    private void Rebuild()
    {
        if (contentRoot == null || rowPrefab == null || inventory == null || destinationDatabase == null) return;

        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        var items = inventory.GetAllCounts()
            .OrderBy(kv => destinationDatabase.GetName(kv.Key));

        foreach (var kv in items)
        {
            var row = Instantiate(rowPrefab, contentRoot);
            row.Set(destinationDatabase.GetName(kv.Key), kv.Value);
        }
    }
}
