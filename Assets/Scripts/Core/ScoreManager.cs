using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public float score = 0;
    private float sheepSaved = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            sheepSaved += 1;
            score += 1 + (sheepSaved * Random.Range(1, 5));
            Destroy(other.gameObject);
        }
    }
}
