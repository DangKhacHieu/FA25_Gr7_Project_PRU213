using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform throwPoint;
    public float attackCooldown = 1.2f;
    public float attackRange = 5f;

    [Header("References")]
    public Animator animator;
    public LayerMask enemyLayer;
    public ObjectPooler spearPool;
    private SpriteRenderer spriteRenderer;

    [Header("Facing Settings")]
    [Tooltip("Nếu sprite gốc nhìn sang phải thì bật = true, nhìn trái thì tắt.")]
    public bool artFacesRight = true;

    private float nextAttackTime = 0f;
    private Transform target;
    private bool isAttacking = false;
    private bool facingRight = true;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Update()
    {
        if (isAttacking) return; // đang attack thì bỏ qua
        if (Time.time < nextAttackTime) return;

        // tìm quái trong tầm
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        var validEnemies = enemies
            .Select(e => e.GetComponent<Enemy>())
            .Where(e => e != null && e.gameObject.activeInHierarchy && e.CurrentHP > 0)
            .ToList();

        if (validEnemies.Count > 0)
        {
            target = validEnemies
                .OrderBy(e => Vector2.Distance(transform.position, e.transform.position))
                .First().transform;

            // 🔥 Flip hướng theo vị trí quái
            UpdateFacingDirection(target.position.x);

            if (!isAttacking)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("isIdle", false);
                isAttacking = true;
            }
        }
        else
        {
            target = null;
            animator.ResetTrigger("Attack");
            animator.SetBool("isIdle", true);
        }
    }

    // ✅ Flip hướng hero dựa vào vị trí target
    private void UpdateFacingDirection(float targetX)
    {
        bool targetIsRight = targetX > transform.position.x;
        if (targetIsRight != facingRight)
        {
            facingRight = targetIsRight;
            Vector3 scale = transform.localScale;

            // Nếu art gốc nhìn phải → scaleX dương là phải, âm là trái
            // Nếu art gốc nhìn trái → ngược lại
            scale.x = artFacesRight
                ? Mathf.Abs(scale.x) * (facingRight ? 1 : -1)
                : Mathf.Abs(scale.x) * (facingRight ? -1 : 1);

            transform.localScale = scale;
        }
    }

    // Animation Event: ném lao
    public void ThrowSpear()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            StopAttack();
            return;
        }

        GameObject spear = spearPool.GetPooledObject();
        if (spear == null) return;

        spear.transform.position = throwPoint.position;
        spear.SetActive(true);

        Weapon_Projectile spearScript = spear.GetComponent<Weapon_Projectile>();
        if (spearScript != null)
            spearScript.Launch(target);
    }

    // Animation Event: kết thúc animation Attack
    public void EndAttack()
    {
        isAttacking = false;
        nextAttackTime = Time.time + attackCooldown;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        if (target == enemy?.transform)
            target = null;
    }

    private void StopAttack()
    {
        isAttacking = false;
        animator.SetBool("isIdle", true);
        nextAttackTime = Time.time + attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (throwPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, throwPoint.position);
        }
    }
}
