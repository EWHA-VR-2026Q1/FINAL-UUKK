using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 모델의 "앞" 방향이 GameObject의 +Z와 반대일 때 (대표적으로 Blender에서 -Y 방향으로 만든 모델)
/// 거미가 뒷걸음질 치는 것처럼 보이는 문제를 해결한다.
///
/// 작동 방식:
///   1) NavMeshAgent의 자동 회전(updateRotation)을 끔
///   2) 매 프레임 agent.velocity 방향을 보고, yOffset 만큼 회전을 더해서 transform.rotation을 직접 설정
///
/// 사용법:
///   - 거미 prefab의 루트 GameObject에 이 컴포넌트만 추가
///   - 모델이 뒷걸음질치면 yOffset = 180
///   - 옆걸음질치면 yOffset = 90 또는 -90 (모델에 따라)
///   - 정상으로 보이면 이 컴포넌트 자체가 불필요 (제거하거나 yOffset = 0)
///
/// SpiderAnimatorDriver는 velocity의 magnitude를 사용하도록 수정돼 있어서
/// 이 회전 보정과 100% 호환된다.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class SpiderFacingFix : MonoBehaviour
{
    [Tooltip("모델 forward 보정 각도(deg). 뒷걸음질치면 180을 넣어라.")]
    public float yOffset = 180f;

    [Tooltip("회전 보간 속도. 클수록 즉각, 작을수록 부드럽게 회전.")]
    public float turnSpeed = 12f;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // NavMeshAgent의 자동 회전을 꺼서 우리가 직접 제어
        agent.updateRotation = false;
    }

    void Update()
    {
        Vector3 vel = agent.velocity;
        vel.y = 0f;

        // 거의 안 움직이는 경우엔 회전 갱신하지 않음 (어색한 미세 회전 방지)
        if (vel.sqrMagnitude < 0.0025f) return;

        // velocity 방향을 보고, yOffset 만큼 더 회전한 상태로 맞춤
        Quaternion baseRot = Quaternion.LookRotation(vel.normalized);
        Quaternion targetRot = baseRot * Quaternion.Euler(0f, yOffset, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
    }
}
