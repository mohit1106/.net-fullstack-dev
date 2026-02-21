using System;

public class Solution
{
    public string EvaluateExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return "Error:InvalidExpression";

        string[] parts = expression.Split(' ');
        if (parts.Length != 3)
            return "Error:InvalidExpression";

        if (!int.TryParse(parts[0], out int a) || !int.TryParse(parts[2], out int b))
            return "Error:InvalidNumber";

        string op = parts[1];

        switch (op)
        {
            case "+":
                return (a + b).ToString();
            case "-":
                return (a - b).ToString();
            case "*":
                return (a * b).ToString();
            case "/":
                if (b == 0)
                    return "Error:DivideByZero";
                return (a / b).ToString();
            default:
                return "Error:UnknownOperator";
        }
    }
}
