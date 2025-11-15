using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyHealth))]
public class GolemSkill : MonoBehaviour
{
    [Header("⚡ Cấu hình kỹ năng tăng tốc tạm thời")]
    public float heSoTangToc = 1.5f;
    public float thoiGianToc = 2f;
    public float thoiGianHoiChieu = 6f;

    [Header("🔥 Hiệu ứng khi tăng tốc (tùy chọn)")]
    public GameObject hieuUngToc;
    public AudioClip amThanhToc;
    [Range(0f, 1f)] public float amLuong = 0.8f;

    [Header("🎨 Tùy chỉnh hiển thị hiệu ứng")]
    [Tooltip("Khoảng cách theo trục Y (âm = thấp hơn, dương = cao hơn Golem)")]
    public float doCaoHieuUng = -0.1f;

    [Tooltip("Độ sâu hiệu ứng (âm = sau lưng, dương = trước mặt)")]
    public float doSauHieuUng = -0.5f;

    [Tooltip("Kích thước hiệu ứng lửa (1 = bình thường, 2 = gấp đôi, 3 = gấp ba,...)")]
    public float heSoPhongTo = 3f;

    [Tooltip("Layer/Order để đảm bảo nằm sau lưng Golem")]
    public string sortingLayerName = "Enemy";
    public int sortingOrder = 1; // nhỏ hơn order của Golem (ví dụ Golem = 2)

    private EnemyMovement enemyMove;
    private EnemyHealth enemyHealth;
    private float tocDoGoc;
    private bool daKichHoat = false;
    private AudioSource audioSource;

    void Start()
    {
        enemyMove = GetComponent<EnemyMovement>();
        enemyHealth = GetComponent<EnemyHealth>();
        tocDoGoc = enemyMove.speed;

        // Âm thanh
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        // Lắng nghe khi bị bắn lần đầu
        enemyHealth.OnDamaged += KichHoatLanDau;
    }

    private void KichHoatLanDau()
    {
        if (!daKichHoat)
        {
            daKichHoat = true;
            StartCoroutine(KichHoatTangToc());
        }
    }

    IEnumerator KichHoatTangToc()
    {
        // 🌟 Hiệu ứng (không bị co theo scale cha)
        if (hieuUngToc != null)
        {
            // 1) Vị trí spawn theo THẾ GIỚI
            Vector3 viTriHieuUng = new Vector3(
                transform.position.x,
                transform.position.y + doCaoHieuUng,
                transform.position.z + doSauHieuUng
            );

            // 2) Tạo effect với WORLD SCALE mong muốn
            GameObject fx = Instantiate(hieuUngToc, viTriHieuUng, Quaternion.identity);
            fx.transform.localScale = Vector3.one * heSoPhongTo; // đặt scale TRƯỚC khi gán parent

            // 3) Gán parent nhưng GIỮ world transform (nên world scale không đổi)
            fx.transform.SetParent(transform, worldPositionStays: true);

            // 4) Chỉnh lại offset theo LOCAL để đi theo Golem đúng vị trí
            fx.transform.localPosition = new Vector3(0f, doCaoHieuUng, doSauHieuUng);

            // 5) Layer/Order để nằm sau lưng Golem
            var renderer = fx.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = sortingOrder;
            }

            // 6) Tự hủy khi hết thời gian buff
            Destroy(fx, thoiGianToc);
        }

        // 🔊 Âm thanh
        if (amThanhToc != null)
            audioSource.PlayOneShot(amThanhToc, amLuong);

        // 🏃‍♂️ Tăng tốc
        enemyMove.speed = tocDoGoc * heSoTangToc;
        yield return new WaitForSeconds(thoiGianToc);
        enemyMove.speed = tocDoGoc;
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
            enemyHealth.OnDamaged -= KichHoatLanDau;
    }
}
