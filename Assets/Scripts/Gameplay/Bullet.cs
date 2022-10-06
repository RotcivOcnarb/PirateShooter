using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public string targetTag;
    public int damage;

    private void Update() {

        Vector2 viewportPosition = GameManager.Instance.mainCamera.WorldToViewportPoint(transform.position);

        if(
            viewportPosition.x < -0.5f ||
            viewportPosition.x > 1.5f ||
            viewportPosition.y < -0.5f ||
            viewportPosition.y > 1.5f
            ) {
            Destroy(gameObject);
        }

    }

    public void SetDirection(Vector2 direction) {
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.SetRotation(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
        body.AddForce(direction, ForceMode2D.Impulse);

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(targetTag)) {
            Ship ship = collision.gameObject.GetComponent<Ship>();
            if (ship) {
                ship.Damage(transform.position, damage);
                Destroy(gameObject);
            }
        }
    }

}
