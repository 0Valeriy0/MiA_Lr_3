using System;
using System.Collections.Generic;
using System.Text;

namespace RectangleApp.Services
{
    public class RectangleService
    {
        public double GetArea(double width, double height)
        {
            ValidateSides(width, height);
            return width * height;
        }

        public double GetPerimeter(double width, double height)
        {
            ValidateSides(width, height);
            return 2 * (width + height);
        }

        private static void ValidateSides(double width, double height)
        {
            if (width <= 0)
                throw new ArgumentException("Ширина не може бути від'ємною.", nameof(width));
            if (height <= 0)
                throw new ArgumentException("Висота не може бути від'ємною.", nameof(height));
        }
    }
}
