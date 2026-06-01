using UnityEngine;

public class EX_OVRInput_Ray : MonoBehaviour
{
    public Transform LeftController;
    public LineRenderer LeftControllerRay;

    [Header("Right Controller")]
    public Transform RightController;
    public LineRenderer RightControllerRay;
    public bool showRightRayWhileGrabbing = true;

    public float distance = 10f;
    public float rayStartOffset = 0.04f;
    public float fallbackRayWidth = 0.01f;

    void Start()
    {
        if (RightController == null)
        {
            RightController = FindChildByName(transform.root, "RightHandOnControllerAnchor");
        }

        if (RightController == null)
        {
            RightController = FindChildByName(transform.root, "RightControllerInHandAnchor");
        }

        if (RightController == null)
        {
            RightController = FindChildByName(transform.root, "RightHandAnchorDetached");
        }

        if (RightControllerRay == null && RightController != null)
        {
            RightControllerRay = CreateRuntimeRay("RightControllerRay_Runtime");
        }

        SetRayEnabled(LeftControllerRay, false);
        SetRayEnabled(RightControllerRay, false);
    }

    void Update()
    {
        bool leftTrigger = OVRInput.Get(OVRInput.RawButton.LIndexTrigger);
        DrawRay(LeftController, LeftControllerRay, leftTrigger);

        bool rightTrigger = OVRInput.Get(OVRInput.RawButton.RIndexTrigger);
        if (showRightRayWhileGrabbing)
        {
            rightTrigger = rightTrigger || OVRInput.Get(OVRInput.RawButton.RHandTrigger);
        }

        DrawRay(RightController, RightControllerRay, rightTrigger);
    }

    void DrawRay(Transform controller, LineRenderer controllerRay, bool visible)
    {
        if (controller == null || controllerRay == null)
        {
            return;
        }

        if (!visible)
        {
            SetRayEnabled(controllerRay, false);
            return;
        }

        SetRayEnabled(controllerRay, true);

        Vector3 start = controller.position + controller.forward * rayStartOffset;
        Vector3 dir = controller.forward;

        controllerRay.SetPosition(0, start);

        if (Physics.Raycast(start, dir, out RaycastHit hit, distance))
        {
            controllerRay.SetPosition(1, hit.point);
        }
        else
        {
            controllerRay.SetPosition(1, start + dir * distance);
        }
    }

    LineRenderer CreateRuntimeRay(string objectName)
    {
        GameObject rayObject = new GameObject(objectName);
        rayObject.transform.SetParent(transform, false);

        LineRenderer ray = rayObject.AddComponent<LineRenderer>();
        ray.positionCount = 2;
        ray.useWorldSpace = true;

        if (LeftControllerRay != null)
        {
            ray.material = LeftControllerRay.material;
            ray.startColor = LeftControllerRay.startColor;
            ray.endColor = LeftControllerRay.endColor;
            ray.startWidth = LeftControllerRay.startWidth;
            ray.endWidth = LeftControllerRay.endWidth;
        }
        else
        {
            ray.startColor = Color.white;
            ray.endColor = Color.white;
            ray.startWidth = fallbackRayWidth;
            ray.endWidth = fallbackRayWidth;
        }

        return ray;
    }

    void SetRayEnabled(LineRenderer ray, bool enabled)
    {
        if (ray != null)
        {
            ray.enabled = enabled;
        }
    }

    Transform FindChildByName(Transform root, string childName)
    {
        if (root == null)
        {
            return null;
        }

        Transform[] children = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }
}
