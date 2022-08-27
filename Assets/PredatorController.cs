using System;
using System.Collections;
using GAME.Core;
using GAME.Movement;
using RPG.Combat;
using UnityEngine;

namespace GAME.Control
{


    public class PredatorController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 30f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float timeDwelling = 2f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        Fighter fighter;
        Health health;
        Mover mover;
        //Guards Momory
        GameObject[] sheep;
        GameObject closeSheep = null;
        GameObject player = null;
        float timeSinceLastSawSheep = Mathf.Infinity;
        float timeSinceLastAttacked = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float chargeTime = 0f;

        Vector3 guardPosition;

        private void Start()
        {
            StartCoroutine(FindSheep());
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
            player = GameObject.FindWithTag("Player");
        }

        IEnumerator FindSheep()
        {
            yield return new WaitForSeconds(10f);

            sheep = GameObject.FindGameObjectsWithTag("Sheep");
            chargeTime += Time.deltaTime * 1;
            Debug.Log("Finding Sheep" + sheep.Length);

        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (timeSinceLastAttacked < aggroTime)
            {
                timeSinceLastAttacked = 0;
                AttactBehaviour(player);
            }

            else if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                AttactBehaviour(player);
            }

            else if (InAttackRangeOfSheep() && fighter.CanAttack(closeSheep))
            {
                timeSinceLastSawSheep = 0;
                AttactBehaviour(closeSheep);
            }
            else if (timeSinceLastSawSheep < suspicionTime)
            {
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
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawSheep += Time.deltaTime;
            timeSinceLastAttacked += Time.deltaTime;
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

        private void AttactBehaviour(GameObject target)
        {
            //Fix this    
            if (target == null) return;
            fighter.Attack(target);
        }

        private bool InAttackRangeOfPlayer()
        {
            if (player == null) return false;
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < (chaseDistance / 2);
        }

        private bool InAttackRangeOfSheep()
        {
            if (sheep == null) return false;
            float distanceToSheep = 30f;
            for (int i = 0; i < sheep.Length; i++)
            {
                if (!sheep[i]) continue;
                distanceToSheep = Vector3.Distance(sheep[i].transform.position, transform.position);
                if (distanceToSheep < chaseDistance)
                {
                    closeSheep = sheep[i];
                    Debug.Log("Sheep!");
                    return distanceToSheep < chaseDistance;
                }
            }
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}