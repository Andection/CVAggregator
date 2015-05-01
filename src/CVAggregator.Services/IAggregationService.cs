using System.Threading.Tasks;

namespace CVAggregator.Services
{
    public interface IAggregationService
    {
        Task Aggregate(IProgressIndication progressIndication);
    }
}