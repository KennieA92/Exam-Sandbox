using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GAME.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 100f;
        bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            print(healthPoints);
            if (healthPoints == 0)
            {
                Die();
            }
        }

        //use when animator
        /*         private void Die()
                {
                    if (isDead) return;

                    isDead = true;
                    GetComponent<Animator>().SetTrigger("die");
                    GetComponent<ActionScheduler>().CancelCurrentAction();
                } */
        private void Die()
        {
            if (gameObject.CompareTag("Player"))
            {

            }
            Destroy(gameObject);
        }

    }
}