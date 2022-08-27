using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    [SerializeField]
    float spawnSpeed = 1f;
    [SerializeField]
    private float spawnOffset = 5;
    public GameObject sheep = null;


    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(SpawnSheep());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnSheep()
    {
        for (int i = 0; i <= 10; i++)
        {
            float x = Random.Range(transform.position.x - spawnOffset, transform.position.x + spawnOffset);
            float z = Random.Range(transform.position.z - spawnOffset, transform.position.z + spawnOffset);

            if (sheep)
            {
                Instantiate(sheep, new Vector3(x, transform.position.y, z), Quaternion.identity);
            }
            yield return new WaitForSeconds(spawnSpeed);
        }
    }
}
