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

    [Header("클리어 시 해금할 스테이지")]
    [SerializeField]
    private int nextStage;

    public float typingSpeed = 0.05f;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        ShowDialogue();
    }

    private void Update()
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

    private void ShowDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine =
            StartCoroutine(TypeDialogue(dialogues[currentIndex]));
    }

    private IEnumerator TypeDialogue(string dialogue)
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

    private void NextDialogue()
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

    private void EndDialogue()
    {
        dialogueText.text = "";

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Debug.Log("대화 종료");

        ProgressManager.Instance.UnlockStage(nextStage);
    }
}