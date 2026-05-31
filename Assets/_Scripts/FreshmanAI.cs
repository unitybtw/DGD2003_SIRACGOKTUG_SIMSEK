using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class FreshmanAI : MonoBehaviour
{
    public enum AIState
    {
        Patrol,
        Investigate,
        Flee,
        PulledToZone // YENİ: Yeşil bölgenin çekim gücüne kapılma durumu
    }

    [Header("Patrol")]
    public Transform[] waypoints;

    [Header("Movement")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 7.5f;
    public float investigateSpeed = 2.5f;

    [Header("NavMesh Recovery")]
    public bool snapToNavMeshOnStart = true;
    public float navMeshSnapRadius = 10f;

    [Header("Investigation")]
    public float investigationWaitTime = 3f;
    public float investigateStoppingDistance = 0.8f;

    [Header("Flee")]
    public float fleeDistance = 18f;
    public float fleeCooldownTime = 0.25f;

    [Header("Animation")]
    public string speedParameterName = "Speed";
    public string stateParameterName = string.Empty;

    [Header("Visual Fix")]
    public float visualYOffset = 0.9f;

    [Header("Debug")]
    [SerializeField] private AIState currentState = AIState.Patrol;
    [SerializeField] private int currentWaypointIndex = 0;

    private NavMeshAgent agent;
    private Animator animator;

    private Transform[] cachedWaypoints;
    private Vector3 investigateTargetPosition;
    private Vector3 fleeTargetPosition;
    private float investigateTimer;
    private float fleeTimer;
    private bool hasActiveTarget;
    private bool navMeshRecoveryAttempted;
    private bool navMeshUnavailableLogged;
    private bool visualOffsetApplied;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.Normal;
        }

        if (agent != null)
        {
            agent.autoBraking = false;
            agent.stoppingDistance = 0.1f;
        }

        ApplyVisualOffsetOnce();
    }

    private void Start()
    {
        CacheWaypointsIfNeeded();
        if (snapToNavMeshOnStart) TrySnapAgentToNavMesh();
        EnterPatrolState(true);
    }

    private void Update()
    {
        if (agent == null) return;

        if (!agent.isOnNavMesh)
        {
            if (snapToNavMeshOnStart && !navMeshRecoveryAttempted) TrySnapAgentToNavMesh();
            UpdateAnimator();
            return;
        }

        navMeshRecoveryAttempted = false;
        navMeshUnavailableLogged = false;

        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrol();
                break;
            case AIState.Investigate:
                UpdateInvestigate();
                break;
            case AIState.Flee:
                UpdateFlee();
                break;
            case AIState.PulledToZone:
                // YENİ: Hipnoz altındayken yapay zeka başka bir şey düşünmez, hedefe koşar!
                break; 
        }

        UpdateAnimator();
    }

    public void ReactToEvent(Vector3 eventPosition, bool isAggressive)
    {
        if (agent == null || !agent.isOnNavMesh || currentState == AIState.PulledToZone) return;

        if (isAggressive) EnterFleeState(eventPosition);
        else EnterInvestigateState(eventPosition);
    }

    // YENİ: Yeşil bölgenin AI'yi zorla kendine çekmesini sağlayan komut
    public void ForcePullToZone(Vector3 zonePosition)
    {
        if (currentState == AIState.PulledToZone) return; // Zaten çekiliyorsa tekrar komut verme
        
        currentState = AIState.PulledToZone;
        SetAgentStoppedSafe(false);
        agent.speed = runSpeed; // Hızlıca koşarak hedefe gitsin
        agent.stoppingDistance = 0f; // Tam içine kadar girmesi için mesafeyi sıfırlıyoruz
        agent.SetDestination(zonePosition);
    }

    private void ApplyVisualOffsetOnce()
    {
        if (visualOffsetApplied || Mathf.Approximately(visualYOffset, 0f)) return;

        string[] visibleChildNames = { "Boy01_Body_Geo", "Boy01_Brows_Geo", "Boy01_Eyes_Geo", "h_Geo" };
        for (int i = 0; i < visibleChildNames.Length; i++)
        {
            Transform child = transform.Find(visibleChildNames[i]);
            if (child != null)
            {
                Vector3 localPos = child.localPosition;
                localPos.y += visualYOffset;
                child.localPosition = localPos;
            }
        }
        visualOffsetApplied = true;
    }

    private void CacheWaypointsIfNeeded()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            cachedWaypoints = waypoints;
            return;
        }

        Transform root = transform.Find("Freshman_TestRoute") ?? transform.Find("Freshman_Waypoints");
        if (root == null)
        {
            cachedWaypoints = waypoints;
            return;
        }

        List<Transform> collected = new List<Transform>();
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child != null) collected.Add(child);
        }

        collected.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        cachedWaypoints = collected.ToArray();
    }

    private Transform[] GetWaypoints()
    {
        if (cachedWaypoints != null && cachedWaypoints.Length > 0) return cachedWaypoints;
        CacheWaypointsIfNeeded();
        return cachedWaypoints;
    }

    private void UpdatePatrol()
    {
        Transform[] activeWaypoints = GetWaypoints();
        if (activeWaypoints == null || activeWaypoints.Length == 0)
        {
            SetAgentStoppedSafe(true);
            hasActiveTarget = false;
            return;
        }

        if (!hasActiveTarget)
        {
            GoToCurrentWaypoint(activeWaypoints);
            return;
        }

        if (agent.pathPending) return;

        if (HasReachedDestination(agent.stoppingDistance + 0.05f))
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % activeWaypoints.Length;
            GoToCurrentWaypoint(activeWaypoints);
        }
    }

    private void UpdateInvestigate()
    {
        if (agent.pathPending) return;

        if (HasReachedDestination(investigateStoppingDistance))
        {
            SetAgentStoppedSafe(true);
            investigateTimer -= Time.deltaTime;
            if (investigateTimer <= 0f) EnterPatrolState(false);
        }
    }

    private void UpdateFlee()
    {
        if (agent.pathPending) return;

        if (HasReachedDestination(agent.stoppingDistance + 0.1f))
        {
            fleeTimer -= Time.deltaTime;
            if (fleeTimer <= 0f) EnterPatrolState(false);
        }
    }

    private void GoToCurrentWaypoint(Transform[] activeWaypoints)
    {
        if (activeWaypoints == null || activeWaypoints.Length == 0)
        {
            hasActiveTarget = false;
            return;
        }

        currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, activeWaypoints.Length - 1);
        Transform waypoint = activeWaypoints[currentWaypointIndex];

        if (waypoint == null)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % activeWaypoints.Length;
            hasActiveTarget = false;
            return;
        }

        SetAgentStoppedSafe(false);
        agent.speed = walkSpeed;
        agent.stoppingDistance = 0.1f;
        agent.SetDestination(waypoint.position);
        hasActiveTarget = true;
    }

    private void EnterPatrolState(bool resetWaypointIndex)
    {
        currentState = AIState.Patrol;
        CacheWaypointsIfNeeded();

        Transform[] activeWaypoints = GetWaypoints();
        if (resetWaypointIndex && activeWaypoints != null && activeWaypoints.Length > 0)
        {
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 0, activeWaypoints.Length - 1);
        }

        investigateTimer = 0f;
        fleeTimer = 0f;
        hasActiveTarget = false;

        SetAgentStoppedSafe(false);
        if (agent != null)
        {
            agent.speed = walkSpeed;
            agent.stoppingDistance = 0.1f;
        }

        if (activeWaypoints != null && activeWaypoints.Length > 0) GoToCurrentWaypoint(activeWaypoints);
    }

    private void EnterInvestigateState(Vector3 eventPosition)
    {
        currentState = AIState.Investigate;
        investigateTimer = investigationWaitTime;
        hasActiveTarget = true;

        Vector3 destination = SampleNavMeshPosition(eventPosition);
        investigateTargetPosition = destination;

        SetAgentStoppedSafe(false);
        agent.speed = investigateSpeed;
        agent.stoppingDistance = investigateStoppingDistance;
        agent.SetDestination(investigateTargetPosition);
    }

    private void EnterFleeState(Vector3 eventPosition)
    {
        currentState = AIState.Flee;
        fleeTimer = fleeCooldownTime;
        hasActiveTarget = true;

        Vector3 awayDirection = (transform.position - eventPosition).normalized;
        if (awayDirection.sqrMagnitude < 0.0001f)
        {
            awayDirection = Random.insideUnitSphere;
            awayDirection.y = 0f;
            awayDirection.Normalize();
        }

        Vector3 rawDestination = transform.position + awayDirection * fleeDistance;
        fleeTargetPosition = SampleNavMeshPosition(rawDestination);

        SetAgentStoppedSafe(false);
        agent.speed = runSpeed;
        agent.stoppingDistance = 0.1f;
        agent.SetDestination(fleeTargetPosition);
    }

    private Vector3 SampleNavMeshPosition(Vector3 targetPosition)
    {
        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, navMeshSnapRadius, agent.areaMask))
        {
            return hit.position;
        }
        return targetPosition;
    }

    private void TrySnapAgentToNavMesh()
    {
        if (navMeshRecoveryAttempted || agent == null || agent.isOnNavMesh) return;

        navMeshRecoveryAttempted = true;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, navMeshSnapRadius, agent.areaMask))
        {
            agent.Warp(hit.position);
            agent.ResetPath();
            hasActiveTarget = false;
        }
        else if (!navMeshUnavailableLogged)
        {
            navMeshUnavailableLogged = true;
            Debug.LogWarning($"{name}: NavMesh üzerinde geçerli nokta bulunamadı.");
        }
    }

    private void SetAgentStoppedSafe(bool value)
    {
        if (agent == null || !agent.isOnNavMesh) return;
        agent.isStopped = value;
    }

    private bool HasReachedDestination(float stoppingDistance)
    {
        if (agent.pathPending) return false;
        if (!agent.hasPath) return true;

        float remainingDistance = agent.remainingDistance;
        if (float.IsInfinity(remainingDistance)) return false;

        return remainingDistance <= stoppingDistance && agent.velocity.sqrMagnitude < 0.01f;
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat(SpeedHash, speed);
        if (!string.IsNullOrWhiteSpace(stateParameterName)) animator.SetInteger(stateParameterName, (int)currentState);
    }
}