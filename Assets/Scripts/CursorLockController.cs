using UnityEngine;

public class CursorLockController : MonoBehaviour
{
    [Header("Gameplay Cursor")]
    [SerializeField] private bool lockAndHideOnStart = true;

    public bool IsLocked { get; private set; }

    private void Start()
    {
        if (lockAndHideOnStart)
            SetLocked(true);
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;

        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
