using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float damage = 0.2f;
    public float coolDownSpeed = 1.0f;
    public int cost = 1;
    public float range = 2;
    public string towerName = "tower";

    public Color defaultColour;
    public Color cooldownColour;

    public MeshRenderer meshRenderer;

    private float cooldown = 0.0f;

    public void Initialise()
    {
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.radius = range;
    }

    private void OnTriggerStay(Collider trigger)
    {
        Enemy enemy = trigger.GetComponent<Enemy>();

        if(!enemy) { return; }

        if (cooldown <= 0) { AttackEnemy(enemy); }
    }

    void AttackEnemy(Enemy enemy)
    {
        cooldown = 1.0f;
        enemy.TakeHit(damage);
    }

    public void OnUpdate()
    {
        cooldown -= Time.deltaTime * coolDownSpeed;
        if (cooldown <= 0) { cooldown = 0; }

        meshRenderer.material.color = Color.Lerp(defaultColour, cooldownColour, cooldown);
    }
}