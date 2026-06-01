using UnityEngine;

public class ChapterLoader : MonoBehaviour
{
    public int stageNumber;
    public string sceneName;

    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        bool unlocked =
            ProgressManager.Instance.IsUnlocked(stageNumber);

        if (!unlocked)
        {
            rend.material.color = Color.gray;
        }
    }

    private void OnMouseDown()
    {
        if (ProgressManager.Instance.IsUnlocked(stageNumber))
        {
            UnityEngine.SceneManagement.SceneManager
                .LoadScene(sceneName);
        }
    }
}