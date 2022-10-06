using Rotslib.UI;
using Rotslib.Utils;
using UnityEngine;
public abstract class Enemy : Ship
{
    //Parameters
    [Header("Enemy Parameters")]
    [SerializeField] float steerSpeed = 10;
    [SerializeField] float steerAcceleration = 0.1f;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] float moveAcceleration = 1;
    [SerializeField] float stopPointRadius = 1;

    //Internal
    protected Rigidbody2D body;

    protected override void Awake() {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void LateUpdate() {
        if (health <= 0) return;
        if (GameManager.Instance.player.GetHealth() <= 0) return;

        Vector2 direction = transform.up;
        Vector2 distanceToPlayer = (GameManager.Instance.player.transform.position - transform.position);
        Vector2 directionToPlayer = distanceToPlayer.normalized;

        foreach (Enemy enemy in GameManager.Instance.GetSpawnedEnemies()) {
            if (enemy != null && enemy != this) {
                Vector2 distanceToEnemy = transform.position - enemy.transform.position;
                float distanceFactor = 1 / distanceToEnemy.sqrMagnitude;
                directionToPlayer += distanceToEnemy.normalized * distanceFactor;
            }
        }

        float targetAngularSpeed = Mathf.Sign(Vector2.SignedAngle(direction, directionToPlayer)) * steerSpeed;



        body.AddTorque((targetAngularSpeed - body.angularVelocity) * steerAcceleration);

        float proximityFactor = Mathf.Min(distanceToPlayer.magnitude - stopPointRadius, 1);

        Vector2 targetVelocity = direction * moveSpeed * proximityFactor;
        body.AddForce((targetVelocity - body.velocity) * moveAcceleration);

    }

    public override bool Damage(Vector2 position, int damage) {
        if (base.Damage(position, damage)) {
            if (health <= 0) {
                GameManager.Instance.IncreaseScore(1);
            }
            return true;
        }
        return false;
    }

}
