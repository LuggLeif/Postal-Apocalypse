using System.Collections.Generic;
using UnityEngine;

public class SkillChoiceManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerExperience experience;
    [SerializeField] private PlayerSkills skills;
    [SerializeField] private SkillChoiceUI choiceUI;
    [SerializeField] private CursorLockController cursorLock;

    [Header("Behavior")]
    [SerializeField] private bool pauseGameWhileChoosing = true;

    private int _pendingChoices = 0;
    private bool _isShowing;

    private readonly List<SkillType> _pool = new()
    {
        SkillType.Capacity,
        SkillType.Delivery,
        SkillType.Health,
        SkillType.Movement,
        SkillType.Knowledge
    };

    private void Awake()
    {
        if (experience == null) experience = FindFirstObjectByType<PlayerExperience>();
        if (skills == null) skills = FindFirstObjectByType<PlayerSkills>();
        if (choiceUI == null) choiceUI = FindFirstObjectByType<SkillChoiceUI>(FindObjectsInactive.Include);
        if (cursorLock == null) cursorLock = FindFirstObjectByType<CursorLockController>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        if (experience != null) experience.OnLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        if (experience != null) experience.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp(int newLevel)
    {
        _pendingChoices++;
        TryShowNext();
    }

    private void TryShowNext()
    {
        if (_isShowing) return;
        if (_pendingChoices <= 0) return;
        if (skills == null || choiceUI == null) return;

        _isShowing = true;
        _pendingChoices--;

        if (pauseGameWhileChoosing)
            Time.timeScale = 0f;

        if (cursorLock != null)
            cursorLock.SetLocked(false);

        SkillType[] picks = PickThreeDistinct();
        choiceUI.Show(skills, picks, OnSkillChosen);
    }

    private void OnSkillChosen(SkillType skill)
    {
        skills.LevelUp(skill);

        choiceUI.Hide();

        if (pauseGameWhileChoosing)
            Time.timeScale = 1f;

        if (cursorLock != null)
            cursorLock.SetLocked(true);

        _isShowing = false;
        TryShowNext();
    }

    private SkillType[] PickThreeDistinct()
    {
        List<SkillType> temp = new List<SkillType>(_pool);

        for (int i = temp.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (temp[i], temp[j]) = (temp[j], temp[i]);
        }

        return new[] { temp[0], temp[1], temp[2] };
    }
}
