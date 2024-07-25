using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public QuadTree quadTree { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        quadTree = new(0, GameManager.Instance.levelBounds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (quadTree != null)
        {
            quadTree.DrawDebug();
        }
    }    
}
