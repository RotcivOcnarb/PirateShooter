using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTrail : MonoBehaviour
{
    [SerializeField] ParticleSystem trailParticle;
    [SerializeField] float trailTimeout = 0.2f;

    private float trailTimer = 0;
    private Rigidbody2D body;

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Vector2 direction = transform.up;

        trailTimer -= Time.deltaTime;
        while (trailTimer < 0) {
            trailTimer += trailTimeout;
            ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
            ep.position = trailParticle.transform.position;
            ep.rotation = 90 - Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            ep.velocity = direction * 0.5f;
            int emission = Mathf.Min(1, (int)Mathf.Round(body.velocity.magnitude));
            trailParticle.Emit(ep, emission);
        }
    }
}
