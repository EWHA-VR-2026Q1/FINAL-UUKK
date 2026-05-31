using UnityEngine;
using System;

public class PlaceTracker : MonoBehaviour
{
    public Action onPlaced;
    private bool triggered = false;

    void OnDisable()
    {
        if (triggered) return;
        triggered = true;
        onPlaced?.Invoke();
    }
}