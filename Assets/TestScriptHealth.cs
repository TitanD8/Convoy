using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptHealth : MonoBehaviour
{
    public HealthBar health;
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = ((int)health.slider.maxValue);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            health.SetHealth(currentHealth -= 1);
            currentHealth -= 1;
        }
    }
}
