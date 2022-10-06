using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Rotslib.Utils;
using Rotslib.UI;
using UnityEngine.UI;

public class Player : Ship
{
    //Object References
    [Header("Player Parameters")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform simpleShootOrigin;
    [SerializeField] Transform tripleShootOrigin;
    [SerializeField] ParticleSystem shootParticle;
    [SerializeField] Image hurtHudImage;

    //Parameters
    [SerializeField] float rotationalSpeed = 1;
    [SerializeField] float rotationalAcceleration = 1;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] float moveAcceleration = 1;
    [SerializeField] float simpleShootCooldown = 0.3f;
    [SerializeField] float simpleShootSpeed = 1;
    [SerializeField] float tripleShootDistance = .4f;


    //Internal
    private int rotateInput = 0;
    private int moveInput = 0;
    private Rigidbody2D body;
    private bool shooting = false;
    private float shootTimer;
    private float hurtHudAlpha = 0;


    protected override void Awake() {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
    }

    void LateUpdate() {
        if(health <= 0) {
            rotateInput = 0;
            moveInput = 0;
        }

        float targetTorque = rotateInput * rotationalSpeed;
        body.AddTorque((targetTorque - body.angularVelocity) * rotationalAcceleration);

        Vector2 direction = transform.up;

        Vector2 targetVelocity = direction * moveInput * moveSpeed;
        body.AddForce((targetVelocity - body.velocity) * moveAcceleration);
    }

    protected override void Update() {
        base.Update();
        shootTimer -= Time.deltaTime;
        if(shooting && shootTimer < 0) {
            shootTimer = simpleShootCooldown;
            ShootSimple();
        }
        hurtHudAlpha += (0 - hurtHudAlpha) / 10f * Time.deltaTime * 60f;
        hurtHudImage.color = new Color(1, 1, 1, hurtHudAlpha);
    }

    public void ShootSimple() {
        Vector2 direction = transform.up;

        Bullet bullet = Instantiate(bulletPrefab, simpleShootOrigin.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.SetDirection(direction * simpleShootSpeed);
        bullet.damage = 30;
        bullet.targetTag = "Enemy";

        shootParticle.Emit(1);
    }

    public void ShootTriple() {
        Vector2 direction = transform.right;

        for(int i = -1; i <= 1; i++) {
            Vector2 position = tripleShootOrigin.position;
            position += (Vector2) transform.up * i * tripleShootDistance;
            Bullet bullet = Instantiate(bulletPrefab, position, Quaternion.identity).GetComponent<Bullet>();
            bullet.SetDirection(direction * simpleShootSpeed);
            bullet.damage = 3;
            bullet.targetTag = "Enemy";
        }
    }

    public override bool Damage(Vector2 position, int damage) {
        if (base.Damage(position, damage)) {
            hurtHudAlpha = 1;

            if (health <= 0) {
                GameManager.Instance.GameOver();
            }
            return true;
        }
        return false;
    }

    //Inputs

    public void InputRotateCW(InputAction.CallbackContext context) {

        if(context.phase == InputActionPhase.Started) {
            rotateInput = -1;
        }
        else if(context.phase == InputActionPhase.Canceled && rotateInput == -1) {
            rotateInput = 0;
        }
    }

    public void InputRotateCCW(InputAction.CallbackContext context) {

        if (context.phase == InputActionPhase.Started) {
            rotateInput = 1;
        }
        else if (context.phase == InputActionPhase.Canceled && rotateInput == 1) {
            rotateInput = 0;
        }
    }

    public void InputMoveForward(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            moveInput = 1;
        }
        else if (context.phase == InputActionPhase.Canceled && moveInput == 1) {
            moveInput = 0;
        }
    }

    public void InputMoveBackwards(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            moveInput = -1;
        }
        else if (context.phase == InputActionPhase.Canceled && moveInput == -1) {
            moveInput = 0;
        }
    }

    public void InputShootSimple(InputAction.CallbackContext context) {
        if(context.phase == InputActionPhase.Started) {
            shooting = true;
        }
        else if(context.phase == InputActionPhase.Canceled) {
            shooting = false;
        }
    }

    public void InputShootTriple(InputAction.CallbackContext context) {
        if (context.phase == InputActionPhase.Started) {
            ShootTriple();
        }
    }
}
