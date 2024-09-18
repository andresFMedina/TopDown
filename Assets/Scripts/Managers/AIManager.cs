using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public QuadTree quadTree { get; private set; }
    private PlayerMovement player;
    public readonly HashSet<Transform> enemies = new();
    public readonly HashSet<Transform> nearEnemies = new();
    [SerializeField] private float updateTreeInterval = 1f;    
    [SerializeField] private float maxRangeEnemiesUpdate = 200f;

    private void Awake()
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
        StartCoroutine(UpdateEnemiesCorutine());
    }

    private IEnumerator UpdateEnemiesCorutine()
    {
        var enemies = player.GetNearEnemies();
        //var farEnemies = nearEnemies.Where(e => enemies.Any(ne => e.transform));
        var farEnemies = new HashSet<Transform>();

        foreach(var nearEnemy in nearEnemies)
        {
            if (!enemies.Contains(nearEnemy.transform))
            {
                farEnemies.Add(nearEnemy);
            }

        }

        print(farEnemies.Count());
        foreach(var farEnemy in farEnemies)
        {
            print("Far Enemy: " + farEnemy.name);
            var enemyMovement = farEnemy.GetComponent<EnemyMovement>();
            enemyMovement.enabled = false;
            nearEnemies.Remove(farEnemy);
        }
        foreach (var enemy in enemies)
        {
            var enemyMovement = enemy.transform.GetComponent<EnemyMovement>();
            enemyMovement.enabled = true;
            nearEnemies.Add(enemy);
        }

        yield return new WaitForSeconds(updateTreeInterval);

        //UpdateTree();
        UpdateEnemies();
        UpdateQuadTree();

        yield return null;
    }    

    void UpdateEnemies()
    {
        List<Transform> nearbyEnemies = new List<Transform>();
        var player = GameManager.Instance.player;
        nearbyEnemies = quadTree.Retrieve(nearbyEnemies, new Rect(player.position.x, player.position.y, 0, 0));

        foreach (Transform enemy in enemies.Where(e => Math.Abs(Vector2.Distance(player.position, e.position)) < maxRangeEnemiesUpdate))
        {            
            if (nearEnemies.Count > 0 && nearEnemies.Contains(enemy)) continue;
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
            enemyScript.enabled = nearbyEnemies.Contains(enemy);
        }
    }

    private void UpdateTree()
    {
        UpdateQuadTreeAsync().Wait();
        var nearbyEnemies = UpdateEnemiesAsync().Result;
        foreach (Transform enemy in enemies)
        {
            if (nearEnemies.Contains(enemy)) continue;
            EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
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

    async Task<List<Transform>> UpdateEnemiesAsync()
    {
        List<Transform> nearbyEnemies = new List<Transform>();
        var player = GameManager.Instance.player;
        return await Task.Run(() => quadTree.Retrieve(nearbyEnemies, new Rect(player.position.x, player.position.y, 0, 0)));
    }

    async Task UpdateQuadTreeAsync()
    {
        quadTree.Clear();

        await Task.Run(() =>
        {
            foreach (Transform enemy in enemies)
            { 
                quadTree.Insert(enemy); 
            }
        });

    }
}
