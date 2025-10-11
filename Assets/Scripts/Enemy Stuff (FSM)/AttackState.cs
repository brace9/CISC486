using System.Collections;
using UnityEngine;

public class AttackState : State
{
    public TargetState targetState;
    private Enemy enemy;

    public float windUpSeconds = 0.4f;
    public float dashMultiplier = 3f;
    public float dashDuration = 0.4f;
    public float jumpHeight = 0.4f;
    public float preAttackDistance = 6f;
    public float dashDistance = 10f;
    public float moveToAttackSpeed = 3f;
    public float postDashCooldown = 0.3f; // small pause after dash

    private bool isExecuting = false;
    private bool attackFinished = false;
    public bool IsDashing => isExecuting;

    void Awake() => enemy = GetComponent<Enemy>();

    public override State RunCurrentState()
    {
        // if the attack routine finished, transition back to targetState
        if (attackFinished)
        {
            attackFinished = false;
            return targetState;
        }

        // Only start the attack routine if it’s not already running
        if (!isExecuting)
        {
            enemy.StartCoroutine(AttackRoutine());
        }

        // Stay in attack state until routine finishes
        return this;
    }

    private IEnumerator AttackRoutine()
    {
        isExecuting = true;

        if (enemy.player == null)
        {
            isExecuting = false;
            yield break;
        }

        // --- Phase 1: Walk to pre-attack position ---
    Vector3 toPlayer = (enemy.player.transform.position - enemy.transform.position).normalized;
    Vector3 attackPos = enemy.player.transform.position - toPlayer * preAttackDistance;

        enemy.navmesh.speed = moveToAttackSpeed;
        enemy.navmesh.SetDestination(attackPos);

        Debug.Log($"[AttackState] Moving to attackPos {attackPos} from {enemy.transform.position}");

        // wait until the agent has path and reaches close to the attackPos
        while (enemy.navmesh.pathPending || enemy.navmesh.remainingDistance > Mathf.Max(0.2f, enemy.navmesh.stoppingDistance))
        {
            yield return null;
        }

        Debug.Log("[AttackState] Reached pre-attack position");

        // --- Phase 2: Wind-up ---
        if (windUpSeconds > 0f)
            yield return new WaitForSeconds(windUpSeconds);

        // --- Phase 3: Dash ---
        float originalSpeed = enemy.navmesh.speed;
        float originalOffset = enemy.navmesh.baseOffset;
        float originalAccel = enemy.navmesh.acceleration;
        bool originalAutoBraking = enemy.navmesh.autoBraking;

        enemy.navmesh.autoBraking = false;
        enemy.navmesh.acceleration = Mathf.Max(originalAccel, 50f);
        enemy.navmesh.speed = originalSpeed * dashMultiplier;

    Vector3 dashDir = (enemy.player.transform.position - enemy.transform.position).normalized;
        Vector3 dashTarget = enemy.transform.position + dashDir * dashDistance;
        Vector3 startPos = enemy.transform.position;

        float t = 0f;
        while (t < dashDuration)
        {
            float progress = t / dashDuration;
            enemy.navmesh.baseOffset = originalOffset + Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            // move manually
            Vector3 nextPos = Vector3.Lerp(startPos, dashTarget, progress);
            enemy.navmesh.Move(nextPos - enemy.transform.position);

            t += Time.deltaTime;
            yield return null;
        }

        // --- Phase 4: Post-dash cooldown ---
        enemy.navmesh.speed = originalSpeed;
        enemy.navmesh.baseOffset = originalOffset;
        enemy.navmesh.acceleration = originalAccel;
        enemy.navmesh.autoBraking = originalAutoBraking;

        yield return new WaitForSeconds(postDashCooldown);

    // Finished attack, mark finished so RunCurrentState will return the next state
    isExecuting = false;
    attackFinished = true;
    }
}