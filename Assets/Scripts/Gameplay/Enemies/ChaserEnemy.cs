using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemy : Enemy
{
    [Header("Chaser Parameters")]
    [SerializeField] float explosionDistance = 1;
    [SerializeField] int explosionDamage = 10;

    protected override void Update() {
        base.Update();

        if (health <= 0) return;
        if (GameManager.Instance.player.GetHealth() <= 0) return;

        float distanceToPlayer = (transform.position - GameManager.Instance.player.transform.position).magnitude;

        if(distanceToPlayer <= explosionDistance && health > 0) {
            Damage(transform.position, (int)health * 2);
            GameManager.Instance.player.Damage(transform.position, explosionDamage);
            //Explode
        }
    }

}
