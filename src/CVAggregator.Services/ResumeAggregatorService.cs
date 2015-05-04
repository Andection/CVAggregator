using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;

namespace CVAggregator.Services
{
    public class ResumeAggregatorService : IAggregationService
    {
        private readonly ResumeService _persistenceService;
        private readonly ResumeRemoteService _loaderService;
        private const int PageSize = 100;
        private ILog _log = LogManager.GetLogger<ResumeAggregatorService>();
        //Екатеринбург
        private const int CityId = 994;
        private const string Message = "Идет загрузка данных...";

        public ResumeAggregatorService(ResumeService persistenceService, ResumeRemoteService loaderService)
        {
            _persistenceService = persistenceService;
            _loaderService = loaderService;
        }

        public Task Aggregate(IProgressIndication progressIndication)
        {
            return Task.Run(() =>
            {
                progressIndication.Indeterminate(Message);
                _persistenceService.Clear();

                var page = _loaderService.LoadCurriculumVitae(0, PageSize, CityId);
                var totalResumes = page.Total;
                var loadedResumes = page.Size;

                progressIndication.Progress(loadedResumes, totalResumes, Message);
                _persistenceService.Insert(page.Data);

                Enumerable.Range(1, totalResumes/PageSize).AsParallel().ForAll(currentPageIndex =>
                {
                    try
                    {
                        var currentPage = _loaderService.LoadCurriculumVitae(currentPageIndex, PageSize, CityId);
                        if (currentPage.Size == 0)
                        {
                            loadedResumes = Interlocked.Add(ref loadedResumes, PageSize);
                        }
                        else
                        {
                            _persistenceService.Insert(currentPage.Data);

                            loadedResumes = Interlocked.Add(ref loadedResumes, page.Size);
                        }

                        progressIndication.Progress(loadedResumes, totalResumes, Message);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(m => m("some exception occured during cv aggregation"), ex);
                    }
                });
            });
        }
    }
}