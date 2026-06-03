using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingAutoReturn : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 30f;
    [SerializeField] private string targetSceneName = "Entry";
    [SerializeField] private bool askBeforeReturning = true;

    private RectTransform saveButtonRect;
    private RectTransform resetButtonRect;
    private bool promptVisible;
    private bool choiceHandled;

    private void Start()
    {
        StartCoroutine(ReturnAfterDelay());
    }

    private void Update()
    {
        if (!promptVisible || choiceHandled || !VRPointerClickUtility.WasClickPressed())
        {
            return;
        }

        if (saveButtonRect != null && VRPointerClickUtility.IsPointingAt(saveButtonRect, 12f))
        {
            SaveAndReturn();
        }
        else if (resetButtonRect != null && VRPointerClickUtility.IsPointingAt(resetButtonRect, 12f))
        {
            ResetAndReturn();
        }
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (askBeforeReturning)
        {
            ShowEndingPrompt();
            yield break;
        }

        LoadTargetScene();
    }

    private void ShowEndingPrompt()
    {
        if (promptVisible) return;
        promptVisible = true;

        Canvas canvas = FindEndingCanvas();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("EndingChoiceCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("EndingChoicePanel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0f, -180f);
        panelRect.sizeDelta = new Vector2(680f, 210f);
        panelRect.localScale = Vector3.one;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.78f);

        TextMeshProUGUI title = CreateText(panel.transform, "EndingChoiceText",
            "Save your current progress?", 34, TextAlignmentOptions.Center);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -54f);
        titleRect.sizeDelta = new Vector2(620f, 70f);

        Button saveButton = CreateButton(panel.transform, "SaveProgressButton", "Save Progress",
            new Vector2(-170f, -55f), new Color(0.24f, 0.56f, 1f, 1f));
        saveButton.onClick.AddListener(SaveAndReturn);
        saveButtonRect = saveButton.GetComponent<RectTransform>();

        Button resetButton = CreateButton(panel.transform, "ResetProgressButton", "Reset Progress",
            new Vector2(170f, -55f), new Color(0.85f, 0.22f, 0.22f, 1f));
        resetButton.onClick.AddListener(ResetAndReturn);
        resetButtonRect = resetButton.GetComponent<RectTransform>();
    }

    private TextMeshProUGUI CreateText(Transform parent, string objectName, string text, int fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.alignment = alignment;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    private Button CreateButton(Transform parent, string objectName, string labelText, Vector2 position,
        Color color)
    {
        GameObject buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(260f, 76f);
        rect.localScale = Vector3.one;

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI label = CreateText(buttonObject.transform, objectName + "Text", labelText, 26,
            TextAlignmentOptions.Center);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return button;
    }

    private void SaveAndReturn()
    {
        if (choiceHandled) return;
        choiceHandled = true;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.data.currentScene = targetSceneName;
            SaveManager.Instance.SaveGame();
        }

        LoadTargetScene();
    }

    private void ResetAndReturn()
    {
        if (choiceHandled) return;
        choiceHandled = true;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetGame();
        }
        else
        {
            PlayerPrefs.DeleteKey("SpiderName");
            PlayerPrefs.Save();
        }

        LoadTargetScene();
    }

    private void LoadTargetScene()
    {
        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("[EndingAutoReturn] Target scene name is empty.", this);
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private Canvas FindEndingCanvas()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>(true);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.name == "Canvas" && canvas.transform.parent == null)
            {
                return canvas;
            }
        }

        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                return canvas;
            }
        }

        return canvases.Length > 0 ? canvases[0] : null;
    }
}
