using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private GameObject Trinity;
    private Damageable trinHP;

    private void Awake()
    {
        Trinity = GameObject.Find("Trinity");
        trinHP = Trinity.GetComponent<Damageable>();
    }

    private void Start()
    {
        UpdateHealthDisplay();
    }

    private void Update()
    {
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        healthDisplay.SetText("HP: " + trinHP.Health + "/" + trinHP.MaxHealth);
    }

}
