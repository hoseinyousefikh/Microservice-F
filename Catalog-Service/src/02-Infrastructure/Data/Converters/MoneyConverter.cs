using Catalog_Service.src._01_Domain.Core.Primitives;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog_Service.src._02_Infrastructure.Data.Converters
{
    public class MoneyConverter : ValueConverter<Money, decimal>
    {
        public MoneyConverter()
            : base(
                convertToProviderExpression: money => money.Amount,
                convertFromProviderExpression: amount => Money.Create(amount, "IRR")) // استفاده از Create به جای Zero
        {
        }
    }
}
