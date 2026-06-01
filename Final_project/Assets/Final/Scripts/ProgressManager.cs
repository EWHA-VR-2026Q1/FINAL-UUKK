using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance;

    public static event System.Action OnStageUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UnlockStage(int stageNumber)
    {
        Debug.Log(
            $"Before Unlock : {SaveManager.Instance.data.highestUnlockedStage}");

        if (stageNumber >
            SaveManager.Instance.data.highestUnlockedStage)
        {
            SaveManager.Instance.data.highestUnlockedStage =
                stageNumber;

            SaveManager.Instance.SaveGame();

            Debug.Log(
                $"After Unlock : {SaveManager.Instance.data.highestUnlockedStage}");

            OnStageUnlocked?.Invoke();
        }
    }

    public bool IsUnlocked(int stageNumber)
    {
        return stageNumber <=
            SaveManager.Instance.data.highestUnlockedStage;
    }
}