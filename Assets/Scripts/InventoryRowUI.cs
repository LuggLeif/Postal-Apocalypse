using TMPro;
using UnityEngine;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text destinationText;
    [SerializeField] private TMP_Text countText;

    public void Set(string destinationName, int count)
    {
        if (destinationText) destinationText.text = destinationName;
        if (countText) countText.text = count.ToString();
    }
}
