using Microsoft.VisualStudio.TestTools.UnitTesting;
using otinova_lib;

namespace otinova_lib_tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void Add_TwoNumbers_ReturnsSum()
        {
            double a = 5;
            double b = 3;
            double expected = 8;

            double actual = Calculator.Add(a, b);

            Assert.AreEqual(expected, actual, 0.001, "Сложение работает неправильно");
        }

        [TestMethod]
        public void Subtract_TwoNumbers_ReturnsDifference()
        {
            double a = 10;
            double b = 4;
            double expected = 6;

            double actual = Calculator.Subtract(a, b);

            Assert.AreEqual(expected, actual, 0.001, "Вычитание работает неправильно");
        }

        [TestMethod]
        public void Multiply_TwoNumbers_ReturnsProduct()
        {
            double a = 6;
            double b = 7;
            double expected = 42;

            double actual = Calculator.Multiply(a, b);

            Assert.AreEqual(expected, actual, 0.001, "Умножение работает неправильно");
        }

        [TestMethod]
        public void Divide_TwoNumbers_ReturnsQuotient()
        {
            double a = 15;
            double b = 3;
            double expected = 5;

            double actual = Calculator.Divide(a, b);

            Assert.AreEqual(expected, actual, 0.001, "Деление работает неправильно");
        }

        [TestMethod]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            double a = 10;
            double b = 0;

            Assert.ThrowsException<DivideByZeroException>(() => Calculator.Divide(a, b));
        }

        [TestMethod]
        public void Divide_ByZero_ThrowsDivideByZeroException_WithCorrectMessage()
        {
            double a = 10;
            double b = 0;

            try
            {
                Calculator.Divide(a, b);
                Assert.Fail("Исключение не было выброшено");
            }
            catch (DivideByZeroException ex)
            {
                StringAssert.Contains(ex.Message, Calculator.DivideByZeroMessage);
            }
        }

        [TestMethod]
        public void Power_NumberAndExponent_ReturnsPoweredValue()
        {
            double a = 2;
            double b = 3;
            double expected = 8;

            double actual = Calculator.Power(a, b);

            Assert.AreEqual(expected, actual, 0.001, "Возведение в степень работает неправильно");
        }
    }
}