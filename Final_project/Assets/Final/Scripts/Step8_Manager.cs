using UnityEngine;

public class Scene08_Manager : MonoBehaviour
{
    public int nextStageNumber = 9;
    private bool cleared = false;

    public void OnSpiderInHouse(GameObject spider)
    {
        if (cleared) return;
        cleared = true;

        Debug.Log("[Scene08] Spider is in the house. Stage clear!");

        ProgressManager.Instance.UnlockStage(nextStageNumber);
    }
}