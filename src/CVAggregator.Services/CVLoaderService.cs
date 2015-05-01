using System;
using System.Threading.Tasks;
using AggregatorService.Domain;

namespace CVAggregator.Services
{
    public class CvLoaderService
    {
        private readonly string _rootUri;

        public CvLoaderService(string rootUri)
        {
            _rootUri = rootUri;
        }

        public Task<CurriculumVitae[]> LoadCurriculumVitae(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}