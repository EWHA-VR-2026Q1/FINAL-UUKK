using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class Step7DecorationStabilizer : MonoBehaviour
{
    [Header("Decoration objects")]
    public string[] objectNames = { "Branch1", "Moss 1", "Soil" };

    [Header("Runtime collider")]
    public Vector3 minimumColliderSize = new Vector3(0.08f, 0.08f, 0.08f);
    public float colliderPadding = 1.15f;

    private void Awake()
    {
        foreach (string objectName in objectNames)
        {
            GameObject item = GameObject.Find(objectName);
            if (item == null)
            {
                Debug.LogWarning($"[Step7DecorationStabilizer] Could not find '{objectName}'.", this);
                continue;
            }

            Stabilize(item);
        }
    }

    private void Stabilize(GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = item.AddComponent<Rigidbody>();
        }

        rb.mass = Mathf.Max(rb.mass, 0.1f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        Collider grabCollider = EnsureGrabCollider(item);
        if (grabCollider != null)
        {
            grabCollider.isTrigger = false;
        }

        Step7ReleaseFreezer freezer = item.GetComponent<Step7ReleaseFreezer>();
        if (freezer == null)
        {
            freezer = item.AddComponent<Step7ReleaseFreezer>();
        }

        freezer.captureStartPose = true;
    }

    private Collider EnsureGrabCollider(GameObject item)
    {
        Collider[] colliders = item.GetComponentsInChildren<Collider>(true);
        foreach (Collider collider in colliders)
        {
            if (collider.enabled && collider.attachedRigidbody == item.GetComponent<Rigidbody>())
            {
                return collider;
            }
        }

        BoxCollider boxCollider = item.AddComponent<BoxCollider>();
        Bounds bounds = CalculateRendererBounds(item);

        if (bounds.size == Vector3.zero)
        {
            boxCollider.size = minimumColliderSize;
            boxCollider.center = Vector3.zero;
            return boxCollider;
        }

        Vector3 localCenter = item.transform.InverseTransformPoint(bounds.center);
        Vector3 localSize = item.transform.InverseTransformVector(bounds.size);
        localSize = new Vector3(Mathf.Abs(localSize.x), Mathf.Abs(localSize.y), Mathf.Abs(localSize.z));
        localSize *= colliderPadding;

        boxCollider.center = localCenter;
        boxCollider.size = new Vector3(
            Mathf.Max(localSize.x, minimumColliderSize.x),
            Mathf.Max(localSize.y, minimumColliderSize.y),
            Mathf.Max(localSize.z, minimumColliderSize.z));

        return boxCollider;
    }

    private static Bounds CalculateRendererBounds(GameObject item)
    {
        Renderer[] renderers = item.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0)
        {
            return new Bounds(item.transform.position, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}

public class Step7ReleaseFreezer : MonoBehaviour
{
    public bool captureStartPose = true;

    private Rigidbody rb;
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void OnReleased()
    {
        StartCoroutine(FreezeAfterRelease());
    }

    private IEnumerator FreezeAfterRelease()
    {
        yield return null;

        if (rb == null)
        {
            yield break;
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;

        if (captureStartPose && transform.position.y < 0.5f)
        {
            transform.SetPositionAndRotation(startPosition, startRotation);
        }
    }
}
