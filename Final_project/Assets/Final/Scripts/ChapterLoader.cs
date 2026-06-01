using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChapterLoader : MonoBehaviour
{
    public int stageNumber;
    public string sceneName;

    [Header("UI")]
    [SerializeField] private Button button;

    [Header("Materials")]
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material unlockedMaterial;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        Refresh();
    }

    public void Refresh()
    {
        bool unlocked =
            ProgressManager.Instance.IsUnlocked(stageNumber);

        Debug.Log(
            $"Stage={stageNumber}, " +
            $"Unlocked={unlocked}");

        if (rend != null)
        {
            Material target =
                unlocked ? unlockedMaterial : lockedMaterial;

            rend.material = target;

            Debug.Log(
                $"Applied Material = {target.name}");
        }
    }

    private void OnMouseDown()
    {
        bool unlocked =
            ProgressManager.Instance.IsUnlocked(stageNumber);

        Debug.Log(
            $"Clicked Stage={stageNumber}, " +
            $"Highest={SaveManager.Instance.data.highestUnlockedStage}, " +
            $"Unlocked={unlocked}");

        if (unlocked)
        {
            Debug.Log($"Loading {sceneName}");

            UnityEngine.SceneManagement.SceneManager
                .LoadScene(sceneName);
        }
    }

    private void OnEnable()
    {
        ProgressManager.OnStageUnlocked += Refresh;
    }

    private void OnDisable()
    {
        ProgressManager.OnStageUnlocked -= Refresh;
    }
}