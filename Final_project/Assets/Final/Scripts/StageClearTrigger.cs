using UnityEngine;

public class StageClearTrigger : MonoBehaviour
{
    public int nextStageNumber;

    public void ClearStage()
    {
        ProgressManager.Instance
            .UnlockStage(nextStageNumber);
    }
}
// 스테이지 클리어 조건이 발생하는 오브젝트에 붙일 것