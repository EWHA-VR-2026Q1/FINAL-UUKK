using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearStageOnSpiderGrab : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private int nextStageNumber = 1;

    [Header("Scene Load")]
    [SerializeField] private bool loadSceneAfterClear = false;
    [SerializeField] private string nextSceneName = "";

    [Header("Filter")]
    [SerializeField] private bool spiderOnly = true;

    private bool cleared = false;

    private void OnEnable()
    {
        EX_OVRInput_Grab.AnyObjectGrabbed += HandleObjectGrabbed;
    }

    private void OnDisable()
    {
        EX_OVRInput_Grab.AnyObjectGrabbed -= HandleObjectGrabbed;
    }

    public void OnGrabbed()
    {
        ClearStage();
    }

    private void HandleObjectGrabbed(GameObject grabbedObject)
    {
        if (spiderOnly && !IsSpiderObject(grabbedObject))
        {
            return;
        }

        ClearStage();
    }

    private void ClearStage()
    {
        if (cleared) return;
        cleared = true;

        if (ProgressManager.Instance != null)
        {
            ProgressManager.Instance.UnlockStage(nextStageNumber);
        }
        else
        {
            Debug.LogWarning("[ClearStageOnSpiderGrab] ProgressManager is not ready.", this);
        }

        if (loadSceneAfterClear && !string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private bool IsSpiderObject(GameObject grabbedObject)
    {
        if (grabbedObject == null)
        {
            return false;
        }

        if (grabbedObject.CompareTag("Spider") || grabbedObject.name.ToLowerInvariant().Contains("spider"))
        {
            return true;
        }

        if (grabbedObject.GetComponentInParent<SpiderGrabbable>() != null ||
            grabbedObject.GetComponentInChildren<SpiderGrabbable>() != null)
        {
            return true;
        }

        Transform root = grabbedObject.transform.root;
        return root != null && root.name.ToLowerInvariant().Contains("spider");
    }
}
