using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
[DisallowMultipleComponent]
public class FloatingDamageText : MonoBehaviour
{
    [Header("Motion & Fade")]
    public float moveUpSpeed = 1.5f;
    public float fadeSpeed = 2f;
    public float lifetime = 1f;
    public bool faceCamera = true;     // billboard
    public bool clampWorldScale = true;

    private TextMeshPro tmp;
    private Color col;
    private float timer;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        col = tmp.color;

        // Ép Sorting để luôn nổi trên sprite
        var mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = "UI"; // hoặc "Effects"
            mr.sortingOrder = 500;
        }
    }

    public void SetText(string text, Color color, int sortingOrderAdd = 0)
    {
        tmp.text = text;
        tmp.color = color;
        col = color;

        var mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.sortingOrder += sortingOrderAdd;
    }

    void Update()
    {
        // Bay lên
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;

        // Billboard để luôn nhìn camera (game 2D ortho vẫn hữu ích)
        if (faceCamera && Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;

        // Giữ kích thước đọc được trên mọi màn (tùy chọn)
        if (clampWorldScale && Camera.main != null)
        {
            // Với camera Ortho: scale theo orthographicSize
            float s = Mathf.Clamp(Camera.main.orthographicSize * 0.02f, 0.015f, 0.03f);
            transform.localScale = new Vector3(s, s, s);
        }

        // Mờ dần
        timer += Time.deltaTime;
        col.a = Mathf.Lerp(1f, 0f, timer / lifetime);
        tmp.color = col;

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
