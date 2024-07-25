using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    public List<Transform> enemies = new();
    private Transform squareEnemies;
    [SerializeField]
    private AIManager aiManager;
    [SerializeField]
    private float updateTreeInterval = 0.5f;
    private float nextUpdateTreeInterval;

    private void Start()
    {
        SpawnEnemies();
    }

    private void Update()
    {
        if (Time.time > nextUpdateTreeInterval && aiManager != null)
        {
            UpdateEnemies();
            UpdateQuadTree();
            nextUpdateTreeInterval = Time.time + updateTreeInterval;
        }
    }

    private void SpawnEnemies()
    {

        for (int i = 0; i < 1000; i++)
        {
            float xPosition = Random.Range(51, 1000);
            float yPosition = Random.Range(51, 1000);
            Vector3 position = new(xPosition, yPosition);
            var go = Instantiate(enemyPrefab, position, Quaternion.identity);
            go.transform.parent = squareEnemies;
            enemies.Add(go.transform);
            if(aiManager != null) aiManager.quadTree.Insert(go.transform);
        }
    }

    void UpdateEnemies()
    {
        List<Transform> nearbyEnemies = new List<Transform>();
        var player = GameManager.Instance.player;
        nearbyEnemies = aiManager.quadTree.Retrieve(nearbyEnemies, new Rect(player.position.x, player.position.y, 0, 0));

        foreach (Transform enemy in enemies)
        {
            if (nearbyEnemies.Contains(enemy))
            {
                EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
                if (enemyScript != null && !enemyScript.enabled)
                {
                    enemyScript.enabled = true;
                }
            }
            else
            {
                EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
                if (enemyScript != null && enemyScript.enabled)
                {
                    enemyScript.enabled = false;
                }
            }
        }
    }

    void UpdateQuadTree()
    {
        aiManager.quadTree.Clear();

        foreach (Transform enemy in enemies)
        {
            aiManager.quadTree.Insert(enemy);
        }
    }
}
