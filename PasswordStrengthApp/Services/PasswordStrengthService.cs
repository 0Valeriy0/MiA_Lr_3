namespace PasswordStrengthApp.Services
{
    /// <summary>
    /// Оцінка складності пароля (ЛР5, Завдання 1, варіант 4).
    /// Бал від 0 до 5: довжина ≥ 8, велика літера, мала літера, цифра, спецсимвол.
    /// </summary>
    public class PasswordStrengthService
    {
        public const int MinLength = 8;
        public const int MaxScore = 5;

        // 1. Основний метод: бал від 0 до 5
        public int GetStrengthScore(string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            int score = 0;
            if (password.Length >= MinLength) score++;
            if (password.Any(char.IsUpper)) score++;
            if (password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(IsSpecial)) score++;
            return score;
        }

        // 2. Рівень складності: "Weak" / "Medium" / "Strong"
        public string GetStrengthLevel(string password)
        {
            int score = GetStrengthScore(password);
            return score switch
            {
                <= 2 => "Weak",
                <= 4 => "Medium",
                _ => "Strong"
            };
        }

        // 3. Чи пароль сильний (виконані всі 5 умов)
        public bool IsStrong(string password) => GetStrengthScore(password) == MaxScore;

        // 4. Перша невиконана вимога; null, якщо пароль відповідає всім вимогам
        public string? GetFirstMissingRequirement(string password)
        {
            ArgumentNullException.ThrowIfNull(password);

            if (password.Length < MinLength) return $"Додайте символів: мінімум {MinLength}";
            if (!password.Any(char.IsUpper)) return "Додайте велику літеру";
            if (!password.Any(char.IsLower)) return "Додайте малу літеру";
            if (!password.Any(char.IsDigit)) return "Додайте цифру";
            if (!password.Any(IsSpecial)) return "Додайте спеціальний символ";
            return null;
        }

        // 5. Підготовка введення: обрізає пробіли по краях
        public string PrepareInput(string password)
        {
            ArgumentNullException.ThrowIfNull(password);
            return password.Trim();
        }

        private static bool IsSpecial(char c) => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c);
    }
}
