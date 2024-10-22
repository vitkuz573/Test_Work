using System.Data;
using System.Text.RegularExpressions;
using Test_Work.Abstractions;

namespace Test_Work.Services;

public class CalculatorService : ICalculatorService
{
    public string Calculate(string expression)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return "Ошибка: выражение не может быть пустым.";
            }

            if (!IsValidExpression(expression))
            {
                return "Ошибка: выражение содержит недопустимые символы.";
            }

            if (ContainsDivisionByZero(expression))
            {
                return "Ошибка: деление на ноль.";
            }

            var result = new DataTable().Compute(expression, null);

            return result.ToString();
        }
        catch (SyntaxErrorException)
        {
            return "Ошибка: неверное выражение.";
        }
        catch (Exception ex)
        {
            return $"Ошибка: {ex.Message}";
        }
    }

    private static bool IsValidExpression(string expression)
    {
        return expression.All(ch => char.IsDigit(ch) || "+-*/() ".Contains(ch));
    }

    private static bool ContainsDivisionByZero(string expression)
    {
        return Regex.IsMatch(expression, @"\/\s*0(\D|$)");
    }
}
