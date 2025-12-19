using System.Collections;
using UnityEngine;

public class DeliveryInteractor : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerPackageInventory inventory;
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private DeliveryUI deliveryUI;

    [Header("Timing")]
    [SerializeField] private float secondsPerPackage = 2f;

    private DeliveryZone _currentZone;
    private Coroutine _deliverRoutine;
    private int _streakCount; // consecutive packages delivered in this uninterrupted session

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

        // Switching zones counts as interruption.
        if (_currentZone != zone) _streakCount = 0;

        _currentZone = zone;

        if (_deliverRoutine != null)
            StopCoroutine(_deliverRoutine);

        _deliverRoutine = StartCoroutine(DeliverLoop(zone));
    }

    private void ExitZone()
    {
        _currentZone = null;
        _streakCount = 0;

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

            float t = 0f;
            while (t < secondsPerPackage)
            {
                if (_currentZone != zone) yield break; // interrupted
                t += Time.deltaTime;
                if (deliveryUI != null) deliveryUI.SetProgress01(t / secondsPerPackage);
                yield return null;
            }

            // Deliver one package
            if (_currentZone != zone) yield break;

            bool removed = inventory.RemoveOne(id);
            if (!removed) break;

            _streakCount++;
            int xpReward = ComputeXpReward(_streakCount);
            experience.AddXP(xpReward);

            if (deliveryUI != null) deliveryUI.SetProgress01(0f);
        }

        // Session ended (no packages or left zone)
        _streakCount = 0;
        if (deliveryUI != null) deliveryUI.Hide();
        _deliverRoutine = null;
    }

    // Example requested: 10, 12, 15, 19... (adds +2, +3, +4...)
    private int ComputeXpReward(int streakCount)
    {
        const int baseXp = 10;
        int n = Mathf.Max(0, streakCount - 1);
        int bonus = (n * (n + 3)) / 2;
        return baseXp + bonus;
    }
}
