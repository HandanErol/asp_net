using InsuranceQuoteCalculator.Models;

namespace InsuranceQuoteCalculator.Services
{
    public interface IQuoteCalculator
    {
        decimal CalculateQuote(Insuree insuree);
    }

    public class BaseQuoteCalculator : IQuoteCalculator
    {
        public decimal CalculateQuote(Insuree insuree)
        {
            return 50m; // Base quote
        }
    }

    public abstract class QuoteCalculatorDecorator : IQuoteCalculator
    {
        protected readonly IQuoteCalculator _calculator;

        public QuoteCalculatorDecorator(IQuoteCalculator calculator)
        {
            _calculator = calculator;
        }

        public virtual decimal CalculateQuote(Insuree insuree)
        {
            return _calculator.CalculateQuote(insuree);
        }
    }

    public class AgeQuoteDecorator : QuoteCalculatorDecorator
    {
        public AgeQuoteDecorator(IQuoteCalculator calculator) : base(calculator) { }

        public override decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = base.CalculateQuote(insuree);
            int age = (DateTime.Now - insuree.DateOfBirth).Days / 365;

            if (age <= 18) return baseQuote + 100m;
            if (age <= 25) return baseQuote + 50m;
            return baseQuote + 25m;
        }
    }

    public class CarQuoteDecorator : QuoteCalculatorDecorator
    {
        public CarQuoteDecorator(IQuoteCalculator calculator) : base(calculator) { }

        public override decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = base.CalculateQuote(insuree);

            if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
                baseQuote += 25m;

            if (insuree.CarMake.ToLower() == "porsche")
            {
                baseQuote += 25m;
                if (insuree.CarModel.ToLower() == "911 carrera")
                    baseQuote += 25m;
            }

            return baseQuote;
        }
    }

    public class DrivingHistoryQuoteDecorator : QuoteCalculatorDecorator
    {
        public DrivingHistoryQuoteDecorator(IQuoteCalculator calculator) : base(calculator) { }

        public override decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = base.CalculateQuote(insuree);
            baseQuote += insuree.SpeedingTickets * 10m;
            
            if (insuree.DUI)
                baseQuote *= 1.25m;

            return baseQuote;
        }
    }

    public class CoverageQuoteDecorator : QuoteCalculatorDecorator
    {
        public CoverageQuoteDecorator(IQuoteCalculator calculator) : base(calculator) { }

        public override decimal CalculateQuote(Insuree insuree)
        {
            decimal baseQuote = base.CalculateQuote(insuree);
            return insuree.FullCoverage ? baseQuote * 1.5m : baseQuote;
        }
    }
}