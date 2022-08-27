using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GAME.Movement;
using GAME.Core;

namespace GAME.Control
{
    public class SheepController : MonoBehaviour
    {
        [SerializeField]
        float thirst = 100f;
        [SerializeField]
        float thirstDecay = 50f;
        [SerializeField]
        float movementRandom = 5f;

        Mover movement;
        Health health;
        // Start is called before the first frame update
        void Start()
        {
            movement = GetComponent<Mover>();
            health = GetComponent<Health>();

        }

        void MoveBehaviour()
        {
            if (movementRandom >= 10)
            {
                if (movementRandom >= 20)
                {
                    movementRandom = 0;
                }
            }
            else
            {
            }
            movementRandom += 1 * Time.deltaTime;

        }

        void Update()
        {


            thirst -= thirstDecay * Time.deltaTime;



        }
    }
}