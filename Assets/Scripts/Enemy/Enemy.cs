using System;
using Unity.Jobs;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
   // public EnemyData Data => data;
    public static event Action<EnemyData> OnEnemyReachedEnd;
    
    private Path _currentPath;
    
    private Vector3 _targetPosition;
    private int _currentWaypoint;

   // public float currentHealth;
   // public float distanceTravelled; // Dùng cho logic “First” (quái đi xa nhất)
   // public bool isDead => currentHealth <= 0;

    /*  private void Awake()
      {
          _currentPath = GameObject.Find("Path1").GetComponent<Path>();
      }*/

    public void SetPath(Path path)
    {
        _currentPath = path;
    }

    private void OnEnable()
    {
        /*_currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);*/

        /* if (_currentPath == null)
        {
            Debug.LogWarning($"{name} chưa có đường đi được gán!");
            return;
        }*/

        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWaypoint);
        transform.position = _targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentPath == null) return;
        // move towards target Position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        /*float step = data.speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
        distanceTravelled += step;*/

        // When target reached, set new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if(relativeDistance < 0.1f)
        {
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1) 
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }else // reached last waypoint
            {
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
            }
           
        }
    }

   /* private void Start()
    {
        currentHealth = data.lives;
        EnemyManager.Instance.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
            EnemyManager.Instance.UnregisterEnemy(this);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EnemyManager.Instance.UnregisterEnemy(this);
            Destroy(gameObject);
        }
    }

    public bool isIceMapEnemy
    {
        get
        {
            return data.type == EnemyType.yeti ||
                   data.type == EnemyType.YetiTanker ||
                   data.type == EnemyType.PhuThuyBang ||
                   data.type == EnemyType.SnowMan ||
                   data.type == EnemyType.BossYeti;
        }
    }*/


}
