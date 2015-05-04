using System.Threading.Tasks;
using AggregatorService.Domain;

namespace CVAggregator.Services
{
    public interface ICurriculumVitaeService
    {
        Task<Page<CurriculumVitae>> Load(QueryCriteria criteria = null);
    }
}