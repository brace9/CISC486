using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyZone zone;

    [Header("Idle")]
    public Vector2 changeDestinationEvery = new Vector2(4, 9);

    State currentState;
    public NavMeshAgent navmesh;
    public GameObject player;
    float nextPositionChange;

    float lastTimeSawPlayer = -999f;
    public float loseSightDelay = 0.6f; // seconds the enemy will keep chasing after losing sight
    // i have no clue how the state machine is supposed to work so i'm doing this for now
    public string testState = "target";

    void Awake()
    {
        navmesh = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player");

        if (zone != null)
            zone.enemy = this;

    }

    void Start()
    {
        //set initial state
        currentState = GetComponent<IdleState>();
        ScheduleNextIdleMovement();
    }

    // Update is called once per frame
    void Update()
    {
        RunStateMachine();

        // if (currentState is IdleState)
        /*
        if (testState == "idle")
        {
            // Pick new position and navigate there
            if (Time.time > nextPositionChange)
            {
                ScheduleNextIdleMovement();

                var targetPos = zone.GetRandomPoint();
                print($"Picked random point: {targetPos}");

                // Check if position is walkable
                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 64, NavMesh.AllAreas))
                {
                    print("Heading to destination!");
                    navmesh.SetDestination(targetPos);
                }
            }
        }

        if (testState == "target")
        {
            navmesh.SetDestination(player.transform.position);
        }
        */

        if (currentState is IdleState)
        {
            HandleIdleMovement();
        }
        else if (currentState is TargetState)
        {
            HandleChaseMovement();
        }
        else if (currentState is AttackState)
        {
            HandleAttack();
        }
    }

    private void RunStateMachine()
    {
            if (currentState == null)
    {
        // try to recover to idle
        currentState = GetComponent<IdleState>();
        if (currentState == null)
        {
            Debug.LogWarning("[Enemy] No current state and no IdleState component found.");
            return;
        }
    }

    // run state to get next state
    State nextState = currentState.RunCurrentState();

    // defensive: if a state returns null, log and stay in current
    if (nextState == null)
    {
        Debug.LogWarning($"[Enemy] {currentState.GetType().Name} returned null nextState. Staying in current state.");
        return;
    }

    // only switch when nextState is different
    if (nextState != currentState)
    {
        Debug.Log($"[Enemy] Switching state {currentState.GetType().Name} -> {nextState.GetType().Name}");
        SwitchToNextState(nextState);
    }
    }

    private void SwitchToNextState(State nextState)
    {
        currentState = nextState;

        if (currentState is IdleState)
            ScheduleNextIdleMovement();
    }

    // schedule the next time the enemy will aimlessly wander
    void ScheduleNextIdleMovement()
    {
        nextPositionChange = Time.time + Random.Range(changeDestinationEvery.x, changeDestinationEvery.y);
    }


    private void HandleIdleMovement()
    {
        if (Time.time > nextPositionChange)
        {
            ScheduleNextIdleMovement();

            var targetPos = zone.GetRandomPoint();
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 64, NavMesh.AllAreas))
            {
                navmesh.SetDestination(hit.position);
            }
        }
    }

    private void HandleChaseMovement()
    {
        if (player != null)
            navmesh.SetDestination(player.transform.position);
    }

    private void HandleAttack()
    {
        // If the current attack state is performing a dash, don't clear the navmesh path
        var attack = GetComponent<AttackState>();
        if (attack != null && attack.IsDashing)
        {
            // during dash, let the agent continue to its dash target
            return;
        }

        navmesh.ResetPath(); // stop moving while attacking
        // You could trigger attack animation or deal damage here
    }

    // Called by states to perform a wind-up dash attack
    public IEnumerator DoDash(float windUp, float dashSpeedMultiplier, float dashDuration, float liftHeight, float dashDistance, System.Action onComplete)
{
    if (navmesh == null)
    {
        Debug.LogWarning("Enemy.DoDash: no NavMeshAgent found");
        onComplete?.Invoke();
        yield break;
    }

    // --- WIND-UP PHASE ---
    if (windUp > 0f)
    {
        yield return new WaitForSeconds(windUp);
    }

    // --- DASH PHASE ---
    float originalSpeed = navmesh.speed;
    float originalBaseOffset = navmesh.baseOffset;
    float originalAcceleration = navmesh.acceleration;
    bool originalAutoBraking = navmesh.autoBraking;

    navmesh.autoBraking = false;
    navmesh.acceleration = Mathf.Max(originalAcceleration, 50f);
    navmesh.speed = originalSpeed * dashSpeedMultiplier;

    // Calculate dash direction & target
    Vector3 dashDir;
    if (player != null)
        dashDir = (player.transform.position - transform.position).normalized;
    else
        dashDir = transform.forward;

    Vector3 dashTarget = transform.position + dashDir * dashDistance;

    float t = 0f;
    Vector3 startPos = transform.position;

    while (t < dashDuration)
    {
        float progress = Mathf.Clamp01(t / dashDuration);

        // Vertical arc (sinusoidal)
        navmesh.baseOffset = originalBaseOffset + Mathf.Sin(progress * Mathf.PI) * liftHeight;

        // Move manually towards dash target
        Vector3 nextPos = Vector3.Lerp(startPos, dashTarget, progress);
        navmesh.Move(nextPos - transform.position);

        t += Time.deltaTime;
        yield return null;
    }

    // Restore NavMeshAgent settings
    navmesh.speed = originalSpeed;
    navmesh.baseOffset = originalBaseOffset;
    navmesh.acceleration = originalAcceleration;
    navmesh.autoBraking = originalAutoBraking;

    onComplete?.Invoke();
}



    public bool CanSeePlayer(float viewDistance = 15f)
    {
            if (player == null) return false;

    float dist = Vector3.Distance(transform.position, player.transform.position);
    bool inDistance = dist < viewDistance;

    if (inDistance)
    {
        // can see player
        // if we want walls to block sight, do a raycast here, doens't accoutn for that rn
        lastTimeSawPlayer = Time.time;
        return true;
    }

    // still consider player "seen" for a short time after losing distance to avoid flicker
    return (Time.time - lastTimeSawPlayer) < loseSightDelay;
    }

    public bool InAttackRange(float attackRange = 2.5f)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.transform.position) < attackRange;
    }
}
