
using System;
using GAME.Combat;
using GAME.Core;
using GAME.Movement;
using UnityEngine;

namespace GAME.Control
{


    public class BanditController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 20f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float timeDwelling = 2f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        BanditFighter fighter;
        Health health;
        Mover mover;
        //Guards Momory
        GameObject player;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        Vector3 guardPosition;

        private void Start()
        {
            fighter = GetComponent<BanditFighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;
            Debug.Log("Not Dead");
            Debug.Log(fighter.CanAttack(player));
            Debug.Log(InAttackRangeOfPlayer());

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                Debug.Log("Found Player");
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                Debug.Log("Suspicious");
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPoisition = guardPosition;
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPoisition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > timeDwelling)
            {
                mover.StartMoveAction(nextPoisition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }
        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < wayPointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            Debug.Log("Attack Behav");
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            print(distanceToPlayer);
            return distanceToPlayer < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}