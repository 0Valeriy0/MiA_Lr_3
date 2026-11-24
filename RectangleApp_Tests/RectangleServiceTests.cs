using RectangleApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RectangleApp.Tests
{
    public class RectangleServiceTests
    {
        private readonly RectangleService _service = new();

        [Theory]
        [InlineData(5, 3, 16)] // Arrange: вхідні дані та очікуваний результат
        [InlineData(2.5, 4, 10)]
        [InlineData(1, 1, 1)]
        public void GetArea_ValidSides_ReturnsProduct(double width, double height, double expected)
        {
            // Act
            double area = _service.GetArea(width, height);

            // Assert
            Assert.Equal(expected, area);
        }

        [Theory]
        [InlineData(5, 3, 16)]
        [InlineData(2.5, 4, 13)]
        [InlineData(1, 1, 4)]
        public void GetPerimeter_ValidSides_ReturnsSumOfAllSides(double width, double height, double expected)
        {
            // Act
            double perimeter = _service.GetPerimeter(width, height);

            // Assert
            Assert.Equal(expected, perimeter);
        }

        [Theory]
        [InlineData(-1, 3)]
        [InlineData(3, -1)]
        [InlineData(-2, -2)]
        public void GetArea_NegativeSide_ThrowsArgumentException(double width, double height)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetArea(width, height));
        }
        [Theory]
        [InlineData(-1, 3)]
        [InlineData(3, -1)]
        public void GetPerimeter_NegativeSide_ThrowsArgumentException(double width, double height)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetPerimeter(width, height));
        }
        [Theory]
        [InlineData(0, 5)]
        [InlineData(5, 0)]
        public void GetArea_ZeroSide_ThrowsArgumentException(double width, double height)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetArea(width, height));
        }

    }
}
