using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    public Animator animator;
    public int RespawnIndex;
    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    private float lerpTimer;
    public Image frontHealthBar, backHealthBar;
    public TextMeshProUGUI textHealth;
    private int killed;
    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;
    public TextMeshProUGUI killedText;
    public int enemiesOnScene;

    void Start()
    {
        animator.SetBool("IsDead", false);
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0f);
        killedText.text = "Killed: 0 / " + enemiesOnScene.ToString();
        killed = 0;
    }

    void Update()
    {
        killedText.text = "Killed: " + killed.ToString() + " / " + enemiesOnScene.ToString();
        health = Mathf.Clamp(health, 0f, maxHealth);
        UpdateHealthUI();
        if (overlay.color.a > 0f) {
            if (health < 30f) {
                return;
            }
            durationTimer += Time.deltaTime;
            if (durationTimer > duration) {
                // fade overlay
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI() {
        textHealth.text = health + "";
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillBack > hFraction) // player taken damage 
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);
        }
        if (fillFront < hFraction) { // player gained health
            backHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.green;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void UpdateKilledText() {
        killed++;
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            Die();
            return;
        }
        lerpTimer = 0f;
        durationTimer = 0f;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
        SoundManager.PlaySound(SoundType.GRUNT, 1);
    }

    public void RestoreHealth(float heal) {
        health += heal;
        lerpTimer = 0f;
        SoundManager.PlaySound(SoundType.HEAL, 1);
    }

    private void Die() {
        StartCoroutine(Death());
    }

    private IEnumerator Death() {
        animator.SetBool("IsDead", true);
        SoundManager.PlaySound(SoundType.PLAYER_DEATH_GRUNT, 1);
        yield return new WaitForSeconds(2);
        SoundManager.PlaySound(SoundType.PLAYER_DEATH_EFFECT, 1);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(RespawnIndex);
    }

}
