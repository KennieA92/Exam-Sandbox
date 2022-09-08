using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GAME.Core
{
    public class HealthBar : MonoBehaviour
    {
        private Health health;
        public Slider hpBar;
        // Start is called before the first frame update
        void Awake()
        {
            health = GetComponent<Health>();
            hpBar.value = 1f;
        }

        // Update is called once per frame
        void Update()
        {
            if (health == null) return;
            if (health.IsDead()) return;

            hpBar.value = health.GetHealth() / health.maxHealthPoints;
            hpBar.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        }
    }

}