using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GAME.Core;
using GAME.Movement;

namespace RPG.Combat
{

    public class Fighter : MonoBehaviour, IAction
    {

        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] float weaponRange = 3f;
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        private void Start()
        {
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null || target.IsDead()) return;
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().CancelAction();
                AttackBehaviour();
            }
        }


        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit() event.

                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");

            // Delete and move to HIT when we got an animation
            if (target == null) return;
            else
            {
                target.TakeDamage(weaponDamage);
            }
        }

        // Animation Event - Called from the animator
        void Hit()
        {
            if (target == null) return;
            else
            {
                target.TakeDamage(weaponDamage);
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(target.transform.position, transform.position) < weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
            print("Take that!");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void CancelAction()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().CancelAction();
        }

        private void StopAttack()
        {
            /*           GetComponent<Animator>().ResetTrigger("attack");
                      GetComponent<Animator>().SetTrigger("stopAttack"); */
        }

        public void StartAction(Vector3 destination, float speed)
        {
            throw new NotImplementedException();
        }
    }

}