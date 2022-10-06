using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    //Parameters
    [Header("Shooter Parameters")]
    [SerializeField] float shootCooldown= 0.5f;
    [SerializeField] float shootRadius = 5;
    [SerializeField] float shootSpeed = 10;
    [SerializeField] float angleAlignmentToShoot = 45;
    [SerializeField] int shootDamage = 1;

    //Object References
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootOrigin;
    [SerializeField] ParticleSystem shootParticle;

    //Internal
    private float shootTimer;

    public void Shoot() {
        Vector2 direction = transform.up;

        Bullet bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.SetDirection(direction * shootSpeed);
        bullet.damage = shootDamage;
        bullet.targetTag = "Player";
        shootParticle.Emit(1);
    }

    protected override void Update() {
        base.Update();
        if (health <= 0) return;
        if (GameManager.Instance.player.GetHealth() <= 0) return;

        Vector2 direction = transform.up;
        Vector2 distanceToPlayer = (GameManager.Instance.player.transform.position - transform.position);
        float angleToPlayer = Vector2.Angle(direction, distanceToPlayer.normalized);

        shootTimer -= Time.deltaTime;
        if (shootTimer < 0 && distanceToPlayer.magnitude < shootRadius && angleToPlayer < angleAlignmentToShoot) {
            shootTimer = shootCooldown;
            Shoot();
        }
    }

}
