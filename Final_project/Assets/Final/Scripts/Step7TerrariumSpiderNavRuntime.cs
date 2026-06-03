using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(-450)]
public class Step7TerrariumSpiderNavRuntime : MonoBehaviour
{
    [Header("Lookup")]
    public string terrariumZoneName = "TerrariumInsideZone";
    public string spiderSpawnPointName = "SpiderSpawnPoint";
    public string[] spiderObjectNames =
    {
        "Black Widow Variant",
        "Spider1",
        "Spider_Cute"
    };

    [Header("NavMesh")]
    public int spiderAgentTypeId = -1372625422;
    public float floorThickness = 0.025f;
    public float floorInset = 0.08f;
    public float navMeshSearchRadius = 0.8f;

    [Header("Spider Agent")]
    public float agentRadius = 0.045f;
    public float agentHeight = 0.1f;
    public float moveSpeed = 0.22f;
    public float angularSpeed = 480f;
    public float acceleration = 30f;
    public float wanderRadius = 0.22f;

    private void Awake()
    {
        BuildTerrariumNavMesh();
        SetupSpiderOnNavMesh();
    }

    private void BuildTerrariumNavMesh()
    {
        Transform zone = FindByName(terrariumZoneName);
        Transform spawnPoint = FindByName(spiderSpawnPointName);

        Bounds walkBounds = zone != null ? GetWorldBounds(zone) : new Bounds(transform.position, new Vector3(0.5f, 0.1f, 0.5f));

        Vector3 center = walkBounds.center;
        if (spawnPoint != null)
        {
            center.y = spawnPoint.position.y - floorThickness * 0.5f;
        }
        else
        {
            center.y = walkBounds.min.y + floorThickness * 0.5f;
        }

        Vector3 size = new Vector3(
            Mathf.Max(0.12f, walkBounds.size.x - floorInset * 2f),
            floorThickness,
            Mathf.Max(0.12f, walkBounds.size.z - floorInset * 2f));

        GameObject navRoot = new GameObject("Step7_Runtime_Terrarium_NavMesh");
        navRoot.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Step7_Runtime_Terrarium_NavFloor";
        floor.transform.SetParent(navRoot.transform, false);
        floor.transform.position = center;
        floor.transform.localScale = size;

        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }

        Collider collider = floor.GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }

        NavMeshSurface surface = navRoot.AddComponent<NavMeshSurface>();
        surface.agentTypeID = spiderAgentTypeId;
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.layerMask = 1 << floor.layer;
        surface.overrideVoxelSize = true;
        surface.voxelSize = 0.01f;
        surface.overrideTileSize = true;
        surface.tileSize = 32;
        surface.minRegionArea = 0f;
        surface.BuildNavMesh();
    }

    private void SetupSpiderOnNavMesh()
    {
        GameObject spider = FindSpider();
        if (spider == null)
        {
            Debug.LogWarning("[Step7TerrariumSpiderNavRuntime] Could not find a Step7 spider.", this);
            return;
        }

        Transform spawnPoint = FindByName(spiderSpawnPointName);
        Vector3 target = spawnPoint != null ? spawnPoint.position : spider.transform.position;

        if (!NavMesh.SamplePosition(target, out NavMeshHit hit, navMeshSearchRadius, NavMesh.AllAreas))
        {
            Debug.LogWarning("[Step7TerrariumSpiderNavRuntime] Could not find the runtime terrarium NavMesh near the spider spawn point.", this);
            return;
        }

        spider.transform.position = hit.position;

        Rigidbody rb = spider.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spider.AddComponent<Rigidbody>();
        }

        rb.mass = 0.05f;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Collider spiderCollider = spider.GetComponent<Collider>();
        if (spiderCollider == null)
        {
            CapsuleCollider capsule = spider.AddComponent<CapsuleCollider>();
            capsule.radius = 0.08f;
            capsule.height = 0.16f;
            capsule.center = new Vector3(0f, 0.08f, 0f);
            spiderCollider = capsule;
        }

        spiderCollider.isTrigger = false;

        NavMeshAgent agent = spider.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = spider.AddComponent<NavMeshAgent>();
        }

        agent.enabled = false;
        agent.agentTypeID = spiderAgentTypeId;
        agent.radius = agentRadius;
        agent.height = agentHeight;
        agent.speed = moveSpeed;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.stoppingDistance = 0.015f;
        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.enabled = true;
        agent.Warp(hit.position);

        SpiderWander wander = spider.GetComponent<SpiderWander>();
        if (wander == null)
        {
            wander = spider.AddComponent<SpiderWander>();
        }

        wander.homePoint = spawnPoint != null ? spawnPoint : spider.transform;
        wander.wanderRadius = wanderRadius;
        wander.moveSpeed = moveSpeed;
        wander.angularSpeed = angularSpeed;
        wander.acceleration = acceleration;
        wander.minBurstDuration = 0.35f;
        wander.maxBurstDuration = 0.9f;
        wander.minPauseDuration = 1.2f;
        wander.maxPauseDuration = 3.0f;
        wander.idleTwitch = true;

        if (spider.GetComponent<SpiderGrabbable>() == null)
        {
            spider.AddComponent<SpiderGrabbable>();
        }
    }

    private GameObject FindSpider()
    {
        foreach (string spiderName in spiderObjectNames)
        {
            Transform found = FindByName(spiderName);
            if (found != null && found.gameObject.activeInHierarchy)
            {
                return found.gameObject;
            }
        }

        GameObject[] tagged = GameObject.FindGameObjectsWithTag("Spider");
        foreach (GameObject candidate in tagged)
        {
            if (candidate.activeInHierarchy)
            {
                return candidate;
            }
        }

        return null;
    }

    private Transform FindByName(string targetName)
    {
        if (string.IsNullOrWhiteSpace(targetName))
        {
            return null;
        }

        Transform[] all = FindObjectsOfType<Transform>(true);
        foreach (Transform candidate in all)
        {
            if (candidate.name == targetName)
            {
                return candidate;
            }
        }

        return null;
    }

    private Bounds GetWorldBounds(Transform target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds;
        }

        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        return new Bounds(target.position, new Vector3(0.5f, 0.1f, 0.5f));
    }
}
