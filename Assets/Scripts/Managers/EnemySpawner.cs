using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    private Transform squareEnemies;
    [SerializeField] private AIManager aiManager;
    [SerializeField] private int enemyQuantity = 1000;    

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {

        for (int i = 0; i < enemyQuantity; i++)
        {
            float xPosition = Random.Range(51, 1000);
            float yPosition = Random.Range(51, 1000);
            Vector3 position = new(xPosition, yPosition);
            var go = Instantiate(enemyPrefab, position, Quaternion.identity);
            go.name = $"Square {i + 1}";
            go.transform.parent = squareEnemies;
            if (aiManager != null)
            {
                aiManager.enemies.Add(go.transform);
                aiManager.quadTree.Insert(go.transform);
            }
        }
    }
}
