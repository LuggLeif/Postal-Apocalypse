using UnityEngine;

public class XPGainPopupSpawner : MonoBehaviour
{
    [SerializeField] private PlayerExperience experience;

    [Header("UI")]
    [SerializeField] private XPGainPopup popupPrefab;
    [SerializeField] private RectTransform parent;      // usually a UI panel under your Canvas
    [SerializeField] private RectTransform spawnAnchor; // e.g. near the XP bar

    [Header("Scatter")]
    [SerializeField] private Vector2 randomOffset = new Vector2(40f, 10f);

    private void OnEnable()
    {
        if (experience != null) experience.OnXpGained += Spawn;
    }

    private void OnDisable()
    {
        if (experience != null) experience.OnXpGained -= Spawn;
    }

    private void Spawn(int amount)
    {
        if (popupPrefab == null || parent == null || spawnAnchor == null) return;

        var popup = Instantiate(popupPrefab, parent);
        var rt = popup.transform as RectTransform;

        Vector2 basePos = spawnAnchor.anchoredPosition;
        Vector2 jitter = new Vector2(
            UnityEngine.Random.Range(-randomOffset.x, randomOffset.x),
            UnityEngine.Random.Range(-randomOffset.y, randomOffset.y)
        );

        rt.anchoredPosition = basePos + jitter;

        popup.Play(amount);
    }
}
