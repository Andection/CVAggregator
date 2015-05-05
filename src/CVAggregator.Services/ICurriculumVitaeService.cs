using System.Threading.Tasks;
using CVAggregator.Domain;

namespace CVAggregator.Services
{
    public interface ICurriculumVitaeService
    {
        Task<Page<Resume>> Load(QueryCriteria criteria = null);
    }
}