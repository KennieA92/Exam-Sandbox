
using System;
using System.Collections;
using GAME.Combat;
using GAME.Core;
using GAME.Movement;
using UnityEngine;
using UnityEngine.AI;

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
        [SerializeField] GameObject goal;
        BanditFighter fighter;
        Health health;
        Mover mover;
        //Bandits Momory
        GameObject[] livestocks;
        GameObject closeLivestock = null;
        GameObject player;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;
        bool handsFree = true;
        Vector3 guardPosition;

        private void Start()
        {
            StartCoroutine(FindLivestock());
            fighter = GetComponent<BanditFighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
        }
        private void Update()
        {
            if (health.IsDead()) return;

            if (!handsFree)
            {
                FleeBehaviour();

            }
            else if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                Debug.Log("Found Player");
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                Debug.Log("Suspicious");
                SuspicionBehaviour();
            }
            else if (InRangeOfSheep() && handsFree)
            {
                StealSheep();
            }

            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void FleeBehaviour()
        {
            if (goal == null) return;
            if (closeLivestock == null) return;
            mover.MoveTo(goal.transform.position, 1f);
        }

        IEnumerator FindLivestock()
        {
            while (true)
            {
                livestocks = GameObject.FindGameObjectsWithTag("Sheep");
                yield return new WaitForSeconds(1f);
            }
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

        private bool InRangeOfSheep()
        {
            if (livestocks == null) return false;
            float distanceToLivestock = 30f;
            for (int i = 0; i < livestocks.Length; i++)
            {
                if (!livestocks[i]) continue;
                distanceToLivestock = Vector3.Distance(livestocks[i].transform.position, transform.position);
                if (distanceToLivestock < chaseDistance)
                {
                    if (closeLivestock == null)
                    {
                        closeLivestock = livestocks[i];
                    }
                    // Currently not working properly. 
                    //if (Vector3.Distance(closeLivestock.transform.position, transform.position) > distanceToLivestock)
                    //{
                    //    closeLivestock = livestocks[i];
                    //    Debug.Log("New Sheep!" + closeLivestock);
                    //}
                    return distanceToLivestock < chaseDistance;
                }
            }
            return false;
        }

        private void StealSheep()
        {
            if (closeLivestock == null) return;
            if (Vector3.Distance(transform.position, closeLivestock.transform.position) < 2f)
            {
                closeLivestock.GetComponent<Health>().TakeDamage(41143413f);
                closeLivestock.GetComponent<Rigidbody>().isKinematic = true;
                closeLivestock.GetComponent<NavMeshAgent>().enabled = false;
                closeLivestock.transform.SetParent(transform);
                closeLivestock.transform.localPosition = new Vector3(0.65f, 1.9f, 0);

                handsFree = false;
            }
            else
            {
                mover.StartMoveAction(closeLivestock.transform.position, 1f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}