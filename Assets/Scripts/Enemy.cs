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
    public State state = State.Patrolling; // Start patrolling immediately


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

        // Only zero velocity when agent isn't navigating to avoid fighting NavMeshAgent
        if (!agent.hasPath || agent.isStopped)
            rb.linearVelocity = Vector3.zero;

        // Face player and track last known position
        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }


void Idle()
    {
        agent.ResetPath();

        idleTimeCounter -= Time.deltaTime;

        if (idleTimeCounter <= 0)
        {
            state = State.Patrolling;
            idleTimeCounter = idleTime;
            // Resume patrolling from next point
            agent.SetDestination(currentTarget);
        }
    }

void Patrolling()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        // Only call SetDestination when not already calculating — calling every frame
        // resets the path calculation and causes pathPending to stay True forever
        if (!agent.pathPending && !agent.hasPath)
            agent.SetDestination(currentTarget);

        // Check if close enough to advance (XZ only to ignore Y differences)
        float dist = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(currentTarget.x, 0f, currentTarget.z));

        if (dist < positionThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            currentTarget = patrolPoints[currentPointIndex].position;
            agent.SetDestination(currentTarget);
        }
    }

void Chasing()
    {
        idleTimeCounter = idleTime;

        agent.SetDestination(lastKnownPlayerPosition);

        if (Vector3.Distance(transform.position, playerTransform.position) <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) > maxVisionDistance)
        {
            // Lost player - go back to patrolling not idle
            state = State.Patrolling;
            if (patrolPoints != null && patrolPoints.Length > 0)
                agent.SetDestination(currentTarget);
        }
        else if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < positionThreshold && !canSeePlayer)
        {
            // Reached last known position but lost sight - resume patrol
            state = State.Patrolling;
            if (patrolPoints != null && patrolPoints.Length > 0)
                agent.SetDestination(currentTarget);
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