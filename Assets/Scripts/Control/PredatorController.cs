using System;
using System.Collections;
using GAME.Core;
using GAME.Movement;
using GAME.Combat;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GAME.Control
{


    public class PredatorController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 30f;
        [SerializeField] float aggroTime = 3f;
        [SerializeField] float roamAreaTolerance = 1f;
        [SerializeField] float timeDwelling = 2f;
        [SerializeField] float suspicionTime = 2f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 1f;
        Fighter fighter;
        Health health;
        Mover mover;
        //Guards Momory
        GameObject[] livestocks;
        GameObject closeLivestock = null;
        GameObject player = null;
        float timeSinceLastSawSheep = Mathf.Infinity;
        float timeSinceLastAttacked = Mathf.Infinity;
        float timeSinceArrivedToDwell = Mathf.Infinity;

        Vector3 waitPosition;

        private void Start()
        {
            StartCoroutine(FindLivestock());
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            waitPosition = transform.position;
            player = GameObject.FindWithTag("Player");
        }

        IEnumerator FindLivestock()
        {
            while (true)
            {
                livestocks = GameObject.FindGameObjectsWithTag("Sheep");
                yield return new WaitForSeconds(1f);
            }
        }

        // Should probably be in FixedUpdate to ensure that physics have been calculated.
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

            else if (InAttackRangeOfSheep() && fighter.CanAttack(closeLivestock))
            {
                timeSinceLastSawSheep = 0;
                AttactBehaviour(closeLivestock);
            }
            else if (timeSinceLastSawSheep < suspicionTime)
            {
                SuspicionBehaviour();

            }
            else
            {
                RoamBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceArrivedToDwell += Time.deltaTime;
            timeSinceLastSawSheep += Time.deltaTime;
            timeSinceLastAttacked += Time.deltaTime;
        }

        private float RangeToTarget(Vector3 position, Vector3 destination)
        {
            return Vector3.Distance(position, destination);
        }

        private void RoamBehaviour()
        {
            if (RangeToTarget(transform.position, waitPosition) < roamAreaTolerance)
            {
                Debug.Log("dwelling!");
                timeSinceArrivedToDwell = 0;
                SetWaitPosition();
            }
            if (timeSinceArrivedToDwell > timeDwelling)
            {
                Debug.Log("roaming!");
                mover.StartMoveAction(waitPosition, patrolSpeedFraction);
            }
        }

        private void SetWaitPosition()
        {
            float x = Random.Range(transform.position.x - roamAreaTolerance, transform.position.x + roamAreaTolerance);
            float z = Random.Range(transform.position.z - roamAreaTolerance, transform.position.z + roamAreaTolerance);
            if (NavMesh.SamplePosition(new Vector3(x, transform.position.y, z), out _, 1.0f, NavMesh.AllAreas))
            {
                waitPosition = new Vector3(x, transform.position.y, z);
                Debug.Log("New Position!" + x + z);
            }
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

        // Currently not working properly.
        private bool InAttackRangeOfSheep()
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
                    Debug.Log("Sheep!");
                    return distanceToLivestock < chaseDistance;
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