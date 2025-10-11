using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,       // Wander aimlessly around zone
    TARGET,     // Pathfind towards player
    ATTACK      // Use held item on player
}

public class Enemy : MonoBehaviour
{
    public EnemyZone zone;
    public BaseItem item;

    [Header("Idle")]
    public Vector2 changeDestinationEvery = new Vector2(4, 9);

    // [Header("Target")]
    [HideInInspector] public GameObject targetPlayer;

    EnemyState currentState = EnemyState.IDLE;

    NavMeshAgent navmesh;
    Rigidbody rb;
    MeshRenderer rend;

    float nextPositionChange;
    float nextItemUse;

    [Header("Debug")]
    public Material material_idle;
    public Material material_target;
    public Material material_attack;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
        if (zone) zone.enemy = this;

        ChangeState(currentState);
    }
    
    // Update is called once per frame
    void Update()
    {

        if (currentState is EnemyState.IDLE)
            RunIdleState();

        else if (currentState is EnemyState.TARGET)
            RunTargetState();

        else if (currentState is EnemyState.ATTACK)
            RunAttackState();
    }

    public void ChangeState(EnemyState state)
    {
        currentState = state;

        if (state == EnemyState.IDLE)
        {

            // if the player walks out of the enemy zone while in the middle of the jump, it will become idle, and want to reset its path, 
            // but if it's mid-air, resetting the path will cause issues.
             if (navmesh != null && IsGrounded())
            {
                navmesh.ResetPath();
            }
            else
            {
                Debug.Log("Enemy.ChangeState: skipping ResetPath because enemy is not grounded.");
            }

            if (material_idle != null) rend.material = material_idle;
        }

        else if (state == EnemyState.TARGET)
        {
            ScheduleNextItemUse();
            if (material_target != null) rend.material = material_target;
        }

        else if (state == EnemyState.ATTACK)
        {
            if (material_attack != null) rend.material = material_attack;
        }
    }

    // Navmesh requires RB to be disabled, so toggle it for physics effects
    public void ToggleRigidbody(bool enabled)
    {
        navmesh.enabled = !enabled;
        rb.isKinematic = !enabled;
    }

    // Temporarily switch to rigidbody then back to navmesh
    public IEnumerator SwitchToRigidbody(float time)
    {
        ToggleRigidbody(true);
        yield return new WaitForSeconds(time);
        yield return new WaitUntil(() => IsGrounded());
        if (rb != null)
            rb.isKinematic = true;
            

        ToggleRigidbody(false);
    }

    public void PathfindTo(Vector3 targetPos)
    {
        if (navmesh != null && navmesh.enabled && navmesh.isOnNavMesh)
            navmesh.SetDestination(targetPos);
    }

    // == STATES //

    // Idle state
    void RunIdleState()
    {
        if (zone != null && Time.time > nextPositionChange)
        {
            ScheduleNextIdleMovement();

            var targetPos = zone.GetRandomPoint();
            // print($"Picked random point: {targetPos}");

            // Check if position is walkable
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 64, NavMesh.AllAreas))
            {
                // print("Heading to destination!");
                PathfindTo(targetPos);
            }
        }
    }

    // Target state
    void RunTargetState()
    {
        if (!targetPlayer) return;

        // Target player directly
        PathfindTo(targetPlayer.transform.position);

        // If holding an item, use it sometimes
        if (item != null && Time.time > nextItemUse)
        {
            ScheduleNextItemUse();
            item.Use();
        }
    }

    // Attack state
    void RunAttackState()
    {

    }


    // Schedule the next time the enemy will aimlessly wander
    void ScheduleNextIdleMovement()
    {
        nextPositionChange = Time.time + Random.Range(changeDestinationEvery.x, changeDestinationEvery.y);
    }

    // Schedule the next time the enemy will use their item
    void ScheduleNextItemUse()
    {
        if (item != null)
        {
            nextItemUse = Time.time + Random.Range(item.enemyUseInterval.x, item.enemyUseInterval.y);
            print($"Will use item in {nextItemUse - Time.time} secs");
        }
    }
    
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
