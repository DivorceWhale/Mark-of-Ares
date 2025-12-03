using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public PlayerHealth player;  // Assign in Inspector
    public TMP_Text healthText;  // Assign in Inspector

    private void Update()
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (player != null && healthText != null)
        {
            healthText.text = $"{player.CurrentHealth}";
        }
    }
}
