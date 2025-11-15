// Assets/Scripts/Hero/HeroData_map_4.cs
using UnityEngine;

[CreateAssetMenu(fileName = "HeroDataMap4", menuName = "Scriptable Objects/HeroDataMap4")]
public class HeroData : ScriptableObject
{
    [Header("Presentation")]
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Placement")]
    public int cost = 100;
    public GameObject prefab; 

    [Header("Targeting / Combat")]
    public LayerMask enemyLayer;
    public float attackRange = 5f;
    [Tooltip("Thời gian giữa 2 đòn tấn công (giây)")]
    public float attackCooldown = 1.2f;

    [Header("Projectile")]
    public float projectileSpeed = 10f;
    public float projectileDamage = 10f;
    public float projectileLifeTime = 3f;
    [Tooltip("Nếu sprite vũ khí gốc không nhìn theo trục +X, bù góc ở đây")]
    public float projectileRotationOffset = 0f;

    [Header("Art Facing")]
    [Tooltip("Sprite gốc nhìn sang phải thì bật = true, nhìn trái thì tắt.")]
    public bool artFacesRight = true;
    
    [Header("Special Ability (Spear Hero)")]
    [Tooltip("Bật kỹ năng 'Ngọn Lao Phán Quyết' cho hero này")]
    public bool hasEmpoweredAttack;
    
    [Tooltip("Số đòn đánh thường trước khi kích hoạt đòn đặc biệt")]
    public int attacksForSpecial = 4;
    
    [Tooltip("Hệ số nhân sát thương (VD: 3 = 300% sát thương)")]
    public float specialDamageMultiplier = 3f;
    
    [Tooltip("Hệ số nhân thời gian bay (VD: 2 = bay xa gấp 2)")]
    public float specialLifetimeMultiplier = 2f;

    // 🌟 THÊM MỚI: Kéo prefab hiệu ứng nổ (từ ảnh) vào đây 🌟
    [Tooltip("Prefab hiệu ứng sẽ chạy khi đòn đặc biệt trúng đích")]
    public GameObject specialEffectPrefab; 
}