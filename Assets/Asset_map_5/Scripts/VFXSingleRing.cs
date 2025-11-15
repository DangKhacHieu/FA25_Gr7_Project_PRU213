using UnityEngine;
using System.Collections;

public class VFXSingleRing : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.2f; // Thời gian hiệu ứng tồn tại (0.2 giây)
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Destroy(gameObject, fadeDuration); // Fallback nếu không có Sprite
            return;
        }

        // Bắt đầu Coroutine để làm hiệu ứng biến mất từ từ
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startTime = Time.time;
        Color startColor = _spriteRenderer.color;

        // Chờ và làm mờ Alpha
        while (Time.time < startTime + fadeDuration)
        {
            float elapsed = Time.time - startTime;
            float alpha = 1.0f - (elapsed / fadeDuration); // Giảm từ 1.0 xuống 0.0

            Color newColor = startColor;
            newColor.a = alpha;
            _spriteRenderer.color = newColor;

            yield return null; // Chờ frame tiếp theo
        }

        // Hủy đối tượng sau khi hiệu ứng mờ hoàn tất
        Destroy(gameObject);
    }
}