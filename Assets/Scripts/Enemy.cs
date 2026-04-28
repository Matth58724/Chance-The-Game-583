using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    // AI SETTINGS
    public int currentPointIndex = 0;        // Index of current patrol point
    public Vector3 currentTarget;            // World position enemy is moving toward
    public float positionThreshold = 2f;     // Distance to consider patrol point reached
    public float idleTime = 5f;              // Seconds to idle before patrolling again
    public float attackDistance = 5f;        // Distance at which enemy stops and attacks
    public float maxVisionDistance = 20f;    // Max distance enemy can detect player

    public Transform[] patrolPoints;         // Patrol points assigned in Inspector

    // REFERENCES
    private NavMeshAgent agent;
    private Rigidbody rb;
    private Transform playerTransform;

    // VISION STATE
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition;

    // IDLE TIMER
    private float idleTimeCounter;

    // STATE MACHINE
    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Patrolling;


    void Start()
    {
        // Cache components
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

        // Start idle timer at full
        idleTimeCounter = idleTime;

        // Find player by tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("Player not found! Make sure Player is tagged Player.");

        // Set first patrol target if points are assigned
        if (patrolPoints.Length > 0)
            currentTarget = patrolPoints[0].position;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Check line of sight to player every frame
        LookForPlayer();

        // Run current state behavior
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrolling:
                Patrolling();
                break;
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
        }

        // Prevent rigidbody from fighting NavMesh Agent movement
        rb.linearVelocity = Vector3.zero;

        // Face player and track last known position
        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }


    void Idle()
    {
        // Stand still
        agent.ResetPath();

        // Count down idle timer
        idleTimeCounter -= Time.deltaTime;

        // Switch to patrolling when timer expires
        if (idleTimeCounter < 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
        }
    }

    void Patrolling()
    {
        if (patrolPoints.Length == 0) return;

        // Initialize current target if not set yet
        if (currentTarget == Vector3.zero)
            currentTarget = patrolPoints[0].position;

        // Check if close enough to current patrol target
        if (Vector3.Distance(currentTarget, transform.position) < positionThreshold)
        {
            // 10% chance to idle at this point
            float chance = Random.Range(0f, 100f);
            if (chance < 10f)
            {
                state = State.Idle;
                return;
            }

            // Advance to next patrol point looping back to start
            currentPointIndex++;
            currentTarget = patrolPoints[currentPointIndex % patrolPoints.Length].position;
        }
        else
        {
            // Keep moving toward current patrol target
            agent.SetDestination(currentTarget);
        }
    }

    void Chasing()
    {
        // Reset idle timer in case we transition to idle after
        idleTimeCounter = idleTime;

        // Move toward last known player position
        agent.SetDestination(lastKnownPlayerPosition);

        // Attack if close enough and has line of sight
        if (Vector3.Distance(transform.position, playerTransform.position)
                 <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        // Give up chase if player is too far
        else if (Vector3.Distance(transform.position, playerTransform.position)
                 > maxVisionDistance)
        {
            state = State.Patrolling;
        }
        // Give up if reached last known position but lost sight
        else if (Vector3.Distance(transform.position, lastKnownPlayerPosition)
                 < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
        }
    }

    void Attacking()
    {
        // Reset idle timer
        idleTimeCounter = idleTime;

        // Stand still while attacking
        agent.ResetPath();

        // If player moved out of range or out of sight switch to chasing
        if (Vector3.Distance(transform.position, playerTransform.position)
            > attackDistance || !canSeePlayer)
        {
            state = State.Chasing;
        }
    }

    // VISION

    void LookForPlayer()
    {
        // Direction from enemy to player
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // Cast ray toward player up to max vision distance
        if (Physics.Raycast(transform.position, directionToPlayer,
            out RaycastHit hit, maxVisionDistance))
        {
            // Can see player only if ray hits player not a wall
            canSeePlayer = hit.transform == playerTransform;

            // Start chasing if spotted and not already attacking
            if (canSeePlayer && state != State.Attacking)
                state = State.Chasing;
        }
        else
        {
            canSeePlayer = false;
        }
    }

    void LookAtPlayer()
    {
        if (!canSeePlayer) return;

        // Look at player keeping Y level to avoid tilting up or down
        transform.LookAt(new Vector3(
            playerTransform.position.x,
            transform.position.y,
            playerTransform.position.z
        ));
    }

    void SetLastKnownPlayerPosition()
    {
        // Only update when enemy has line of sight to player
        if (canSeePlayer)
            lastKnownPlayerPosition = playerTransform.position;
    }
}