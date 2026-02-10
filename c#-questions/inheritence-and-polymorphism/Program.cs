using System;
using System.Globalization;

public abstract class Employee
{
    public abstract decimal Pay { get; }
}

public sealed class HourlyEmployee : Employee
{
    private readonly decimal rate;
    private readonly decimal hours;

    public HourlyEmployee(decimal rate, decimal hours)
    {
        this.rate = rate;
        this.hours = hours;
    }

    public override decimal Pay => rate * hours;
}

public sealed class SalariedEmployee : Employee
{
    private readonly decimal salary;

    public SalariedEmployee(decimal salary)
    {
        this.salary = salary;
    }

    public override decimal Pay => salary;
}

public sealed class CommissionEmployee : Employee
{
    private readonly decimal commission;
    private readonly decimal baseSalary;

    public CommissionEmployee(decimal commission, decimal baseSalary)
    {
        this.commission = commission;
        this.baseSalary = baseSalary;
    }

    public override decimal Pay => baseSalary + commission;
}

public class Solution
{
    public static decimal TotalPay(string[] employees)
    {
        if (employees == null || employees.Length == 0) return 0m;

        decimal total = 0m;

        for (int i = 0; i < employees.Length; i++)
        {
            var s = employees[i];
            if (string.IsNullOrWhiteSpace(s)) continue;

            var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) continue;

            Employee e;

            switch (parts[0][0])
            {
                case 'H':
                    e = new HourlyEmployee(
                        decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                        decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    );
                    break;

                case 'S':
                    e = new SalariedEmployee(
                        decimal.Parse(parts[1], CultureInfo.InvariantCulture)
                    );
                    break;

                case 'C':
                    e = new CommissionEmployee(
                        decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                        decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                    );
                    break;

                default:
                    continue;
            }

            total += e.Pay;
        }

        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
