using System.Collections;

namespace PasswordStrengthApp.Tests.TestData
{
    // Дані для [ClassData]: паролі, що відповідають усім 5 вимогам
    public class StrongPasswordsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { "Abcdef1!" };
            yield return new object[] { "Qwerty#2026" };
            yield return new object[] { "Пароль_123Z" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
