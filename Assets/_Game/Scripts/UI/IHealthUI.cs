public interface IHealthUI
{
    void UpdateHealth(float currentHealth, float maxHealth);
    void SetCharacterName(string name);
    void Show();
    void Hide();
} 