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
    private Transform rightPointer;
    private Transform leftPointer;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        clickCollider = GetComponent<Collider>();

        Refresh();
    }

    private void Update()
    {
        if (VRPointerClickUtility.WasClickPressed() &&
            IsClosestChapterHit())
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

    private bool IsClosestChapterHit()
    {
        if (clickCollider == null)
        {
            return false;
        }

        return IsClosestChapterHit(GetPointerRay(ref rightPointer,
                   "RightHandAnchor", "RightHandOnControllerAnchor", "RightHandAnchorDetached"))
               || IsClosestChapterHit(GetPointerRay(ref leftPointer,
                   "LeftHandAnchor", "LeftHandOnControllerAnchor", "LeftHandAnchorDetached"))
               || IsClosestChapterHit(GetCameraRay());
    }

    private bool IsClosestChapterHit(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, 20f, ~0, QueryTriggerInteraction.Collide);
        if (hits == null || hits.Length == 0)
        {
            return false;
        }

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            ChapterLoader loader = hit.collider.GetComponentInParent<ChapterLoader>();
            if (loader != null)
            {
                return loader == this;
            }
        }

        return false;
    }

    private Ray GetPointerRay(ref Transform cachedPointer, params string[] names)
    {
        if (cachedPointer == null)
        {
            cachedPointer = FindFirstTransform(names);
        }

        if (cachedPointer != null)
        {
            return new Ray(cachedPointer.position, cachedPointer.forward);
        }

        return GetCameraRay();
    }

    private Ray GetCameraRay()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            return new Ray(cam.transform.position, cam.transform.forward);
        }

        return new Ray(Vector3.zero, Vector3.forward);
    }

    private Transform FindFirstTransform(params string[] names)
    {
        foreach (string objectName in names)
        {
            GameObject found = GameObject.Find(objectName);
            if (found != null)
            {
                return found.transform;
            }
        }

        return null;
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
