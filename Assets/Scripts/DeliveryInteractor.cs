using System;
using System.Collections;
using UnityEngine;

public class DeliveryInteractor : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerPackageInventory inventory;
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private DeliveryUI deliveryUI;
    [SerializeField] private PlayerSkills skills;

    [Header("Fallback (used if Skills missing)")]
    [SerializeField] private float secondsPerPackageFallback = 2f;

    /// <summary>
    /// Fired when a delivery streak ends. Value is the streak length (packages delivered consecutively).
    /// Only fired for streak >= 2.
    /// </summary>
    public event Action<int> OnDeliveryStreakEnded;

    private DeliveryZone _currentZone;
    private Coroutine _deliverRoutine;
    private int _streakCount;

    private void Awake()
    {
        if (skills == null) skills = GetComponent<PlayerSkills>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<DeliveryZone>(out var zone)) return;
        EnterZone(zone);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<DeliveryZone>(out var zone)) return;
        if (zone == _currentZone) ExitZone();
    }

    private void EnterZone(DeliveryZone zone)
    {
        if (zone == null) return;

        if (_currentZone != null && _currentZone != zone)
            EndStreakIfAny();

        _currentZone = zone;

        if (_deliverRoutine != null)
            StopCoroutine(_deliverRoutine);

        _deliverRoutine = StartCoroutine(DeliverLoop(zone));
    }

    private void ExitZone()
    {
        _currentZone = null;
        EndStreakIfAny();

        if (_deliverRoutine != null)
        {
            StopCoroutine(_deliverRoutine);
            _deliverRoutine = null;
        }

        if (deliveryUI != null) deliveryUI.Hide();
    }

    private IEnumerator DeliverLoop(DeliveryZone zone)
    {
        if (zone == null || inventory == null || experience == null) yield break;

        int id = zone.DestinationId;
        if (id < 0) yield break;

        if (deliveryUI != null) deliveryUI.Show(zone.DestinationName);

        while (_currentZone == zone)
        {
            int have = inventory.GetCount(id);
            if (have <= 0) break;

            float secondsPerPackage = (skills != null) ? skills.DeliverySecondsPerPackage : secondsPerPackageFallback;
            secondsPerPackage = Mathf.Max(0.05f, secondsPerPackage);

            float t = 0f;
            while (t < secondsPerPackage)
            {
                if (_currentZone != zone) yield break;
                t += Time.deltaTime;
                if (deliveryUI != null) deliveryUI.SetProgress01(t / secondsPerPackage);
                yield return null;
            }

            if (_currentZone != zone) yield break;

            bool removed = inventory.RemoveOne(id);
            if (!removed) break;

            _streakCount++;

            int knowledgeBonus = (skills != null) ? skills.KnowledgeXpBonusPerDelivery : 0;
            int xpReward = ComputeXpReward(_streakCount) + knowledgeBonus;

            experience.AddXP(xpReward);

            if (deliveryUI != null) deliveryUI.SetProgress01(0f);
        }

        if (_currentZone == zone)
        {
            EndStreakIfAny();
            if (deliveryUI != null) deliveryUI.Hide();
            _deliverRoutine = null;
        }
    }

    private void EndStreakIfAny()
    {
        if (_streakCount >= 2)
            OnDeliveryStreakEnded?.Invoke(_streakCount);

        _streakCount = 0;
    }

    // Base streak XP: 10, 12, 15, 19... (adds +2, +3, +4...)
    private int ComputeXpReward(int streakCount)
    {
        const int baseXp = 10;
        int n = Mathf.Max(0, streakCount - 1);
        int bonus = (n * (n + 3)) / 2;
        return baseXp + bonus;
    }
}
