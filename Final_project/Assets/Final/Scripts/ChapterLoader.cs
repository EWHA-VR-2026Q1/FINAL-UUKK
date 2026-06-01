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
            $"Stage {stageNumber}, Unlocked = {unlocked}");

        if (button != null)
        {
            button.interactable = unlocked;
        }

        if (rend != null)
        {
            if (unlocked)
            {
                rend.material = unlockedMaterial;
            }
            else
            {
                rend.material = lockedMaterial;
            }
        }
    }

    private void OnMouseDown()
    {
        if (ProgressManager.Instance.IsUnlocked(stageNumber))
        {
            SceneManager.LoadScene(sceneName);
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