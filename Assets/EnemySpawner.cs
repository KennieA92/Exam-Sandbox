using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace GAME.Core
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] Transform player;
        public GameObject enemy;
        private bool start = true;
        private GameObject[] sheep;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SpawnEnemies());
        }
        IEnumerator SpawnEnemies()
        {
            if (start)
            {
                yield return new WaitForSeconds(10f);
                start = false;
            }
            float enemySpawns = 1f;
            float waitTime = 30f;
            while (waitTime >= 0)
            {
                for (int i = 0; i < enemySpawns; i++)
                {
                    Create();
                }
                yield return new WaitForSeconds(waitTime);
                waitTime--;
                float res = waitTime % 2;
                if (res == 0)
                {
                    enemySpawns++;
                }

            }
            yield return null;
        }

        // Change sheep[index] to player, to make the predators spawn based on the player location.
        void Create()
        {

            sheep = GameObject.FindGameObjectsWithTag("Sheep");
            int index = Random.Range(0, sheep.Length);
            if (sheep[index] == null) return;
            float x = Random.Range(sheep[index].transform.position.x - 50, sheep[index].transform.position.x + 50);
            float z = Random.Range(sheep[index].transform.position.z - 50, sheep[index].transform.position.z + 50);
            Debug.Log("Entered Create");
            while (!NavMesh.SamplePosition(new Vector3(x, sheep[index].transform.position.y, z), out _, 1.0f, NavMesh.AllAreas))
            {
                x = Random.Range(sheep[index].transform.position.x - 50, sheep[index].transform.position.x + 50);
                z = Random.Range(sheep[index].transform.position.z - 50, sheep[index].transform.position.z + 50);
            }
            if (NavMesh.SamplePosition(new Vector3(x, sheep[index].transform.position.y, z), out _, 1.0f, NavMesh.AllAreas))
            {
                Instantiate(enemy, new Vector3(x, sheep[index].transform.position.y, z), Quaternion.identity);
            }
        }

        // Potentionally a better way to spawn enemies will be looked into if time allows it.  Is considered "nice to have" 
        void DoSpawnEnemies()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            int vertextIndex = Random.Range(0, triangulation.vertices.Length);
            NavMeshHit hit;


            if (NavMesh.SamplePosition(triangulation.vertices[vertextIndex], out hit, 2f, 0))
            {

                enemy.GetComponent<NavMeshAgent>().Warp(hit.position);
                enemy.GetComponent<NavMeshAgent>().enabled = true;
            }
        }

    }
}