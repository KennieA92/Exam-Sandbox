using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME.Control
{
    public class PatrolPathController : MonoBehaviour
    {
        [SerializeField] float gatherDistance = 10f;
        GameObject[] sheep = null;
        List<GameObject> closeSheep = null;


        private void Start()
        {
            sheep = GameObject.FindGameObjectsWithTag("Sheep");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                InfluenceSheep();
            }
        }


        void FindSheep()
        {
            float distanceToSheep = 30f;
            foreach (GameObject shee in sheep)
            {
                if (shee == null) continue;
                distanceToSheep = Vector3.Distance(shee.transform.position, transform.position);
                if (distanceToSheep < gatherDistance)
                {
                    Debug.Log(shee);
                    closeSheep.Add(shee);
                }
            }
        }


        void InfluenceSheep()
        {
            FindSheep();
            for (int i = 0; i <= closeSheep.Count; i++)
            {
                AIController currentSheep = closeSheep[i].GetComponent<AIController>();

            }
        }
    }
}

