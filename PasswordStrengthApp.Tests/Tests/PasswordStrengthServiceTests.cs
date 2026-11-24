using PasswordStrengthApp.Services;
using PasswordStrengthApp.Tests.TestData;

namespace PasswordStrengthApp.Tests
{
    public class PasswordStrengthServiceTests
    {
        private readonly PasswordStrengthService _service = new();

        // ===================== [Fact] =====================

        [Fact]
        public void GetStrengthScore_StrongPassword_ReturnsMaxScore()
        {
            // Arrange
            string password = "Abcdef1!";

            // Act
            int score = _service.GetStrengthScore(password);

            // Assert — перевірка значень
            Assert.Equal(PasswordStrengthService.MaxScore, score);
        }

        [Fact]
        public void GetStrengthScore_WeakAndStrong_ReturnDifferentScores()
        {
            // Arrange
            string weakPassword = "abc";
            string strongPassword = "Abcdef1!";

            // Act
            int weak = _service.GetStrengthScore(weakPassword);
            int strong = _service.GetStrengthScore(strongPassword);

            // Assert
            Assert.NotEqual(weak, strong);
        }

        [Fact]
        public void GetStrengthScore_Null_ThrowsArgumentNullException()
        {
            // Arrange
            string password = null!;

            // Act & Assert — перевірка винятків
            var ex = Assert.Throws<ArgumentNullException>(() => _service.GetStrengthScore(password));
            Assert.Equal("password", ex.ParamName);
        }

        [Fact]
        public void GetFirstMissingRequirement_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetFirstMissingRequirement(null!));
        }

        [Fact]
        public void IsStrong_WeakPassword_ReturnsFalse()
        {
            // Arrange
            string password = "password";

            // Act
            bool result = _service.IsStrong(password);

            // Assert — булеві умови
            Assert.False(result);
        }

        [Fact]
        public void GetStrengthScore_AnyPassword_IsInValidRange()
        {
            // Arrange
            string password = "Hello123";

            // Act
            int score = _service.GetStrengthScore(password);

            // Assert — діапазони
            Assert.InRange(score, 0, PasswordStrengthService.MaxScore);
        }

        [Fact]
        public void GetStrengthScore_StrongPassword_NotInWeakRange()
        {
            // Arrange
            string password = "Abcdef1!";

            // Act
            int score = _service.GetStrengthScore(password);

            // Assert
            Assert.NotInRange(score, 0, 2);
        }

        [Fact]
        public void GetFirstMissingRequirement_PerfectPassword_ReturnsNull()
        {
            // Arrange
            string password = "Abcdef1!";

            // Act
            string? result = _service.GetFirstMissingRequirement(password);

            // Assert — референси
            Assert.Null(result);
        }

        [Fact]
        public void GetFirstMissingRequirement_NoDigit_ReturnsDigitHint()
        {
            // Arrange
            string password = "Abcdefgh";

            // Act
            string? result = _service.GetFirstMissingRequirement(password);

            // Assert — рядки
            Assert.NotNull(result);
            Assert.StartsWith("Додайте", result);
            Assert.Contains("цифру", result);
        }

        [Fact]
        public void GetStrengthLevel_AnyPassword_MatchesAllowedValues()
        {
            // Arrange
            string password = "Abc12345";

            // Act
            string level = _service.GetStrengthLevel(password);

            // Assert — регулярний вираз
            Assert.Matches("^(Weak|Medium|Strong)$", level);
        }

        [Fact]
        public void PrepareInput_NoSpaces_ReturnsSameInstance()
        {
            // Arrange
            string input = "Abcdef1!";

            // Act
            string result = _service.PrepareInput(input);

            // Assert — той самий об'єкт (Trim нічого не обрізав)
            Assert.Same(input, result);
        }

        [Fact]
        public void PrepareInput_WithSpaces_ReturnsNewInstance()
        {
            // Arrange
            string input = "  Abcdef1!  ";

            // Act
            string result = _service.PrepareInput(input);

            // Assert — новий об'єкт рядка
            Assert.NotSame(input, result);
            Assert.Equal("Abcdef1!", result);
        }

        //  [Theory] + [InlineData] 

        [Theory]
        [InlineData("", 0)]          // граничний: порожній рядок
        [InlineData("abc", 1)]       // тільки малі літери
        [InlineData("abcdefgh", 2)]  // рівно 8 символів (межа довжини)
        [InlineData("Abcdefgh", 3)]
        [InlineData("Abcdefg1", 4)]
        [InlineData("Abcdef1!", 5)]
        public void GetStrengthScore_VariousPasswords_ReturnsExpectedScore(string password, int expected)
        {
            // Act
            int score = _service.GetStrengthScore(password);

            // Assert
            Assert.Equal(expected, score);
        }

        // [Theory] + [MemberData] 

        public static IEnumerable<object[]> LevelData => new List<object[]>
        {
            new object[] { "abc", "Weak" },
            new object[] { "12345678", "Weak" },
            new object[] { "Abcdefgh", "Medium" },
            new object[] { "Abcdefg1", "Medium" },
            new object[] { "Abcdef1!", "Strong" }
        };

        [Theory]
        [MemberData(nameof(LevelData))]
        public void GetStrengthLevel_VariousPasswords_ReturnsExpectedLevel(string password, string expected)
        {
            // Act
            string level = _service.GetStrengthLevel(password);

            // Assert
            Assert.Equal(expected, level);
        }

        // [Theory] + [ClassData] 

        [Theory]
        [ClassData(typeof(StrongPasswordsData))]
        public void IsStrong_StrongPasswords_ReturnsTrue(string password)
        {
            // Act
            bool result = _service.IsStrong(password);

            // Assert
            Assert.True(result);
        }
    }
}
