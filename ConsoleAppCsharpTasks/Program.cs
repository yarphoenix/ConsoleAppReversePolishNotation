using System.Globalization;

class Program
{
    public static void Main()
    {
        string? expression = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(expression))
        {
            Console.WriteLine("Ошибка: выражение не должно быть пустым.");
            return;
        }

        var reversePolishNotation = ProcessReversePolishNotation(expression.Replace(" ", ""));
        PrintReversePolishNotation(reversePolishNotation);

        try
        {
            double result = CalculateReversePolishNotation(reversePolishNotation);
            Console.WriteLine(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка вычисления: {ex.Message}");
        }
    }

    public static void PrintReversePolishNotation(Queue<string> reversePolishNotation)
    {
        Console.WriteLine(string.Join(" ", reversePolishNotation));
    }

    public static double CalculateReversePolishNotation(Queue<string> reversePolishNotation)
    {
        var operands = new Stack<double>();
        foreach (var token in reversePolishNotation)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double number))
            {
                operands.Push(number);
            }
            else if (IsOperator(token[0]))
            {
                double secondOperand = operands.Pop();
                double firstOperand = operands.Pop();
                operands.Push(token[0] switch
                {
                    '+' => firstOperand + secondOperand,
                    '-' => firstOperand - secondOperand,
                    '*' => firstOperand * secondOperand,
                    '/' => firstOperand / secondOperand,
                    '^' => Math.Pow(firstOperand, secondOperand),
                    _ => throw new InvalidOperationException("Неизвестный оператор")
                });
            }
        }
        return operands.Pop();
    }

    public static Queue<string> ProcessReversePolishNotation(string expression)
    {
        var output = new Queue<string>();
        var priority = new Dictionary<char, int>
        {
            ['+'] = 1, ['-'] = 1, 
            ['*'] = 2, ['/'] = 2, 
            ['^'] = 3
        };
        var operators = new Stack<char>();

        for (int i = 0; i < expression.Length; i++)
        {
            char current = expression[i];

            if (char.IsWhiteSpace(current))
                continue;

            // Унарный минус
            if (current == '-' && (i == 0 || expression[i - 1] == '(' || IsOperator(expression[i - 1])))
            {
                string number = "-";
                i++;
                bool hasDot = false;
                while (i < expression.Length && (char.IsDigit(expression[i]) || (expression[i] == '.' && !hasDot)))
                {
                    if (expression[i] == '.')
                        hasDot = true;
                    number += expression[i++];
                }
                i--;
                output.Enqueue(number);
            }
            else if (char.IsDigit(current) || current == '.')
            {
                string number = "";
                bool hasDot = false;
                while (i < expression.Length && (char.IsDigit(expression[i]) || (expression[i] == '.' && !hasDot)))
                {
                    if (expression[i] == '.')
                        hasDot = true;
                    number += expression[i++];
                }
                i--;
                output.Enqueue(number);
            }
            else if (current == '(')
            {
                operators.Push(current);
            }
            else if (current == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                    output.Enqueue(operators.Pop().ToString());
                if (operators.Count > 0 && operators.Peek() == '(')
                    operators.Pop();
            }
            else if (IsOperator(current))
            {
                while (operators.Count > 0 &&
                       operators.Peek() != '(' &&
                       priority[current] <= priority[operators.Peek()])
                {
                    output.Enqueue(operators.Pop().ToString());
                }
                operators.Push(current);
            }
        }

        while (operators.Count > 0)
        {
            char op = operators.Pop();
            if (op != '(' && op != ')')
                output.Enqueue(op.ToString());
        }

        return output;
    }

    private static bool IsOperator(char c) => c is '+' or '-' or '*' or '/' or '^';
}