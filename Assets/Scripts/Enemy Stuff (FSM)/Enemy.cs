using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyZone zone;

    [Header("Idle")]
    public Vector2 changeDestinationEvery = new Vector2(4, 9);

    State currentState;
    NavMeshAgent navmesh;
    GameObject player;
    float nextPositionChange;

    // i have no clue how the state machine is supposed to work so i'm doing this for now
    public string testState = "target";

    void Awake()
    {
        navmesh = GetComponent<NavMeshAgent>();
        zone.enemy = this;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        RunStateMachine();

        // if (currentState is IdleState)
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
    }

    private void RunStateMachine()
    {
        State nextState = currentState?.RunCurrentState();

        if (nextState != null)
        {
            //switch to next state
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
}
