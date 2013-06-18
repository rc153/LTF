using Toolkit.Ids;

namespace Toolkit.MarketData
{
    public interface IQuoteFeed
    {
        IQuoteModel getOrCreateModel(Id id);
    }

    public interface ITradeFeed
    {
        ITradeModel getOrCreateModel(Id id);
    }
}
