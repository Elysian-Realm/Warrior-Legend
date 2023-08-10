using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent<Transform> OnDie;

    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0) invulnerable = false;
        }
        if (currentPower < maxPower && currentHealth > 0.1f) currentPower += powerRecoverSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water") && currentHealth > 0.1f)
        {
            currentHealth = 0;
            currentPower = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke(other.transform);
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable || currentHealth <= 0) return;

        currentHealth -= attacker.damage;
        if (currentHealth > 0)
        {
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform);//应该用碰撞位置而不是transform，否则陷阱伤害方向会错乱
        }
        else
        {
            currentHealth = 0;
            OnDie?.Invoke(attacker.transform);
        }

        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        invulnerable = true;
        invulnerableCounter = invulnerableDuration;
    }

    public void Onslide(float cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
}
