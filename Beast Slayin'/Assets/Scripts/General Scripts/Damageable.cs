using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    public int MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    [SerializeField]
    private int health = 100;
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;

            if (health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool isAlive = true;
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive was set to " + value);

            if (!isAlive)
            {
                // Play death audio clip
                if (deathAudioSource == null)
                {
                    deathAudioSource = gameObject.AddComponent<AudioSource>();
                }

                if (deathClip != null)
                {
                    deathAudioSource.clip = deathClip;
                    deathAudioSource.Play();
                }
                Destroy(gameObject);
            }
        }
    }

    public AudioSource audioSource;
    public AudioSource deathAudioSource;
    public AudioClip dmgClip;
    public AudioClip deathClip;

    public bool isInvincible;
    public bool IsHit
    {
        get
        {
            return animator.GetBool(AnimationStrings.isHit);
        }
        private set
        {
            animator.SetBool(AnimationStrings.isHit, value);

            // Play hit audio clip
            if (audioSource != null && dmgClip != null)
            {
                audioSource.clip = dmgClip;
                audioSource.Play();
            }
        }
    }
    public float iFrames = 0.5f;
    private float timeSinceHit = 0;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > iFrames)
            {
                isInvincible = false;
                timeSinceHit = 0;
                IsHit = false;
            }
            timeSinceHit += Time.deltaTime;
        }
    }

    public bool Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;

            IsHit = true;

            return true;
        }
        else
        {
            return false;
        }
    }
}
