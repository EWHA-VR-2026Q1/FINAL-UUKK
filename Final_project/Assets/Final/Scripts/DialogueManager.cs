using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("대사")]
    [TextArea(2, 5)]
    public string[] dialogues;

    public float typingSpeed = 0.05f;

    private int currentIndex = 0;

    private Coroutine typingCoroutine;

    private bool isTyping = false;

    public StageClearTrigger stageClear;

    [SerializeField]
    public int nextStage;

    void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        ShowDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextDialogue();
        }

        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            NextDialogue();
        }
    }

    void ShowDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine =
            StartCoroutine(TypeDialogue(dialogues[currentIndex]));
    }

    IEnumerator TypeDialogue(string dialogue)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in dialogue)
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextDialogue()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            dialogueText.text = dialogues[currentIndex];

            isTyping = false;

            return;
        }

        currentIndex++;

        if (currentIndex < dialogues.Length)
        {
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
{
    dialogueText.text = "";

    if (dialoguePanel != null)
    {
        dialoguePanel.SetActive(false);
    }

    Debug.Log("대화 종료");
    stageClear.ClearStage();
    ProgressManager.Instance.UnlockStage(nextStage);
}
}