using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ChapterLoader : MonoBehaviour, IPointerClickHandler
{
    public int stageNumber;
    public string sceneName;

    [Header("UI")]
    [SerializeField] private Button button;

    [Header("Materials")]
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material unlockedMaterial;

    private Renderer rend;
    private Collider clickCollider;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        clickCollider = GetComponent<Collider>();

        Refresh();
    }

    private void Update()
    {
        if (VRPointerClickUtility.WasClickPressed() &&
            VRPointerClickUtility.IsPointingAt(clickCollider))
        {
            TryLoadChapter();
        }
    }

    public void Refresh()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[ChapterLoader] ProgressManager is not ready.", this);
            return;
        }

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
        TryLoadChapter();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TryLoadChapter();
    }

    private void TryLoadChapter()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[ChapterLoader] ProgressManager is not ready.", this);
            return;
        }

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
