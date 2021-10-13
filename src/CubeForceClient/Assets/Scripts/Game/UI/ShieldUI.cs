using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    public float shield_cooldown;
    public Slider slider;
    private Coroutine coroutine;
    private bool coroutine_running = false;

    private void Start() {
        slider.value = 1;
    }
    public void updateShield(bool shield_ready)
    {
        if (!shield_ready)
        {
            coroutine = StartCoroutine(shieldCooldownAnimation(shield_cooldown));
        }
        else
        {
            if (coroutine_running)
            {
                StopCoroutine(coroutine);
                coroutine_running = false;
                slider.value = 1;
            }
        }
    }


    private IEnumerator shieldCooldownAnimation(float shield_cooldown)
    {
        coroutine_running = true;
        slider.value = 0.0f;
        int steps = 50;
        float step_size = (float)1 / steps;
        for (int i = 0; i < steps; i++)
        {
            yield return new WaitForSeconds(shield_cooldown / steps);
            slider.value += step_size;
        }
        coroutine_running = false;
    }
}
