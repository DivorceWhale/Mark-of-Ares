using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class HealthBar : MonoBehaviour
{
    [Header("Optional Settings")]
    public Slider slider;           // Assign manually or auto-detect
    public Image fill;              // Assign manually or auto-detect
    public Gradient gradient;       // Optional: color based on health

    private void Awake()
    {
        // Auto-assign slider if missing
        if (slider == null)
            slider = GetComponent<Slider>();

        // Auto-assign fill image if missing
        if (fill == null && slider != null && slider.fillRect != null)
            fill = slider.fillRect.GetComponent<Image>();

        if (slider == null)
            Debug.LogError("HealthBar: No Slider found on " + gameObject.name);
        if (fill == null)
            Debug.LogWarning("HealthBar: No Fill image assigned for " + gameObject.name);
    }

    public void SetMaxHealth(int health)
    {
        if (slider == null) return;

        slider.maxValue = health;
        slider.value = health;

        UpdateFillColor();
    }

    public void SetHealth(int health)
    {
        if (slider == null) return;

        slider.value = health;

        if (fill != null && gradient != null)
            fill.color = gradient.Evaluate(slider.normalizedValue);

        // DEBUG LOG
        Debug.Log("HealthBar updated: " + health + " | Fill width: " + (fill != null ? fill.rectTransform.rect.width : 0));

        UpdateFillColor();
    }

    private void UpdateFillColor()
    {
        if (fill != null && gradient != null)
        {
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }
    }
}
