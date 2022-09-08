using System;
using System.Collections;
using System.Collections.Generic;
using GAME.Core;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 1f;

    Health target = null;
    float damage = 0;

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        transform.LookAt(GetAimLocation());
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.transform.position) < 1.5f)
        {
            if (target.GetComponent<Health>())
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public void SetTarget(Health target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    private Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if (targetCapsule == null)
        {
            return target.transform.position;
        }

        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }

}
