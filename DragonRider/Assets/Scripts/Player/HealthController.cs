using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    //
    [Header("Paramaters")]
    public int maxHealth = 100;
    [Header("UI")]
    public Image healthBarImage;

    //
    private int currentHealth = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamage()
    {
        Debug.Log("Damage received, current health: " + currentHealth);
        currentHealth--;
        healthBarImage.fillAmount = (float)currentHealth / (float) maxHealth;
    }
}
