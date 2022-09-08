using GAME.Core;
using UnityEngine;
namespace GAME.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {

        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject EquippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;


        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            if (EquippedPrefab != null)
            {
                Instantiate(EquippedPrefab, GetTransform(rightHand, leftHand));
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {

            Transform handTransform;
            if (!isRightHanded)
            {
                handTransform = leftHand;
            }
            else
            {
                handTransform = rightHand;
            }
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetWeaponDamage()
        {
            return weaponDamage;
        }
        public float GetTimeBetweenAttacks()
        {
            return timeBetweenAttacks;
        }
    }
}