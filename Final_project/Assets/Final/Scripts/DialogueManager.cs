using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour, IPointerClickHandler
{
    public static bool IsDialogueActive { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("Stage To Unlock When Complete")]
    [SerializeField]
    private int nextStage;

    public float typingSpeed = 0.05f;

    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    private void Start()
    {
        IsDialogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        ShowDialogue();
    }

    private void Update()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Return) ||
            IsVRConfirmPressed())
        {
            AdvanceDialogue();
        }

        if (Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began)
        {
            AdvanceDialogue();
        }
    }

    private void OnDestroy()
    {
        IsDialogueActive = false;
    }

    private bool IsVRConfirmPressed()
    {
        return OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) ||
               OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) ||
               OVRInput.GetDown(OVRInput.RawButton.A) ||
               OVRInput.GetDown(OVRInput.RawButton.X) ||
               OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
               OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ||
               OVRInput.GetDown(OVRInput.Button.One) ||
               OVRInput.GetDown(OVRInput.Button.Three);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!IsDialogueActive)
        {
            return;
        }

        NextDialogue();
    }

    private void ShowDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
        {
            EndDialogue();
            return;
        }

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
        IsDialogueActive = false;

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        Debug.Log("Dialogue ended.");

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.UnlockStage(nextStage);
        }
    }
}
