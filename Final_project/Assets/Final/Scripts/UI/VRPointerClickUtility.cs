using UnityEngine;

public static class VRPointerClickUtility
{
    private const float DefaultMaxDistance = 20f;

    public static bool WasClickPressed()
    {
        return OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger) ||
               OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger) ||
               OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
               OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ||
               OVRInput.GetDown(OVRInput.RawButton.A) ||
               OVRInput.GetDown(OVRInput.RawButton.X);
    }

    public static bool IsPointingAt(RectTransform rectTransform, float maxDistance = DefaultMaxDistance)
    {
        if (rectTransform == null)
        {
            return false;
        }

        return IsPointerRayOverRect(GetRightPointerRay(), rectTransform, maxDistance) ||
               IsPointerRayOverRect(GetLeftPointerRay(), rectTransform, maxDistance) ||
               IsPointerRayOverRect(GetCameraRay(), rectTransform, maxDistance);
    }

    public static bool IsPointingAt(Collider collider, float maxDistance = DefaultMaxDistance)
    {
        if (collider == null)
        {
            return false;
        }

        return IsPointerRayHittingCollider(GetRightPointerRay(), collider, maxDistance) ||
               IsPointerRayHittingCollider(GetLeftPointerRay(), collider, maxDistance) ||
               IsPointerRayHittingCollider(GetCameraRay(), collider, maxDistance);
    }

    private static Ray GetRightPointerRay()
    {
        Transform hand = GameObject.Find("RightHandAnchor")?.transform;
        return hand != null ? new Ray(hand.position, hand.forward) : GetCameraRay();
    }

    private static Ray GetLeftPointerRay()
    {
        Transform hand = GameObject.Find("LeftHandAnchor")?.transform;
        return hand != null ? new Ray(hand.position, hand.forward) : GetCameraRay();
    }

    private static Ray GetCameraRay()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return new Ray(Vector3.zero, Vector3.forward);
        }

        return new Ray(mainCamera.transform.position, mainCamera.transform.forward);
    }

    private static bool IsPointerRayOverRect(Ray ray, RectTransform rectTransform, float maxDistance)
    {
        Plane plane = new Plane(rectTransform.forward, rectTransform.position);
        if (!plane.Raycast(ray, out float distance) || distance > maxDistance)
        {
            return false;
        }

        Vector3 worldPoint = ray.GetPoint(distance);
        Vector2 localPoint = rectTransform.InverseTransformPoint(worldPoint);
        return rectTransform.rect.Contains(localPoint);
    }

    private static bool IsPointerRayHittingCollider(Ray ray, Collider target, float maxDistance)
    {
        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            return false;
        }

        return hit.collider == target || hit.collider.transform.IsChildOf(target.transform);
    }
}
