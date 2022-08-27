using System;
using GAME.Core;
using GAME.Movement;
using RPG.Combat;
using UnityEngine;

namespace GAME.Control
{


    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float playerDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float timeDwelling = 2f;
        [SerializeField] bool isAggressive = false;
        [SerializeField] float fleeDistance = 20f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        Fighter fighter;
        Health health;
        Mover mover;
        //Guards Momory
        GameObject player;
        GameObject[] predators;
        GameObject closePredator = null;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastSawPredator = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        Vector3 guardPosition;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();
            predators = GameObject.FindGameObjectsWithTag("Predator");
            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;

            if (isAggressive)
            {
                if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
                {
                    AttactBehaviour();
                }
                else if (timeSinceLastSawPlayer < suspicionTime)
                {
                    SuspicionBehaviour();

                }

            }
            else if (!isAggressive && InRangeOfPredator())
            {
                FleeBehaviour();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && InRangeOfPlayer())
            {
                timeSinceArrivedAtWaypoint = timeDwelling + 1;
            }
            else if (timeSinceLastSawPredator < suspicionTime)
            {
                SuspicionBehaviour();
            }

            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void FleeBehaviour()
        {
            if (!closePredator) return;
            /* Vector3 dir = transform.position - closePredator.transform.position;
            */


            Vector3 runTo = transform.position + ((transform.position - closePredator.transform.position));
            Debug.Log("Afraid");
            mover.StartMoveAction(runTo, 1f);
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawPredator += Time.deltaTime;
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

        private void AttactBehaviour()
        {

            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        private bool InRangeOfPredator()
        {
            float distanceToPredator = 30f;
            for (int i = 0; i < predators.Length; i++)
            {
                if (predators[i] == null) continue;
                distanceToPredator = Vector3.Distance(predators[i].transform.position, transform.position);
                if (distanceToPredator < fleeDistance)
                {
                    closePredator = predators[i];
                }
            }
            return distanceToPredator < fleeDistance;
        }

        private bool InRangeOfPlayer()
        {
            if (player == null) return false;
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < playerDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}