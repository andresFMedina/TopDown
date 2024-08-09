using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public QuadTree quadTree { get; private set; }
    private PlayerMovement player;
    public List<Transform> enemies;
    public List<string> nearEnemies = new();
    [SerializeField] private float updateTreeInterval = 1f;
    private float nextUpdateTreeInterval;

    void Awake()
    {
        quadTree = new(0, GameManager.Instance.levelBounds);
    }

    private void Start()
    {
        player = GameManager.Instance.player.GetComponent<PlayerMovement>();
    }

    private void OnDrawGizmos()
    {
        if (quadTree != null)
        {
            quadTree.DrawDebug();
        }
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        var enemies = player.GetNearEnemies();
        foreach (var enemy in enemies)
        {
            if (enemy.transform.TryGetComponent<EnemyMovement>(out var enemyMovement))
            {
                enemyMovement.enabled = true;
                nearEnemies.Add(enemyMovement.name);
            }
        }

        if (Time.time > nextUpdateTreeInterval)
        {
            //UpdateTree();
            UpdateEnemies();
            UpdateQuadTree();
            nextUpdateTreeInterval = Time.time + updateTreeInterval;
        }

    }

    private void UpdateTree()
    {
        Task.WaitAll(UpdateEnemiesAsync(), UpdateQuadTreeAsync());
    }

    void UpdateEnemies()
    {
        List<Transform> nearbyEnemies = new List<Transform>();
        var player = GameManager.Instance.player;
        nearbyEnemies = quadTree.Retrieve(nearbyEnemies, new Rect(player.position.x, player.position.y, 0, 0));

        foreach (Transform enemy in enemies)
        {
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            if (enemyScript == null || nearEnemies.Contains(enemy.name)) continue;

            enemyScript.enabled = nearbyEnemies.Contains(enemy);
        }
    }

    void UpdateQuadTree()
    {
        quadTree.Clear();

        foreach (Transform enemy in enemies)
        {
            quadTree.Insert(enemy);
        }
    }

    async Task UpdateEnemiesAsync()
    {
        List<Transform> nearbyEnemies = new List<Transform>();
        var player = GameManager.Instance.player;
        nearbyEnemies = await Task.Run(() => quadTree.Retrieve(nearbyEnemies, new Rect(player.position.x, player.position.y, 0, 0)));

        foreach (Transform enemy in enemies)
        {
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            if (enemyScript == null || nearEnemies.Contains(enemy.name)) continue;

            enemyScript.enabled = nearbyEnemies.Contains(enemy);
        }
    }

    async Task UpdateQuadTreeAsync()
    {
        quadTree.Clear();

        foreach (Transform enemy in enemies)
        {
            await Task.Run(() => quadTree.Insert(enemy));
        }
    }
}
