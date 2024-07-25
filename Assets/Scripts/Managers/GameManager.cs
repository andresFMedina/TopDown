using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Transform player;
    public Rect levelBounds;
    public GameObject bgPrefab;
    public Transform background;

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
        for (int y = 0; y <= levelBounds.height; y+=100)
        {
            for (int x = 0; x <= levelBounds.width; x+=100)
            {
                var go = Instantiate(bgPrefab, new Vector3(x,y), Quaternion.identity);
                go.transform.parent = background;
            }
        }
    }
}
