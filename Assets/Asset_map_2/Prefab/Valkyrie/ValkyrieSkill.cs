using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class ValkyrieSkill : MonoBehaviour
{
    [Header("🛡️ Kỹ năng đặc biệt của Valkyrie")]
    [Tooltip("0.8 = nhận 80% sát thương (giảm 20%)")]
    public float heSoGiamSatThuong = 0.8f;

    [Tooltip("Thời gian hiệu lực của hiệu ứng (giây)")]
    public float thoiGianHieuLuc = 2.5f;

    [Tooltip("Thời gian hồi chiêu sau khi hiệu ứng kết thúc (giây)")]
    public float thoiGianHoiChieu = 6f;

    [Header("Hiệu ứng khiên phóng to")]
    [Tooltip("Object hình cái khiên (SpriteRenderer)")]
    public GameObject spriteKhien;

    private EnemyHealth mauEnemy;
    private bool dangGiamSatThuong = false;
    private bool dangHoiChieu = false;
    private float demHieuLuc = 0f;
    private float demHoiChieu = 0f;

    void Awake()
    {
        mauEnemy = GetComponent<EnemyHealth>();
        mauEnemy.OnTakeDamage += XuLySatThuong;

        if (spriteKhien != null)
            spriteKhien.SetActive(false);
    }

    void OnDestroy()
    {
        if (mauEnemy != null)
            mauEnemy.OnTakeDamage -= XuLySatThuong;
    }

    void Update()
    {
        // Hiệu lực buff
        if (dangGiamSatThuong)
        {
            demHieuLuc -= Time.deltaTime;
            if (demHieuLuc <= 0f)
            {
                dangGiamSatThuong = false;
                dangHoiChieu = true;
                demHoiChieu = thoiGianHoiChieu;

                if (spriteKhien != null)
                    spriteKhien.SetActive(false);

                Debug.Log("🛡️ Valkyrie hết khiên, bắt đầu hồi chiêu!");
            }
        }

        // Hồi chiêu
        if (dangHoiChieu)
        {
            demHoiChieu -= Time.deltaTime;
            if (demHoiChieu <= 0f)
            {
                dangHoiChieu = false;
                Debug.Log("✨ Kỹ năng của Valkyrie sẵn sàng kích hoạt lại!");
            }
        }
    }

    private int XuLySatThuong(int damageGoc)
    {
        int satThuongSauCung = damageGoc;

        if (!dangHoiChieu)
        {
            if (!dangGiamSatThuong)
            {
                dangGiamSatThuong = true;
                demHieuLuc = thoiGianHieuLuc;

                if (spriteKhien != null)
                {
                    spriteKhien.SetActive(true);
                    StartCoroutine(PhongToKhien(spriteKhien.transform));
                }

                Debug.Log($"🛡️ Valkyrie kích hoạt giảm sát thương trong {thoiGianHieuLuc}s!");
            }

            if (dangGiamSatThuong)
                satThuongSauCung = Mathf.RoundToInt(damageGoc * heSoGiamSatThuong);
        }

        return satThuongSauCung;
    }

    private System.Collections.IEnumerator PhongToKhien(Transform target)
    {
        float elapsed = 0f;
        float speed = 5f;      // tốc độ nhấp nháy nhanh hơn
        float maxScale = 5f;   // phóng to gấp đôi
        Vector3 baseScale = Vector3.one;

        Debug.Log("🔹 Bắt đầu phóng to khiên!");

        while (elapsed < thoiGianHieuLuc)
        {
            float t = Mathf.PingPong(elapsed * speed, 1f);   // dao động từ 0 -> 1 -> 0
            float scale = Mathf.Lerp(1f, maxScale, t);       // nội suy giữa 1 và 2
            target.localScale = new Vector3(scale, scale, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = baseScale;
        Debug.Log("🔹 Kết thúc phóng to khiên!");
    }

}
