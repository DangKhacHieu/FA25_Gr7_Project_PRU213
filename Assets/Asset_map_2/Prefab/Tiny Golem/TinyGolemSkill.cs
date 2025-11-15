using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyHealth))]
public class TinyGolemSkill : MonoBehaviour
{
    [Header("⚙️ Cấu hình kỹ năng tăng tốc đồng minh")]
    public float phamVi = 4f;
    public float thoiGianHieuLuc = 4f;
    public float thoiGianHoiChieu = 6f;
    public float heSoTangTocThuong = 1.4f;
    public float heSoTangTocManh = 1.65f;

    [Header("Hiệu ứng kích hoạt (ánh sáng lan tỏa)")]
    public GameObject hieuUngBuff;

    [Header("Âm thanh khi kích hoạt")]
    public AudioClip amThanhBuff;         // file âm thanh roar hoặc thump
    [Range(0f, 1f)] public float amLuong = 1f;

    private EnemyHealth mauEnemy;
    private AudioSource audioSource;
    private float demHoiChieu = 0f;

    void Start()
    {
        mauEnemy = GetComponent<EnemyHealth>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        demHoiChieu = thoiGianHoiChieu;
        Debug.Log($"🧱 [TinyGolemSkill] Khởi tạo kỹ năng tăng tốc đồng minh.");
    }

    void Update()
    {
        if (mauEnemy == null || mauEnemy.GetCurrentHPPercent() <= 0)
            return;

        demHoiChieu -= Time.deltaTime;
        if (demHoiChieu <= 0f)
        {
            BuffDongMinh();
            demHoiChieu = thoiGianHoiChieu;
        }
    }

    void BuffDongMinh()
    {
        float hpPercent = mauEnemy.GetCurrentHPPercent();
        float heSoTang = hpPercent > 0.5f ? heSoTangTocThuong : heSoTangTocManh;
        string loaiBuff = hpPercent > 0.5f ? "+40%" : "+65%";

        Debug.Log($"🔥 [TinyGolemSkill] Tiny Golem kích hoạt buff {loaiBuff} (HP: {(hpPercent * 100f):F0}%)");

        // 🌟 Hiệu ứng ánh sáng
        if (hieuUngBuff != null)
        {
            GameObject fx = Instantiate(hieuUngBuff, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // 🔊 Phát âm thanh roar / thump
        if (amThanhBuff != null)
        {
            audioSource.PlayOneShot(amThanhBuff, amLuong);
            Debug.Log("🔊 [TinyGolemSkill] Phát âm thanh buff!");
        }
        else
        {
            Debug.LogWarning("⚠️ [TinyGolemSkill] Chưa gán file âm thanh buff trong Inspector!");
        }

        // Buff tốc độ cho quái gần boss
        Collider2D[] quanhBoss = Physics2D.OverlapCircleAll(transform.position, phamVi);
        foreach (Collider2D col in quanhBoss)
        {
            if (col.CompareTag("Enemy") && col.gameObject != this.gameObject)
            {
                EnemyMovement move = col.GetComponent<EnemyMovement>();
                if (move != null)
                    StartCoroutine(TangTocTamThoi(move, heSoTang));
            }
        }
    }

    System.Collections.IEnumerator TangTocTamThoi(EnemyMovement move, float heSo)
    {
        float tocDoGoc = move.speed;
        move.speed = tocDoGoc * heSo;
        Debug.Log($"⚡ [TinyGolemSkill] {move.name} được buff tốc độ! (x{heSo})");

        yield return new WaitForSeconds(thoiGianHieuLuc);
        if (move != null) move.speed = tocDoGoc;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, phamVi);
    }
}
