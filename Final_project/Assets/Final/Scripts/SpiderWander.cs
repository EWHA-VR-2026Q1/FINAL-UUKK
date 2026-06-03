using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 실제 거미처럼 "잠깐 다다다 움직이고 → 한참 멈추는" 패턴으로 돌아다닌다.
/// 공격 행동은 일부러 없음. 사용자가 거미와 친해지는 시뮬레이션이라
/// 자연스러운 거미 locomotion(이동)만 흉내냄.
///
/// 사용법:
///  1) 거미 Prefab(Variant)에 NavMeshAgent를 추가
///  2) 이 스크립트를 같은 GameObject에 붙임
///  3) Scene의 바닥을 Navigation Static으로 만들고 NavMesh를 Bake
///  4) (선택) homePoint에 빈 GameObject를 두면 그 주변만 돌아다님
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class SpiderWander : MonoBehaviour
{
    [Header("배회 영역")]
    [Tooltip("거미가 돌아다닐 중심점. 비어있으면 시작 위치를 사용한다.")]
    public Transform homePoint;
    [Tooltip("중심점에서 이 반경 안에서만 돌아다닌다 (미터).")]
    public float wanderRadius = 3f;

    [Header("Burst (한 번 다다다 움직이는 동안)")]
    [Tooltip("한 번 움직이는 최소 시간(초).")]
    public float minBurstDuration = 0.6f;
    [Tooltip("한 번 움직이는 최대 시간(초).")]
    public float maxBurstDuration = 1.6f;

    [Header("Pause (멈춰서 가만히 있는 동안)")]
    [Tooltip("멈춰서 쉬는 최소 시간(초). 클수록 덜 위협적으로 느껴짐.")]
    public float minPauseDuration = 1.0f;
    [Tooltip("멈춰서 쉬는 최대 시간(초).")]
    public float maxPauseDuration = 3.5f;

    [Header("속도")]
    [Tooltip("이동 속도(m/s). 귀여운 거미는 0.4 정도, 사실적 거미는 1.0 정도 추천.")]
    public float moveSpeed = 0.8f;
    [Tooltip("회전 속도(deg/s). 거미는 회전이 빠른 편이라 300~720 사이가 자연스러움.")]
    public float angularSpeed = 480f;
    [Tooltip("가/감속(m/s^2). 거미는 즉각 멈추므로 크게.")]
    public float acceleration = 30f;

    [Header("Idle 미세 트위치 (사실적 거미 느낌)")]
    [Tooltip("멈춰 있을 때 작게 좌우로 살짝 돌리는 동작. 끄면 더 차분함(귀여운 거미용).")]
    public bool idleTwitch = true;
    [Tooltip("트위치 발생 평균 주기(초).")]
    public float twitchInterval = 1.5f;
    [Tooltip("트위치 최대 각도(deg).")]
    public float twitchAngle = 25f;

    private NavMeshAgent agent;
    private Vector3 origin;
    private float stateTimer;
    private float twitchTimer;
    private bool isMoving;

    public bool IsMoving => isMoving;          // 다른 스크립트가 상태 확인할 수 있게 노출
    public float DesignedMoveSpeed => moveSpeed; // SpiderUserAware가 원래 속도로 복원할 때 참고

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.autoBraking = true;
        // 거미는 NavMeshAgent의 "회전 자동 회전"을 그대로 써도 OK
        agent.updateRotation = true;
    }

    void Start()
    {
        origin = homePoint ? homePoint.position : transform.position;
        EnterPause(); // 처음엔 가만히 있다가 시작
    }

    void Update()
    {
        if (!IsAgentReady())
        {
            isMoving = false;
            stateTimer = 0.2f;
            return;
        }

        stateTimer -= Time.deltaTime;

        if (isMoving)
        {
            // 도착했거나 burst 시간이 끝나면 멈춤
            bool arrived = !agent.pathPending && agent.remainingDistance < 0.05f;
            if (stateTimer <= 0f || arrived) EnterPause();
        }
        else
        {
            // 멈춰있는 동안 트위치
            if (idleTwitch) DoIdleTwitch();
            if (stateTimer <= 0f) EnterBurst();
        }
    }

    void EnterBurst()
    {
        if (!IsAgentReady())
        {
            stateTimer = 0.2f;
            isMoving = false;
            return;
        }

        Vector3 candidate = PickRandomPointInRadius(origin, wanderRadius);
        if (NavMesh.SamplePosition(candidate, out var hit, 1.0f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(hit.position);
            stateTimer = Random.Range(minBurstDuration, maxBurstDuration);
            isMoving = true;
        }
        else
        {
            // 유효한 점을 못 찾으면 잠깐 후 재시도
            stateTimer = 0.2f;
        }
    }

    void EnterPause()
    {
        if (IsAgentReady())
        {
            agent.isStopped = true;
        }

        stateTimer = Random.Range(minPauseDuration, maxPauseDuration);
        twitchTimer = Random.Range(0.3f, twitchInterval);
        isMoving = false;
    }

    bool IsAgentReady()
    {
        return agent != null && agent.enabled && agent.isOnNavMesh;
    }

    void DoIdleTwitch()
    {
        twitchTimer -= Time.deltaTime;
        if (twitchTimer > 0f) return;
        float yaw = Random.Range(-twitchAngle, twitchAngle);
        transform.Rotate(0f, yaw, 0f, Space.Self);
        twitchTimer = Random.Range(twitchInterval * 0.6f, twitchInterval * 1.6f);
    }

    Vector3 PickRandomPointInRadius(Vector3 center, float radius)
    {
        Vector2 r = Random.insideUnitCircle * radius;
        return new Vector3(center.x + r.x, center.y, center.z + r.y);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.5f);
        Vector3 c = homePoint ? homePoint.position : transform.position;
        Gizmos.DrawWireSphere(c, wanderRadius);
    }
}
