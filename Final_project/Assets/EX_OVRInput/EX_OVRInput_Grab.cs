using UnityEngine;

public class EX_OVRInput_Grab : MonoBehaviour
{
    [Header("Hand")]
    public Transform RightHand;

    [Header("Grab")]
    public float grabRadius = 0.2f;
    public LayerMask GrabLayer;

    Rigidbody GrabbedRB;
    Collider PlayerCollider;
    Collider ObjectCollider;

    Vector3 PosOffset;
    Quaternion RotOffset;

    void Start()
    {
        PlayerCollider = GetComponent<CharacterController>().GetComponent<Collider>();
    }

    void Update()
    {
        bool grab = OVRInput.Get(OVRInput.RawButton.RHandTrigger);

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
        Collider[] hits = Physics.OverlapSphere(RightHand.position, grabRadius, GrabLayer);

        if (hits.Length == 0)
            return;

        float minDist = float.MaxValue;
        Rigidbody closest = null;

        foreach (Collider c in hits)
        {
            Rigidbody rb = c.attachedRigidbody;

            if (rb == null)
                continue;

            float d = Vector3.Distance(RightHand.position, rb.position);

            if (d < minDist)
            {
                minDist = d;
                closest = rb;
            }
        }

        if (closest == null)
            return;

        GrabbedRB = closest;

        ObjectCollider = GrabbedRB.GetComponent<Collider>();

        GrabbedRB.isKinematic = true;

        // Player와 충돌 무시
        Physics.IgnoreCollision(ObjectCollider, PlayerCollider, true);

        // 잡힌 오브젝트에 알림 (SpiderGrabbable 같은 컴포넌트가 반응)
        GrabbedRB.gameObject.SendMessage("OnGrabbed", SendMessageOptions.DontRequireReceiver);

        // offset ���
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

        Physics.IgnoreCollision(ObjectCollider, PlayerCollider, false);

        // 놓인 오브젝트에 알림
        GrabbedRB.gameObject.SendMessage("OnReleased", SendMessageOptions.DontRequireReceiver);

        GrabbedRB = null;
        ObjectCollider = null;
    }

    void OnDrawGizmos()
    {
        if (RightHand == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(RightHand.position, grabRadius);
    }
}