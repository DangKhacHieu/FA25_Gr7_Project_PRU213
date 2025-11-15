using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeathBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    private Image _image;
    private float _target = 1f;
    private Coroutine _coroutine;
    private Color _newHealthBarColor;

    private Vector3 _originalScale;

    void Awake()
    {
        _image = GetComponent<Image>();
        _originalScale = transform.localScale;
    }

    void Start()
    {
        CheckHealthGradientAmount();
        _image.color = _newHealthBarColor;
    }

    private void LateUpdate()
    {
        if (transform.parent == null) return;
        float parentScaleX = transform.parent.localScale.x;
        Vector3 currentScale = transform.localScale;

        if (parentScaleX < 0)
        {
            Debug.Log("quay < 0");
            currentScale.x = -Mathf.Abs(_originalScale.x);
        }
        else
        {
            Debug.Log("quay > 0");
            currentScale.x = Mathf.Abs(_originalScale.x);
        }
        transform.localScale = currentScale;
    }

    public void UpdateHeathBar(float maxHealth, float currentHealth)
    {
        _target = currentHealth / maxHealth;
        CheckHealthGradientAmount();

        if (_image == null) _image = GetComponent<Image>(); // Đề phòng

        if (!gameObject.activeInHierarchy)
        {
            _image.fillAmount = _target;
            _image.color = _newHealthBarColor;
        }
        else
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _coroutine = StartCoroutine(DrainHealthBar());
        }
    }

    private IEnumerator DrainHealthBar()
    {
        float fillAmount = _image.fillAmount;
        Color currentColor = _image.color;
        float elapsedTime = 0f;
        while (elapsedTime < _timeToDrain)
        {
            elapsedTime += Time.deltaTime;

            _image.fillAmount = Mathf.Lerp(fillAmount, _target, (elapsedTime / _timeToDrain));
            _image.color = Color.Lerp(currentColor, _newHealthBarColor, (elapsedTime / _timeToDrain));
            yield return null;
        }
    }

    private void CheckHealthGradientAmount()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_target);
    }
}