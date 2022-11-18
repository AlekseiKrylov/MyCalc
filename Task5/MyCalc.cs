using System.Globalization;
using System.Text.RegularExpressions;

namespace Task5
{
    public class MyCalc
    {
        private const string PATTERN_NUMBER = @"-?\d+(?:\.?\d+)?";
        private const string PATTERN_MATH_SYMBOL = @"[-+*\/]";
        private const string PATTERN_FIRST_MATH_OPERATION = @"[*\/]";
        private const string PATTERN_SECOND_MATH_OPERATION = @"[-+]";
        private const string PATTERN_ROUND_BRACKETS = @"\(([^\(\)]*)\)";
        private const string PATTERN_INCORRECT_ROUND_BRACKETS = @"(?:\d+\()|(?:\)\d+)";
        private static readonly string _patternSimpleMathFormula = string.Format("^{0}{1}{0}(?:{1}{0})*$", PATTERN_NUMBER, PATTERN_MATH_SYMBOL);
        private string _originalString;
        private List<string> _listFromFile;
        private string _resultString;
        private List<string> _resultListFromFile;

        public MyCalc(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("You didn't enter anything.");

            bool isMathEx = CheckMathFormula(input);
            if (isMathEx)
            {
                _originalString = input;
                _resultString = CalculateMathFormula(_originalString).ToString(CultureInfo.InvariantCulture);
                return;
            }

            if (!File.Exists(input))
                throw new ArgumentException($"Exception. Wrong input. '{input}' is not a mathematical expression or is not a file path or file not found.");

            FileToList(input);
            CalcFromFile();
            ListToFile();
        }

        public string GetResult()
        {
            if (_resultListFromFile is not null)
                return $"The result has saved in: {_resultString}";

            return _resultString;
        }

        private bool CheckMathFormula(string input)
        {
            bool haveRoundBrackets = Regex.IsMatch(input, PATTERN_ROUND_BRACKETS);
            if (!haveRoundBrackets)
                return CheckSimpleMathFormula(input);

            bool incorrectRoundBrackets = Regex.IsMatch(input, PATTERN_INCORRECT_ROUND_BRACKETS);
            if (incorrectRoundBrackets)
                return false;

            string replaseRoundBrackets = "1";
            string checkString = input;
            do
            {
                MatchCollection roundBracketsCollection = Regex.Matches(checkString, PATTERN_ROUND_BRACKETS);
                for (int i = 0; i < roundBracketsCollection.Count; i++)
                {
                    string tmpChekString = roundBracketsCollection[i].Groups[1].Value;
                    bool isMathEx = CheckSimpleMathFormula(tmpChekString);
                    if (!isMathEx)
                        return false;
                }

                checkString = Regex.Replace(checkString, PATTERN_ROUND_BRACKETS, replaseRoundBrackets);
                if (checkString == "1")
                    return true;

                haveRoundBrackets = Regex.IsMatch(checkString, PATTERN_ROUND_BRACKETS);
                if (!haveRoundBrackets)
                    return CheckSimpleMathFormula(checkString);

            } while (haveRoundBrackets);
            return true;
        }

        private bool CheckSimpleMathFormula(string input)
        {
            bool matchSimpleMathFormula = Regex.IsMatch(input, _patternSimpleMathFormula);
            if (!matchSimpleMathFormula)
                return false;
            return true;
        }

        private void FileToList(string filePath)
        {
            if (new FileInfo(filePath).Length == 0)
                throw new ArgumentException("File is empty");

            _listFromFile = File.ReadAllLines(filePath).ToList();
            _originalString = filePath;
        }

        private void ListToFile()
        {
            string originalFileName = Path.GetFileName(_originalString);
            string originalPath = _originalString.Remove(_originalString.Length - originalFileName.Length);
            _resultString = $"{originalPath}result_{originalFileName}";
            File.WriteAllLines(_resultString, _resultListFromFile);
        }

        private decimal CalculateMathFormula(string input)
        {
            var matchBrackets = Regex.Match(input, PATTERN_ROUND_BRACKETS);
            if (!matchBrackets.Success)
                return CalculateSimpleMathFormula(input);

            string inBrackets = matchBrackets.Groups[1].Value;
            string left = input.Substring(0, matchBrackets.Index);
            string right = input.Substring(matchBrackets.Index + matchBrackets.Length);
            string resaultCalculating = CalculateSimpleMathFormula(inBrackets).ToString(CultureInfo.InvariantCulture);
            return CalculateMathFormula(left + resaultCalculating + right);
        }

        private decimal CalculateSimpleMathFormula(string input)
        {
            var matchFitstMathOperation = Regex.Match(input, string.Format("({0})({1})({0})", PATTERN_NUMBER, PATTERN_FIRST_MATH_OPERATION));
            var matchSecongMathOperation = Regex.Match(input, string.Format("({0})({1})({0})", PATTERN_NUMBER, PATTERN_SECOND_MATH_OPERATION));
            var match = matchFitstMathOperation.Success == true ? matchFitstMathOperation : matchSecongMathOperation.Success == true ? matchSecongMathOperation : null;

            if (match != null)
            {
                string left = input.Substring(0, match.Index);
                string right = input.Substring(match.Index + match.Length);
                string resultCalculating = Calculate(match).ToString(CultureInfo.InvariantCulture);
                return CalculateSimpleMathFormula(left + resultCalculating + right);
            }

            if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal numbevr))
                throw new FormatException($"Exception. Wrong input string '{input}'");

            return decimal.Parse(input, CultureInfo.InvariantCulture);
        }

        private decimal Calculate(Match match)
        {
            decimal a = decimal.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            decimal b = decimal.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            string operationSymbol = match.Groups[2].Value;

            switch (operationSymbol)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b == 0)
                        throw new DivideByZeroException("Exception. Divide by zero.");
                    return a / b;
                default: throw new FormatException($"Exception. Wrong input string '{match.Value}'");
            }
        }

        private void CalcFromFile()
        {
            _resultListFromFile = new List<string>();

            foreach (string item in _listFromFile)
            {
                bool isMathEx = CheckMathFormula(item);
                if (!isMathEx)
                    _resultListFromFile.Add($"{item} = Exception. Wrong input.");
                else
                {
                    try
                    {
                        string result = CalculateMathFormula(item).ToString(CultureInfo.InvariantCulture);
                        _resultListFromFile.Add($"{item} = {result}");
                    }
                    catch (DivideByZeroException ex)
                    {
                        _resultListFromFile.Add($"{item} = {ex.Message}");
                    }
                    catch (FormatException ex)
                    {
                        _resultListFromFile.Add($"{item} = {ex.Message}");
                    }
                }
            }
        }
    }
}
