using Rotslib.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    //Parameters
    [Header("Ship Parameters")]
    [SerializeField] ProgressBar lifeBar;
    [SerializeField] float maxHealth = 100;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] shipSprites;
    [SerializeField] Sprite destroyedSprite;
    [SerializeField] ParticleSystem explosionParticle;
    [SerializeField] GameObject explosionPrefab;

    //Internal
    protected float health;
    private float tweenHealth;
    private Color shipColor;
    float persistentTime = 5;
    float colorAlpha = 1;

    protected virtual void Awake() {
        health = maxHealth;
        tweenHealth = health;
        shipColor = spriteRenderer.color;
    }

    public float GetHealth() {
        return health;
    }

    protected virtual void Update() {
        tweenHealth += (health - tweenHealth) / 5f * Time.deltaTime * 60f;
        lifeBar.SetValue(tweenHealth, maxHealth);

        shipColor += (Color.black - shipColor) / 5f * Time.deltaTime * 60f;
        shipColor.a = colorAlpha;
        spriteRenderer.color = shipColor;

        if (health > 0) {
            float healthAlpha = health / maxHealth;
            int healthIndex = Mathf.Clamp((int)Mathf.Round(healthAlpha * shipSprites.Length), 0, shipSprites.Length - 1);
            spriteRenderer.sprite = shipSprites[healthIndex];
        }
        else {
            spriteRenderer.sprite = destroyedSprite;
        }

        if(health <= 0) {
            lifeBar.gameObject.SetActive(false);
            persistentTime -= Time.deltaTime;
            if(persistentTime < 0) {
                colorAlpha -= Time.deltaTime;
                if(colorAlpha < 0) {
                    Destroy(gameObject);
                }
            }
        }
    }

    public virtual bool Damage(Vector2 position, int damage) {
        if (health <= 0) return false;

        health -= damage;
        if(health <= 0) {
            //Animação de destruir
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else {
            //Animação de dano
            ParticleSystem.EmitParams pm = new ParticleSystem.EmitParams() {
                position = position,
            };
            explosionParticle.Emit(pm, 1);
            shipColor = Color.white;
        }
        return true;
    }
}
