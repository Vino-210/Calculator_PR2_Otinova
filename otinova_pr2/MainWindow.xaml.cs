using otinova_lib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace otinova_pr2
{
    public partial class MainWindow : Window
    {
        private bool _isNewExpression = true;

        public MainWindow()
        {
            InitializeComponent();
            ResultBox.Focus();
        }

        private static bool IsOperator(string s)
        {
            return s == "+" || s == "-" || s == "*" || s == "/" || s == "^";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            string content = button.Content.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(content)) return;

            string current = ResultBox.Text;

            if (_isNewExpression)
            {
                if (char.IsDigit(content[0]) || content == ".")
                {
                    ResultBox.Text = content;
                    _isNewExpression = false;
                }
                else
                {
                    ResultBox.Text = current == "0" ? content : current + content;
                    _isNewExpression = false;
                }
            }
            else
            {
                if (content == ".")
                {
                    int lastOperatorIndex = -1;
                    for (int i = current.Length - 1; i >= 0; i--)
                    {
                        if (!char.IsDigit(current[i]) && current[i] != '.')
                        {
                            lastOperatorIndex = i;
                            break;
                        }
                    }

                    string lastNumber = lastOperatorIndex == -1
                        ? current
                        : current[(lastOperatorIndex + 1)..];

                    if (lastNumber.Contains('.'))
                        return;
                }

                if (IsOperator(content))
                {
                    if (current.Length > 0 && IsOperator(current[^1].ToString())) return;
                }

                if (IsOperator(content) && current.EndsWith('.'))
                    return;

                ResultBox.Text += content;
            }

            ExpressionBox.Text = ResultBox.Text;
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = ResultBox.Text;

                if (string.IsNullOrWhiteSpace(text) || text == "0")
                {
                    ResultBox.Text = "0";
                    ExpressionBox.Text = "";
                    return;
                }

                if (text.Count(c => c == '(') != text.Count(c => c == ')'))
                {
                    MessageBox.Show("Ошибка: незакрытые скобки", "Ошибка");
                    return;
                }

                string expression = text.Replace(',', '.');
                ExpressionBox.Text = expression + " =";

                double result = Calculate(expression);

                ResultBox.Text = result.ToString(CultureInfo.InvariantCulture);
                _isNewExpression = true;
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Ошибка: деление на ноль", "Ошибка");
                Reset();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
                Reset();
            }
        }

        private static double Calculate(string expr)
        {
            expr = expr.Replace(" ", "");

            for (int i = 0; i < expr.Length - 1; i++)
            {
                if (char.IsDigit(expr[i]) && expr[i + 1] == '(')
                {
                    expr = expr.Insert(i + 1, "*");
                    i++;
                }
            }

            for (int i = 0; i < expr.Length - 1; i++)
            {
                if (expr[i] == ')' && (char.IsDigit(expr[i + 1]) || expr[i + 1] == '.'))
                {
                    expr = expr.Insert(i + 1, "*");
                    i++;
                }
            }

            while (expr.Contains('('))
            {
                int lastOpen = expr.LastIndexOf('(');
                int firstClose = expr.IndexOf(')', lastOpen);

                string inside = expr.Substring(lastOpen + 1, firstClose - lastOpen - 1);
                double insideResult = Calculate(inside);

                expr = expr[..lastOpen] + insideResult.ToString(CultureInfo.InvariantCulture) + expr[(firstClose + 1)..];
            }

            return CalculateWithoutBrackets(expr);
        }

        private static double CalculateWithoutBrackets(string expr)
        {
            var parts = new List<string>();
            string currentNumber = "";

            foreach (char c in expr)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    currentNumber += c;
                }
                else
                {
                    if (currentNumber != "")
                    {
                        parts.Add(currentNumber);
                        currentNumber = "";
                    }
                    parts.Add(c.ToString());
                }
            }

            if (currentNumber != "")
            {
                parts.Add(currentNumber);
            }

            while (parts.Contains("^"))
            {
                int index = parts.IndexOf("^");

                double left = double.Parse(parts[index - 1], CultureInfo.InvariantCulture);
                double right = double.Parse(parts[index + 1], CultureInfo.InvariantCulture);

                double result = Calculator.Power(left, right);

                parts.RemoveRange(index - 1, 3);
                parts.Insert(index - 1, result.ToString(CultureInfo.InvariantCulture));
            }

            while (parts.Contains("*") || parts.Contains("/"))
            {
                int index = -1;
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i] == "*" || parts[i] == "/")
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1) break;

                double left = double.Parse(parts[index - 1], CultureInfo.InvariantCulture);
                double right = double.Parse(parts[index + 1], CultureInfo.InvariantCulture);

                double result = parts[index] == "*"
                    ? Calculator.Multiply(left, right)
                    : Calculator.Divide(left, right);

                parts.RemoveRange(index - 1, 3);
                parts.Insert(index - 1, result.ToString(CultureInfo.InvariantCulture));
            }

            while (parts.Count > 1)
            {
                double left = double.Parse(parts[0], CultureInfo.InvariantCulture);
                string op = parts[1];
                double right = double.Parse(parts[2], CultureInfo.InvariantCulture);

                double result = op == "+"
                    ? Calculator.Add(left, right)
                    : Calculator.Subtract(left, right);

                parts.RemoveRange(0, 3);
                parts.Insert(0, result.ToString(CultureInfo.InvariantCulture));
            }

            return double.Parse(parts[0], CultureInfo.InvariantCulture);
        }

        private void Reset()
        {
            ResultBox.Text = "0";
            ExpressionBox.Text = "";
            _isNewExpression = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
    }
}