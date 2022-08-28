using GAME.Core;
using UnityEngine;

public class Target : MonoBehaviour, IArrowHittable
{
    public float forceAmount = 1.0f;
    public Material otherMaterial = null;

    Health health;

    public void Hit(Arrow arrow)
    {
        ApplyMaterial();
        ApplyForce(arrow);
        health = GetComponent<Health>();
        health.TakeDamage(arrow.GetDamage());
        Debug.Log("hit");

    }

    private void ApplyMaterial()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
            meshRenderer.material = otherMaterial;
    }

    private void ApplyForce(Arrow arrow)
    {
        if (TryGetComponent(out Rigidbody rigidbody))
            rigidbody.AddForce(arrow.transform.forward * forceAmount);
    }

    private void DisableCollider(Arrow arrow)
    {
        if (arrow.TryGetComponent(out Collider collider))
            collider.enabled = false;
    }
}
