using UnityEngine;

public class DeliveryStreakAnnouncer : MonoBehaviour
{
    [SerializeField] private DeliveryInteractor interactor;

    [Header("UI")]
    [SerializeField] private DeliveryStreakPopup popupPrefab;
    [SerializeField] private RectTransform parent;      // e.g. HUD/Popups
    [SerializeField] private RectTransform spawnAnchor; // e.g. near delivery UI
    [SerializeField] private Vector2 randomOffset = new Vector2(30f, 10f);

    private void OnEnable()
    {
        if (interactor != null) interactor.OnDeliveryStreakEnded += OnStreakEnded;
    }

    private void OnDisable()
    {
        if (interactor != null) interactor.OnDeliveryStreakEnded -= OnStreakEnded;
    }

    private void OnStreakEnded(int streakCount)
    {
        string name = GetLiveryName(streakCount);
        if (string.IsNullOrEmpty(name)) return;

        if (popupPrefab == null || parent == null || spawnAnchor == null) return;

        var popup = Instantiate(popupPrefab, parent);
        var rt = popup.transform as RectTransform;

        Vector2 jitter = new Vector2(
            UnityEngine.Random.Range(-randomOffset.x, randomOffset.x),
            UnityEngine.Random.Range(-randomOffset.y, randomOffset.y)
        );

        rt.anchoredPosition = spawnAnchor.anchoredPosition + jitter;

        popup.Play($"{name}!");
    }

    private string GetLiveryName(int streakCount)
    {
        return streakCount switch
        {
            2 => "di-livery",
            3 => "tri-livery",
            4 => "tet-livery",
            5 => "pent-livery",
            6 => "hex-livery",
            7 => "hept-livery",
            8 => "oct-livery",
            9 => "non-livery",
            10 => "de-livery",
            _ => null
        };
    }
}
