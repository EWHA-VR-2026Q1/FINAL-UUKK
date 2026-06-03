using System.Collections;
using UnityEngine;

public class Step1GoalHUDSequence : MonoBehaviour
{
    [Header("HUD")]
    public GoalHUD goalHUD;

    [Header("Sequence")]
    public float secondsPerLine = 10f;
    public int nextStageNumber = 2;

    [TextArea(2, 4)]
    public string[] lines =
    {
        "Hi. It is nice to meet you.",
        "Before we start, here is what spiders need.",
        "A good home needs proper floor material and stable structures.",
        "Food is important too.",
        "Your first mission is to learn the basics.",
        "When you are ready, press the button to continue."
    };

    private void Awake()
    {
        DisableLegacyDialogue();

        if (goalHUD == null)
        {
            goalHUD = FindObjectOfType<GoalHUD>(true);
        }

        if (goalHUD != null)
        {
            goalHUD.showOnStart = false;
            goalHUD.autoHideAfter = 0f;
        }
    }

    private IEnumerator Start()
    {
        if (goalHUD == null)
        {
            goalHUD = FindObjectOfType<GoalHUD>(true);
        }

        if (goalHUD == null)
        {
            Debug.LogWarning("[Step1GoalHUDSequence] GoalHUD was not found.", this);
            UnlockNextStage();
            yield break;
        }

        goalHUD.showOnStart = false;
        goalHUD.autoHideAfter = 0f;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            goalHUD.ShowGoal(line);
            yield return new WaitForSeconds(secondsPerLine);
        }

        UnlockNextStage();
    }

    private void DisableLegacyDialogue()
    {
        DialogueManager dialogueManager = GetComponent<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.enabled = false;
        }

        GameObject dialoguePanel = GameObject.Find("DialoguePanel");
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void UnlockNextStage()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[Step1GoalHUDSequence] ProgressManager is not ready.", this);
            return;
        }

        ProgressManager.Instance.UnlockStage(nextStageNumber);
    }
}
