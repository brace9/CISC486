using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,       // Wander aimlessly around zone
    TARGET,     // Pathfind towards player
    FLEE,       // Run away from player
    ATTACK      // Use held item on player
}

public class Enemy : MonoBehaviour, IDamageable
{
    public float hp = 5;
    public float fleeHP = 1;

    public EnemyZone zone;
    public BaseItem item;
    public GameObject droppedItem;


    [Header("Attacking")]
    public float attackRange = 2f;
    public int attackDamage = 1;
    public float attackInterval = 1f;

    private float nextAttackTime;

    [Header("Idle")]
    public Vector2 changeIdleDestinationEvery = new Vector2(4, 9);

    [Header("Target")]
    public GameObject targetPlayer;

    [Header("Flee")]
    public Vector2 changeFleeDestinationEvery = new Vector2(0.5f, 1.0f);
    public float playerSearchRadius = 25.0f;
    public float fleeSpeedMult = 2.0f;
    public float fleeDistance = 10.0f;

    EnemyState currentState = EnemyState.IDLE;

    NavMeshAgent navmesh;
    Rigidbody rb;
    MeshRenderer rend;

    float baseSpeed;
    float nextPositionChange;
    float nextItemUse;

    [Header("Debug")]
    public Material material_idle;
    public Material material_target;
    public Material material_attack;
    public Material material_flee;

    void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
        if (zone) zone.enemy = this;

        baseSpeed = navmesh.speed;

        ChangeState(currentState);
    }

    // Update is called once per frame
    void Update()
    {

        if (currentState is EnemyState.IDLE)
            RunIdleState();

        else if (currentState is EnemyState.TARGET)
            RunTargetState();

        else if (currentState is EnemyState.FLEE)
            RunFleeState();

        else if (currentState is EnemyState.ATTACK)
            RunAttackState();


        // debug key to test fleeing
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeState(EnemyState.FLEE);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        print("took damage! hp left: " + hp);

        if (hp <= 0)
        {
            OnDefeated();
        }

        else if (hp <= fleeHP)
        {
            ChangeState(EnemyState.FLEE);
        }
    }

    // IDamageable implementation - allow callers to include the damage source if available.
    public void TakeDamage(float damage, GameObject source)
    {
        // For now, we simply forward to the single-arg TakeDamage implementation.
        // Source can be used for knockback, scoring, or attribution in the future.
        TakeDamage(damage);
    }

    public void OnDefeated()
    {
        if (droppedItem != null && item != null)
        {
            var drop = Instantiate(droppedItem, transform.position, Quaternion.identity);
            drop.GetComponent<ItemPickup>().item = item;
        }

        Destroy(zone.gameObject);
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
                nextPositionChange = 0;
            }
            else
            {
                Debug.Log("Enemy.ChangeState: skipping ResetPath because enemy is not grounded.");
            }

            if (material_idle != null) rend.material = material_idle;
        }

        else if (state == EnemyState.FLEE)
        {
            nextPositionChange = 0;
            if (material_flee != null) rend.material = material_flee;
        }

        else if (state == EnemyState.TARGET)
        {
            ScheduleNextItemUse();
            if (material_target != null) rend.material = material_target;
        }

        else if (state == EnemyState.ATTACK)
        {
            nextAttackTime = Time.time + attackInterval;
            if (material_attack != null) rend.material = material_attack;
        }

        navmesh.speed = baseSpeed * (IsFleeing() ? fleeSpeedMult : 1);
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
            ScheduleNextMovement();

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

    // Flee state
    void RunFleeState()
    {
        if (Time.time > nextPositionChange)
        {
            ScheduleNextMovement();

            Transform fleeFrom = FindNearestPlayer(playerSearchRadius);
            if (fleeFrom == null) return;

            Vector3 fleeDir = (transform.position - fleeFrom.position).normalized; // opposite direction of player
            Vector3 fleeTo = transform.position + (fleeDir * fleeDistance); // flee to this point

            // test if flee position is valid
            if (NavMesh.SamplePosition(fleeTo, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            {
                navmesh.SetDestination(hit.position);
            }
            // otherwise just pick a decent nearby position
            else
            {
                Vector3 randomFlee = (Random.insideUnitSphere * fleeDistance) + transform.position;
                if (NavMesh.SamplePosition(randomFlee, out NavMeshHit hit2, 4f, NavMesh.AllAreas))
                {
                    navmesh.SetDestination(hit.position);
                }
            }
        }
    }

    // Target state
    void RunTargetState()
    {
        if (!targetPlayer) return;

        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= attackRange)
        {
            ChangeState(EnemyState.ATTACK);
        }

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
        if (!targetPlayer)
        {
            ChangeState(EnemyState.IDLE);
            return;
        }

        // if player is too far, go back to targeting
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) > attackRange * 1.5f)
        {
            ChangeState(EnemyState.TARGET);
        }

        // Face the player
        Vector3 lookDir = (targetPlayer.transform.position - transform.position);
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);
        }

        // Attack on interval
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackInterval;
            PerformAttack();
            //print("Attacked!");
        }

    }

    void PerformAttack()
    {
        if (!targetPlayer) return;

        Player player = targetPlayer.GetComponentInParent<Player>();
        if (player != null)
        {
            player.TakeDamage(attackDamage);

            // Knockback
            Vector3 knockbackDir = (targetPlayer.transform.position - transform.position).normalized;
            knockbackDir.y = 0.25f;
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                //set velocity to 0 before knocking back
                playerRb.velocity = new Vector3(0f, 0f, 0f);
                playerRb.AddForce(knockbackDir * 50f, ForceMode.Impulse);
            }
        }
        
    }


    // Schedule the next time the enemy's pathfinding should update
    void ScheduleNextMovement()
    {
        var timer = IsFleeing() ? changeFleeDestinationEvery : changeIdleDestinationEvery;
        nextPositionChange = Time.time + Random.Range(timer.x, timer.y);
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

    Transform FindNearestPlayer(float radius)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");
        float maxDist = radius;
        Transform closestPlayer = null;

        // Search all Player objects and find the closest one
        foreach (var p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < maxDist)
            {
                maxDist = dist;
                closestPlayer = p.transform;
            }
        }

        return closestPlayer;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    public bool IsFleeing()
    {
        return currentState is EnemyState.FLEE;
    }
}
