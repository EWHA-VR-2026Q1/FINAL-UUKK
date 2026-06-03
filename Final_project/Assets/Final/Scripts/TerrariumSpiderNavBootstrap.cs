using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Sets up the spider in a small terrarium NavMesh at runtime.
/// This keeps the scene/prefab layout simple while avoiding Humanoid-sized
/// NavMeshAgent settings on a tiny walking surface.
/// </summary>
public class TerrariumSpiderNavBootstrap : MonoBehaviour
{
    [Header("Lookup")]
    public string spiderObjectName = "Spider1";
    public string homePointName = "SpiderSpawnPoint";

    [Header("Spider Agent")]
    public int spiderAgentTypeId = -1372625422;
    public float agentRadius = 0.05f;
    public float agentHeight = 0.1f;
    public float moveSpeed = 0.35f;
    public float angularSpeed = 480f;
    public float acceleration = 30f;

    [Header("Terrarium Wander")]
    public float wanderRadius = 0.28f;
    public float navMeshSearchRadius = 0.7f;

    private IEnumerator Start()
    {
        yield return null;
        SetupSpider();
    }

    private void SetupSpider()
    {
        Transform spider = FindChildByName(transform, spiderObjectName);
        Transform homePoint = FindChildByName(transform, homePointName);

        if (spider == null)
        {
            Debug.LogWarning($"[TerrariumSpiderNavBootstrap] Could not find '{spiderObjectName}'.", this);
            return;
        }

        if (homePoint == null)
        {
            homePoint = spider;
            Debug.LogWarning($"[TerrariumSpiderNavBootstrap] Could not find '{homePointName}', using spider position.", this);
        }

        NavMeshAgent agent = spider.GetComponent<NavMeshAgent>();
        if (agent == null) agent = spider.gameObject.AddComponent<NavMeshAgent>();

        agent.enabled = false;
        agent.agentTypeID = spiderAgentTypeId;
        agent.radius = agentRadius;
        agent.height = agentHeight;
        agent.speed = moveSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = 0.02f;
        agent.autoBraking = true;

        Vector3 target = homePoint.position;
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            spider.position = hit.position;
        }
        else
        {
            Debug.LogWarning($"[TerrariumSpiderNavBootstrap] No Spider NavMesh found near '{homePointName}'.", this);
            return;
        }

        agent.enabled = true;
        if (agent.isOnNavMesh)
        {
            agent.Warp(spider.position);
        }
        else
        {
            Debug.LogWarning("[TerrariumSpiderNavBootstrap] Spider agent is still not on a NavMesh after setup.", this);
            agent.enabled = false;
            return;
        }

        SpiderWander wander = spider.GetComponent<SpiderWander>();
        if (wander == null) wander = spider.gameObject.AddComponent<SpiderWander>();

        wander.homePoint = homePoint;
        wander.wanderRadius = wanderRadius;
        wander.moveSpeed = moveSpeed;
        wander.angularSpeed = angularSpeed;
        wander.acceleration = acceleration;
        wander.minBurstDuration = 0.4f;
        wander.maxBurstDuration = 1.1f;
        wander.minPauseDuration = 0.8f;
        wander.maxPauseDuration = 2.2f;
    }

    private Transform FindChildByName(Transform root, string targetName)
    {
        if (root.name == targetName) return root;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindChildByName(root.GetChild(i), targetName);
            if (found != null) return found;
        }

        return null;
    }
}
