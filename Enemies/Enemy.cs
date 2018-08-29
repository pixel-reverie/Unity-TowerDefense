using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 1;
    private float currentHealth = 1;
    public float speed = 0.5f;

    public int wealth = 1;
    public int score = 10;

    public Color defaultColour;
    public Color damagedColour;

    public MeshRenderer meshRenderer;

    Action OnDieCallback;
    Action OnReachGoalCallback;
    public void Initialise(Action onDieCallback, Action onReachGoalCallback)
    {
        OnDieCallback = onDieCallback;
        OnReachGoalCallback = onReachGoalCallback;
        currentHealth = maxHealth;
    }

    public void OnUpdate()
    {
        this.Move();
    }

    public void Move()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

    public void TakeHit(float damage)
    {
        currentHealth -= damage;

        meshRenderer.material.color = Color.Lerp(defaultColour, damagedColour, 1-(currentHealth / maxHealth));

        if(currentHealth <= 0)
        {
            this.Die();
        }
    }

    public void Die()
    {
        OnDieCallback.Invoke();
        GameObject.Destroy(this.gameObject);
    }
    public void ReachGoal()
    {
        OnReachGoalCallback.Invoke();
        GameObject.Destroy(this.gameObject);
    }
}
