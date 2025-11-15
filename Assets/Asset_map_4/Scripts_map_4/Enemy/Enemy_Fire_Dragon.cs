using UnityEngine;
using System.Collections;

public class Enemy_Fire_Dragon : Enemy_map_4
{
    [Header("Fire Dragon Skill")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float stunDuration = 1.5f;
    [SerializeField] private LayerMask platformLayer;

    [SerializeField] private GameObject explosionEffect;

    public override void Initialize(float healthMultiplier, Path_map_4 assignedPath)
    {
        base.Initialize(healthMultiplier, assignedPath);

        if (explosionEffect != null)
        {
            explosionEffect.SetActive(false);
        }
    }

    protected override void Die()
    {
        ExplodeOnDeath();
        base.Die();
    }

    private void ExplodeOnDeath()
    {
        Debug.Log("RỒNG CON BẮT ĐẦU NỔ!");
        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        if (explosionEffect != null)
        {
            explosionEffect.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        Collider2D[] platforms = Physics2D.OverlapCircleAll(transform.position, explosionRadius, platformLayer);
        foreach (var col in platforms)
        {
            var hero = col.GetComponentInChildren<HeroAttack_map_4>();
            if (hero != null)
            {
                Debug.Log($"Làm choáng {hero.name}!");
                hero.Stun(stunDuration); //
            }
        }

        if (explosionEffect != null)
        {
            explosionEffect.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}