using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;

public class EndingAutoReturn : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 6f;
    [SerializeField] private string targetSceneName = "Entry";
    [SerializeField] private bool askBeforeReturning = true;

    [Header("Ending Scene Stabilization")]
    [SerializeField] private string[] spiderObjectNames = { "Spider_Cute", "Black Widow Variant" };
    [SerializeField] private Vector3 cuteSpiderPosition = new Vector3(-0.45f, 0.77f, 1.35f);
    [SerializeField] private Vector3 blackWidowPosition = new Vector3(0.45f, 0.77f, 1.35f);

    private RectTransform saveButtonRect;
    private RectTransform resetButtonRect;
    private Image saveButtonImage;
    private Image resetButtonImage;
    private readonly Color saveBaseColor = new Color(0.24f, 0.56f, 1f, 1f);
    private readonly Color saveHoverColor = new Color(0.46f, 0.72f, 1f, 1f);
    private readonly Color resetBaseColor = new Color(0.85f, 0.22f, 0.22f, 1f);
    private readonly Color resetHoverColor = new Color(1f, 0.42f, 0.42f, 1f);
    private bool promptVisible;
    private bool choiceHandled;

    private void Start()
    {
        StabilizeEndingScene();
        StartCoroutine(ReturnAfterDelay());
    }

    private void Update()
    {
        if (!promptVisible || choiceHandled)
        {
            return;
        }

        bool pointingAtSave = saveButtonRect != null && VRPointerClickUtility.IsPointingAt(saveButtonRect, 12f);
        bool pointingAtReset = resetButtonRect != null && VRPointerClickUtility.IsPointingAt(resetButtonRect, 12f);

        if (saveButtonImage != null)
            saveButtonImage.color = pointingAtSave ? saveHoverColor : saveBaseColor;

        if (resetButtonImage != null)
            resetButtonImage.color = pointingAtReset ? resetHoverColor : resetBaseColor;

        if (!VRPointerClickUtility.WasClickPressed())
        {
            return;
        }

        if (pointingAtSave)
        {
            SaveAndReturn();
        }
        else if (pointingAtReset)
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

        Canvas canvas = CreateChoiceCanvas();

        GameObject panel = new GameObject("EndingChoicePanel");
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(460f, 150f);
        panelRect.localScale = Vector3.one;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.72f);

        TextMeshProUGUI title = CreateText(panel.transform, "EndingChoiceText",
            "Save your progress?", 22, TextAlignmentOptions.Center);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -36f);
        titleRect.sizeDelta = new Vector2(420f, 44f);

        Button saveButton = CreateButton(panel.transform, "SaveProgressButton", "Save",
            new Vector2(-115f, -38f), saveBaseColor);
        saveButton.onClick.AddListener(SaveAndReturn);
        saveButtonRect = saveButton.GetComponent<RectTransform>();
        saveButtonImage = saveButton.GetComponent<Image>();

        Button resetButton = CreateButton(panel.transform, "ResetProgressButton", "Reset",
            new Vector2(115f, -38f), resetBaseColor);
        resetButton.onClick.AddListener(ResetAndReturn);
        resetButtonRect = resetButton.GetComponent<RectTransform>();
        resetButtonImage = resetButton.GetComponent<Image>();
    }

    private Canvas CreateChoiceCanvas()
    {
        GameObject canvasObject = new GameObject("EndingChoiceCanvas");
        canvasObject.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 50;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(480f, 170f);
        canvasRect.localScale = Vector3.one * 0.01f;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        canvasObject.AddComponent<GraphicRaycaster>();
        PositionCanvasInFrontOfPlayer(canvas.transform);
        return canvas;
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
        rect.sizeDelta = new Vector2(150f, 48f);
        rect.localScale = Vector3.one;

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;

        TextMeshProUGUI label = CreateText(buttonObject.transform, objectName + "Text", labelText, 20,
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
            SaveManager.Instance.data.highestUnlockedStage = Mathf.Max(
                SaveManager.Instance.data.highestUnlockedStage, 9);
            SaveManager.Instance.SaveGame();
        }
        else
        {
            SaveData saveData = new SaveData
            {
                currentScene = targetSceneName,
                highestUnlockedStage = 9
            };
            SaveManager.WriteStoredProgress(saveData);
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
            SaveManager.ClearStoredProgress();
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

    private void StabilizeEndingScene()
    {
        for (int i = 0; i < spiderObjectNames.Length; i++)
        {
            GameObject spider = FindSceneObjectByName(spiderObjectNames[i]);
            if (spider == null) continue;

            spider.SetActive(true);

            if (spider.name == "Spider_Cute")
                spider.transform.position = cuteSpiderPosition;
            else if (spider.name == "Black Widow Variant")
                spider.transform.position = blackWidowPosition;

            foreach (NavMeshAgent agent in spider.GetComponentsInChildren<NavMeshAgent>(true))
                agent.enabled = false;

            foreach (SpiderWander wander in spider.GetComponentsInChildren<SpiderWander>(true))
                wander.enabled = false;

            foreach (SpiderUserAware userAware in spider.GetComponentsInChildren<SpiderUserAware>(true))
                userAware.enabled = false;

            foreach (SpiderGrabbable grabbable in spider.GetComponentsInChildren<SpiderGrabbable>(true))
                grabbable.enabled = false;

            foreach (XRGrabInteractable xrGrab in spider.GetComponentsInChildren<XRGrabInteractable>(true))
                xrGrab.enabled = false;

            foreach (OVRGrabbable ovrGrab in spider.GetComponentsInChildren<OVRGrabbable>(true))
                ovrGrab.enabled = false;

            foreach (Rigidbody rb in spider.GetComponentsInChildren<Rigidbody>(true))
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    private void PositionCanvasInFrontOfPlayer(Transform canvasTransform)
    {
        Camera camera = Camera.main;
        if (camera == null) return;

        Vector3 forward = camera.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f)
            forward = Vector3.forward;

        forward.Normalize();
        canvasTransform.position = camera.transform.position + forward * 1.8f + Vector3.down * 0.22f;
        canvasTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
        canvasTransform.localScale = Vector3.one * 0.01f;
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return null;

        foreach (Transform item in FindObjectsOfType<Transform>(true))
        {
            if (item.name == objectName)
                return item.gameObject;
        }

        return null;
    }
}
