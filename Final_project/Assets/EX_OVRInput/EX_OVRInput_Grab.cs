using UnityEngine;

public class EX_OVRInput_Grab : MonoBehaviour
{
    public static event System.Action<GameObject> AnyObjectGrabbed;

    [Header("Hand")]
    public Transform RightHand;

    [Header("Grab")]
    public float grabRadius = 0.2f;
    public bool enableRayGrab = false;
    public float rayGrabDistance = 0.8f;
    public LayerMask GrabLayer;
    public bool searchAllLayersIfNoGrabLayerHit = true;

    [Header("Debug")]
    [Tooltip("매 trigger 입력 시점에 OverlapSphere 결과를 콘솔에 출력. 진단 끝나면 끄는 게 좋음.")]
    public bool verboseGrabLog = false;

    Rigidbody GrabbedRB;
    Collider PlayerCollider;
    Collider ObjectCollider;

    Vector3 PosOffset;
    Quaternion RotOffset;

    void Start()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
            PlayerCollider = controller.GetComponent<Collider>();

        if (RightHand == null)
        {
            Transform found = FindChildByName(transform.root, "RightHandOnControllerAnchor");
            if (found == null) found = FindChildByName(transform.root, "RightHandAnchorDetached");
            RightHand = found;
        }

        if (GrabLayer.value == 0)
        {
            GrabLayer = LayerMask.GetMask("Default");
            Debug.LogWarning("[EX_OVRInput_Grab] GrabLayer 가 비어있어서 Default 레이어로 자동 설정함.", this);
        }

        if (RightHand == null)
            Debug.LogError("[EX_OVRInput_Grab] RightHand 를 자동으로 찾지 못함. Inspector에서 RightHandAnchor를 드래그해야 함.", this);
    }

    void Update()
    {
        bool grab =
            OVRInput.Get(OVRInput.RawButton.RHandTrigger) ||
            OVRInput.Get(OVRInput.RawButton.RIndexTrigger);

        if (grab)
        {
            if (GrabbedRB == null)
                TryGrab();
            else
                UpdateGrab();
        }
        else
        {
            if (GrabbedRB != null)
                Release();
        }
    }

    void TryGrab()
    {
        if (RightHand == null) return;

        Collider[] hits = Physics.OverlapSphere(RightHand.position, grabRadius, GrabLayer);

        if (verboseGrabLog)
        {
            string hitList = hits.Length == 0 ? "(none)" : "";
            foreach (var h in hits)
                hitList += $"\n  - {h.name} (layer={LayerMask.LayerToName(h.gameObject.layer)}, rb={(h.attachedRigidbody ? h.attachedRigidbody.name : "null")})";
            Debug.Log($"[EX_OVRInput_Grab] TryGrab @ {RightHand.position} radius={grabRadius} layerMask={GrabLayer.value} hits={hits.Length}{hitList}", this);
        }

        if (TryGrabFromHits(hits))
        {
            return;
        }

        if (searchAllLayersIfNoGrabLayerHit)
        {
            Collider[] allLayerHits = Physics.OverlapSphere(RightHand.position, grabRadius);

            if (verboseGrabLog)
            {
                string hitList = allLayerHits.Length == 0 ? "(none)" : "";
                foreach (var h in allLayerHits)
                    hitList += $"\n  - {h.name} (layer={LayerMask.LayerToName(h.gameObject.layer)}, rb={(h.attachedRigidbody ? h.attachedRigidbody.name : "null")})";
                Debug.Log($"[EX_OVRInput_Grab] Fallback all-layer search hits={allLayerHits.Length}{hitList}", this);
            }

            if (TryGrabFromHits(allLayerHits))
            {
                return;
            }
        }

        if (enableRayGrab)
        {
            TryRayGrab();
        }
    }

    bool TryGrabFromHits(Collider[] hits)
    {
        if (hits == null || hits.Length == 0)
            return false;

        float minDist = float.MaxValue;
        Rigidbody closest = null;
        Collider closestCollider = null;

        foreach (Collider c in hits)
        {
            if (c.transform.root == transform.root)
                continue;

            Rigidbody rb = c.attachedRigidbody;

            if (rb == null)
                continue;

            float d = Vector3.Distance(RightHand.position, rb.position);

            if (d < minDist)
            {
                minDist = d;
                closest = rb;
                closestCollider = c;
            }
        }

        if (closest == null)
            return false;

        GrabbedRB = closest;

        ObjectCollider = closestCollider != null ? closestCollider : GrabbedRB.GetComponent<Collider>();

        GrabbedRB.isKinematic = true;

        // Player와 충돌 무시
        if (ObjectCollider != null && PlayerCollider != null)
            Physics.IgnoreCollision(ObjectCollider, PlayerCollider, true);

        // 잡힌 오브젝트에 알림 (SpiderGrabbable 같은 컴포넌트가 반응)
        NotifyGrabbed(GrabbedRB.gameObject);

        // offset ���
        PosOffset = Quaternion.Inverse(RightHand.rotation) * (GrabbedRB.position - RightHand.position);

        RotOffset = Quaternion.Inverse(RightHand.rotation) * GrabbedRB.rotation;

        return true;
    }

    void TryRayGrab()
    {
        Ray ray = new Ray(RightHand.position, RightHand.forward);
        RaycastHit[] rayHits = Physics.RaycastAll(ray, rayGrabDistance, GrabLayer);
        System.Array.Sort(rayHits, (a, b) => a.distance.CompareTo(b.distance));

        RaycastHit hit = default;
        Rigidbody rb = null;

        foreach (RaycastHit candidate in rayHits)
        {
            if (candidate.collider.transform.root == transform.root)
                continue;

            rb = candidate.collider.attachedRigidbody;
            if (rb != null)
            {
                hit = candidate;
                break;
            }
        }

        if (rb == null)
        {
            if (verboseGrabLog)
                Debug.Log($"[EX_OVRInput_Grab] RayGrab miss. origin={RightHand.position} dir={RightHand.forward} dist={rayGrabDistance} hits={rayHits.Length}", this);
            return;
        }

        if (verboseGrabLog)
            Debug.Log($"[EX_OVRInput_Grab] RayGrab hit {hit.collider.name} rb={(rb ? rb.name : "null")}", this);

        GrabbedRB = rb;
        ObjectCollider = hit.collider;
        GrabbedRB.isKinematic = true;

        if (ObjectCollider != null && PlayerCollider != null)
            Physics.IgnoreCollision(ObjectCollider, PlayerCollider, true);

        NotifyGrabbed(GrabbedRB.gameObject);

        PosOffset = Quaternion.Inverse(RightHand.rotation) * (GrabbedRB.position - RightHand.position);
        RotOffset = Quaternion.Inverse(RightHand.rotation) * GrabbedRB.rotation;
    }

    void UpdateGrab()
    {
        Vector3 targetPos = RightHand.position + RightHand.rotation * PosOffset;

        Quaternion targetRot = RightHand.rotation * RotOffset;

        GrabbedRB.MovePosition(targetPos);
        GrabbedRB.MoveRotation(targetRot);
    }

    void Release()
    {
        GrabbedRB.isKinematic = false;

        // 던지기 (놓을 때 컨트롤러 속도 그대로 전달)
        GrabbedRB.velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        GrabbedRB.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);

        if (ObjectCollider != null && PlayerCollider != null)
            Physics.IgnoreCollision(ObjectCollider, PlayerCollider, false);

        // 놓인 오브젝트에 알림
        GrabbedRB.gameObject.SendMessage("OnReleased", SendMessageOptions.DontRequireReceiver);

        GrabbedRB = null;
        ObjectCollider = null;
    }

    Transform FindChildByName(Transform root, string childName)
    {
        if (root == null) return null;

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == childName)
                return child;
        }

        return null;
    }

    void NotifyGrabbed(GameObject grabbedObject)
    {
        if (grabbedObject == null)
            return;

        grabbedObject.SendMessage("OnGrabbed", SendMessageOptions.DontRequireReceiver);
        AnyObjectGrabbed?.Invoke(grabbedObject);
    }

    void OnDrawGizmos()
    {
        if (RightHand == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(RightHand.position, grabRadius);
    }
}
