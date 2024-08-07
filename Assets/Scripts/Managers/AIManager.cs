using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public QuadTree quadTree { get; private set; }
    private PlayerMovement player;
    public List<Transform> enemies;
    public List<string> nearEnemies = new();

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

        UpdateEnemies();
        UpdateQuadTree();

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
}
