using System.Threading.Tasks;

namespace CVAggregator.Services
{
    public class AggregatorService : IAggregationService
    {
        private readonly CvPersistenceService _persistenceService;
        private readonly CvLoaderService _loaderService;
        private const int PageSize = 100;
        //Екатеринбурга
        private const int CityId = 994;
        private const string Message = "Идет загрузка данных...";

        public AggregatorService(CvPersistenceService persistenceService, CvLoaderService loaderService)
        {
            _persistenceService = persistenceService;
            _loaderService = loaderService;
        }

        public async Task Aggregate(IProgressIndication progressIndication)
        {
            progressIndication.Indeterminate(Message);
            _persistenceService.Clear();
            var pageIndex = 0;

            var page = await _loaderService.LoadCurriculumVitae(pageIndex, PageSize, CityId);
            progressIndication.Progress(page.Size, page.Total, Message);
            _persistenceService.Insert(page.Data);

            while (page.Size == PageSize)
            {
                pageIndex++;
                page = await _loaderService.LoadCurriculumVitae(pageIndex, PageSize, CityId);
                _persistenceService.Insert(page.Data);

                progressIndication.Progress(PageSize*pageIndex + page.Size, page.Total, Message);
            }
        }
    }
}