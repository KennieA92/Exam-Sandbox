using System;
using System.Collections;
using GAME.Core;
using GAME.Movement;
using GAME.Combat;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GAME.Control
{


    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float playerDistance = 10f;
        [SerializeField] float seekWaterDistance = 10f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float dwellTolerance = 2f;
        // Used to describe the ammount of time a livestock
        // should dwell at a waypoint or after a player is away.
        [SerializeField] float timeDwelling = 4f;
        [SerializeField] float fleeDistance = 20f;
        [SerializeField] bool automaticPathing = true;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float thirst = 100;
        Fighter fighter;
        Health health;
        Mover mover;
        //Guards Momory
        GameObject player;
        GameObject[] predators;
        GameObject closePredator = null;
        GameObject[] bandits;
        GameObject closeBandit = null;
        GameObject[] waterSources;
        GameObject closeWaterSource;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastSawPredator = Mathf.Infinity;
        // Used to determine time to dwell or stay at a waypoint
        float timeSinceArrivedToDwell = Mathf.Infinity;
        int currentWaypointIndex = 0;

        Vector3 waitPosition;
        Vector3 guardPosition;

        private void Start()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();
            predators = GameObject.FindGameObjectsWithTag("Predator");
            bandits = GameObject.FindGameObjectsWithTag("Bandit");
            waterSources = GameObject.FindGameObjectsWithTag("Water");
            waitPosition = transform.position;
            guardPosition = transform.position;
            StartCoroutine(FindThreats());
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (InRangeOfPredator())
            {
                FleeBehaviour();
            }
            else if (InRangeOfWater() && thirst < 95)
            {
                DrinkingBehaviour();
            }
            else if (timeSinceLastSawPredator < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else if (InRangeOfBandit() && InRangeOfPlayer())
            {
                FollowPlayerBehaviour();
            }
            else if (InRangeOfBandit())
            {
                FleeBehaviour();
            }

            else if (automaticPathing)
            {
                PatrolBehaviour();
            }
            else
            {
                if (InRangeOfPlayer())
                {
                    FollowPlayerBehaviour();
                }
                else
                {
                    GrazingBehaviour();
                }
            }

            /*else if (InRangeOfPlayer()  )
            {
                timeSinceArrivedAtWaypoint = timeDwelling + 1;
            }*/

            UpdateTimers();
        }

        IEnumerator FindThreats()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                predators = GameObject.FindGameObjectsWithTag("Predator");
                bandits = GameObject.FindGameObjectsWithTag("Bandit");
            }
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedToDwell += Time.deltaTime;
            timeSinceLastSawPredator += Time.deltaTime;
        }

        private void FleeBehaviour()
        {
            if (closePredator == null && closeBandit == null) return;

            if (closePredator)
            {
                Debug.Log("Afraid");
                Vector3 runTo = transform.position + ((transform.position - closePredator.transform.position));
                mover.StartMoveAction(runTo, 1f);
            }
            else if (closeBandit && InRangeOfPlayer())
            {
                mover.StartMoveAction(player.transform.position, 1f);
                Debug.Log("Save me from the Bandit!");
            }
            else
            {
                Vector3 runTo = transform.position + ((transform.position - closeBandit.transform.position));
                mover.StartMoveAction(runTo, 1f);
            }
            timeSinceLastSawPredator = 0f;
        }

        private void PatrolBehaviour()
        {
            Debug.Log("Patrolling");
            Vector3 nextPoisition = guardPosition;
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    timeSinceArrivedToDwell = 0;
                    CycleWaypoint();
                }
                nextPoisition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedToDwell > timeDwelling)
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
            return RangeToTarget(transform.position, GetCurrentWaypoint()) < dwellTolerance;
        }

        private float RangeToTarget(Vector3 position, Vector3 destination)
        {
            return Vector3.Distance(position, destination);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private bool InRangeOfPredator()
        {
            if (predators == null) return false;
            float distanceToPredator = Mathf.Infinity;
            for (int i = 0; i < predators.Length; i++)
            {
                if (predators[i] == null) continue;
                distanceToPredator = RangeToTarget(predators[i].transform.position, transform.position);
                if (distanceToPredator < fleeDistance)
                {
                    closePredator = predators[i];
                    Debug.Log("In range of Predator!" + closePredator.name);
                }
            }
            return distanceToPredator < fleeDistance;
        }

        private bool InRangeOfBandit()
        {
            if (bandits == null) return false;
            float distanceToBandit = Mathf.Infinity;
            for (int i = 0; i < bandits.Length; i++)
            {
                if (bandits[i] == null) continue;
                distanceToBandit = RangeToTarget(bandits[i].transform.position, transform.position);
                if (distanceToBandit < fleeDistance)
                {
                    closeBandit = bandits[i];
                }
            }
            return distanceToBandit < fleeDistance;
        }

        private bool InRangeOfPlayer()
        {
            if (player == null) return false;
            return RangeToTarget(player.transform.position, transform.position) < playerDistance;
        }

        private void FollowPlayerBehaviour()
        {
            if (RangeToTarget(player.transform.position, transform.position) > 3f)
            {
                mover.StartMoveAction(player.transform.position, 1f);
                timeSinceLastSawPlayer = 0f;
            }
            else
            {
                mover.CancelAction();
            }
        }

        private void SetWaitPosition()
        {
            float x = Random.Range(transform.position.x - dwellTolerance, transform.position.x + dwellTolerance);
            float z = Random.Range(transform.position.z - dwellTolerance, transform.position.z + dwellTolerance);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(new Vector3(x, transform.position.y, z), out hit, 1.0f, NavMesh.AllAreas))
            {
                waitPosition = new Vector3(x, transform.position.y, z);
                Debug.Log("New Position!" + x + z);
            }
        }

        private void GrazingBehaviour()
        {
            if (RangeToTarget(transform.position, waitPosition) < dwellTolerance)
            {
                Debug.Log("dwelling!");
                timeSinceArrivedToDwell = 0;
            }
            if (timeSinceArrivedToDwell > timeDwelling)
            {
                SetWaitPosition();
                Debug.Log("grazing!");
                mover.StartMoveAction(waitPosition, patrolSpeedFraction);
            }
        }

        private bool InRangeOfWater()
        {
            if (waterSources == null) return false;
            float distanceToWater = Mathf.Infinity;
            for (int i = 0; i < waterSources.Length; i++)
            {
                if (waterSources[i] == null) continue;
                distanceToWater = RangeToTarget(waterSources[i].transform.position, transform.position);
                closeWaterSource = waterSources[i];
            }
            return distanceToWater < seekWaterDistance;
        }

        private void DrinkingBehaviour()
        {
            if (closeWaterSource == null) return;
            mover.StartMoveAction(closeWaterSource.transform.position, 1f);
            if (RangeToTarget(closeWaterSource.transform.position, transform.position) < 3f)
            {
                thirst += 0.5f * Time.deltaTime;
            }

        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}