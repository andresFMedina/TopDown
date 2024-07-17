using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForce : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1.5f;
    [SerializeField]
    private float minChaseDistance = 50f;

    Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(transform.position, GameManager.Instance.player.position);
        if (distance < minChaseDistance)
        {

            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (GameManager.Instance.player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }
}
