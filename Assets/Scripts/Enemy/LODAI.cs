using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LODAI : MonoBehaviour
{
    private EnemyMovement enemyMovement;
    [SerializeField]
    private float minActivationDistance = 50f;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        enemyMovement.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, GameManager.Instance.player.position);
        if (distance < minActivationDistance)
        {
            if(enemyMovement != null && !enemyMovement.enabled)
            {
                enemyMovement.enabled = true;
                return;
            }

            enemyMovement.enabled = false;
        }
    }
}
