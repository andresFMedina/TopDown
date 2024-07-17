using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;
    public GameObject enemy;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {

        for (int i = 0; i < 100; i++)
        {
            float xPosition = Random.Range(51, 1000);
            float yPosition = Random.Range(51, 1000);
            Vector3 position = new(xPosition, yPosition);
            Instantiate(enemy, position, Quaternion.identity);
        }
    }
}
