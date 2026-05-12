using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    // ── AI SETTINGS ──────────────────────────────────────────────
    public float patrolRadius    = 15f;   // How far from spawn to wander
    public float idleTime        = 2f;    // Seconds to wait at each patrol point
    public float attackDistance  = 5f;    // Distance to stop and attack
    public float maxVisionDistance = 20f; // Max detection range
    public float positionThreshold = 1.5f;

    // ── LEGACY - kept for compatibility but no longer used ───────
    public Transform[] patrolPoints;
    public int currentPointIndex = 0;
    public Vector3 currentTarget;

    // ── REFERENCES ───────────────────────────────────────────────
    private NavMeshAgent agent;
    private Transform playerTransform;

    // ── SPAWN INFO ───────────────────────────────────────────────
    private Vector3 spawnPosition; // Center of this enemy's patrol zone

    // ── VISION STATE ─────────────────────────────────────────────
    private bool canSeePlayer;
    private Vector3 lastKnownPlayerPosition;

    // ── IDLE TIMER ───────────────────────────────────────────────
    private float idleTimer;
    private bool waitingAtPoint = false;

    // ── STATE MACHINE ────────────────────────────────────────────
    public enum State { Idle, Patrolling, Chasing, Attacking }
    public State state = State.Patrolling;

    // ── UNITY METHODS ────────────────────────────────────────────

    void Start()
    {
        agent         = GetComponent<NavMeshAgent>();
        spawnPosition = transform.position;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError("Player not found!");

        // Pick first random patrol destination
        SetNewPatrolDestination();
    }

    void Update()
    {
        if (playerTransform == null) return;

        LookForPlayer();

        switch (state)
        {
            case State.Idle:      Idle();      break;
            case State.Patrolling: Patrolling(); break;
            case State.Chasing:   Chasing();   break;
            case State.Attacking: Attacking(); break;
        }


        LookAtPlayer();
        SetLastKnownPlayerPosition();
    }

    // ── STATES ───────────────────────────────────────────────────

    void Idle()
    {
        agent.ResetPath();
        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            waitingAtPoint = false;
            state = State.Patrolling;
            SetNewPatrolDestination();
        }
    }

    void Patrolling()
    {
        // Only set destination when agent has no active path
        if (!agent.pathPending && !agent.hasPath)
            SetNewPatrolDestination();

        // Check if close enough to destination
        if (!agent.pathPending && agent.remainingDistance <= positionThreshold)
        {
            // Brief idle pause at each point
            state     = State.Idle;
            idleTimer = idleTime;
        }
    }

    void Chasing()
    {
        agent.SetDestination(lastKnownPlayerPosition);

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distToPlayer <= attackDistance && canSeePlayer)
        {
            state = State.Attacking;
        }
        else if (distToPlayer > maxVisionDistance)
        {
            state = State.Patrolling;
            SetNewPatrolDestination();
        }
        else if (Vector3.Distance(transform.position, lastKnownPlayerPosition) < positionThreshold && !canSeePlayer)
        {
            state = State.Patrolling;
            SetNewPatrolDestination();
        }
    }

    void Attacking()
    {
        agent.ResetPath();

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distToPlayer > attackDistance || !canSeePlayer)
            state = State.Chasing;
    }

    // ── HELPERS ──────────────────────────────────────────────────

    void SetNewPatrolDestination()
    {
        // Try up to 10 times to find a valid NavMesh point within patrol radius
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidate, out hit, 3f, NavMesh.AllAreas))
            {
                currentTarget = hit.position;
                agent.SetDestination(currentTarget);
                return;
            }
        }

        // Fallback: go back to spawn
        agent.SetDestination(spawnPosition);
    }

void LookForPlayer()
    {
        Vector3 dirToPlayer = playerTransform.position - transform.position;
        float distToPlayer = dirToPlayer.magnitude;

        // Raycast toward player - if anything blocks it (wall, obstacle) enemy can't see player
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, out RaycastHit hit, maxVisionDistance))
        {
            if (hit.transform == playerTransform)
            {
                canSeePlayer = true;
                if (state == State.Patrolling || state == State.Idle)
                    state = State.Chasing;
            }
            else
            {
                // Something is blocking line of sight
                canSeePlayer = false;
            }
        }
        else
        {
            canSeePlayer = false;
        }
    }

    void LookAtPlayer()
    {
        if (!canSeePlayer) return;
        transform.LookAt(new Vector3(
            playerTransform.position.x,
            transform.position.y,
            playerTransform.position.z));
    }

    void SetLastKnownPlayerPosition()
    {
        if (canSeePlayer)
            lastKnownPlayerPosition = playerTransform.position;
    }
}