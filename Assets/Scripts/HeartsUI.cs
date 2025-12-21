using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;

    [Header("UI")]
    [SerializeField] private Transform container;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    private readonly List<Image> _hearts = new();

    private void Awake()
    {
        if (health == null) health = FindFirstObjectByType<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (health != null) health.OnChanged -= Refresh;
    }

    private void Refresh()
    {
        if (health == null || container == null || heartPrefab == null) return;

        int max = health.MaxHearts;

        // Ensure enough heart images exist
        while (_hearts.Count < max)
        {
            var img = Instantiate(heartPrefab, container);
            _hearts.Add(img);
        }

        // Enable only up to max
        for (int i = 0; i < _hearts.Count; i++)
        {
            _hearts[i].gameObject.SetActive(i < max);
        }

        // Set sprites
        int cur = health.CurrentHearts;
        for (int i = 0; i < max; i++)
        {
            _hearts[i].sprite = (i < cur) ? fullHeartSprite : emptyHeartSprite;
        }
    }
}
