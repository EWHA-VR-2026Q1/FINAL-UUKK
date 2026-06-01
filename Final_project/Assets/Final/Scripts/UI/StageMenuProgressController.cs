using UnityEngine;

public class StageMenuProgressController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private StageMenuButton[] stageButtons;
    [SerializeField] private bool findButtonsInChildren = true;

    [Header("State")]
    [SerializeField] private bool markCurrentStage = true;

    private void Awake()
    {
        CacheButtons();
    }

    private void OnEnable()
    {
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        CacheButtons();

        if (SaveManager.Instance == null || SaveManager.Instance.data == null)
        {
            Debug.LogWarning("[StageMenuProgressController] SaveManager is not ready.", this);
            return;
        }

        int highestUnlockedStage = SaveManager.Instance.data.highestUnlockedStage;

        foreach (StageMenuButton button in stageButtons)
        {
            if (button == null) continue;

            if (button.stageNumber > highestUnlockedStage)
            {
                button.SetState(StageMenuButton.State.Locked);
            }
            else if (markCurrentStage && button.stageNumber == highestUnlockedStage)
            {
                button.SetState(StageMenuButton.State.Current);
            }
            else
            {
                button.SetState(StageMenuButton.State.Cleared);
            }
        }
    }

    private void CacheButtons()
    {
        if (!findButtonsInChildren) return;
        stageButtons = GetComponentsInChildren<StageMenuButton>(true);
    }
}
