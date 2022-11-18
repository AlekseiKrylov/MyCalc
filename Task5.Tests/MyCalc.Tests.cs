namespace Task5.Tests
{
    [TestClass]
    public class MyCalcTests
    {
        [TestMethod]
        public void SimpleFormula()
        {
            MyCalc calc = new("-2+2*3");
            string expectedValue = "4";
            string actualValue = calc.GetResult();
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void FormulaWithRoundBrackets()
        {
            MyCalc calc = new("(5-6*6/8+(-2+5+(2*5)+(1.5*5)))+9.4");
            string expectedValue = "30.4";
            string actualValue = calc.GetResult();
            Assert.AreEqual(expectedValue, actualValue);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void NullExeption()
        {
            MyCalc calcNull = new(null);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void EmptyExeption()
        {
            MyCalc calcEmpty = new("");
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void NotFormulaExeption()
        {
            MyCalc calcEmpty = new("1+x+4");
        }

        [ExpectedException(typeof(DivideByZeroException))]
        [TestMethod]
        public void DivideByZeroExeption()
        {
            MyCalc calcEmpty = new("2/0");
        }
    }
}