using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("References")]
    public int health;
    public int maxHealth;

    public Sprite emptyHeart;
    public Sprite fullHeart;

    public Image[] hearts;

    public PlayerHealth playerHealth;

    // Initialize references
    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth == null)
        {
            enabled = false;
            return;
        }

        health = playerHealth.CurrentHealth;
        maxHealth = playerHealth.maxHealth;
    }

    // Update heart display based on current health
    void Update()
    {
        health = playerHealth.CurrentHealth;
        maxHealth = playerHealth.maxHealth;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}