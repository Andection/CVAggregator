using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AggregatorService.Domain;
using Newtonsoft.Json;

namespace CVAggregator.Services
{
    public class CvLoaderService
    {
        private readonly string _rootUri;

        public CvLoaderService(string rootUri)
        {
            _rootUri = rootUri;
        }

        public async Task<CurriculumVitae[]> LoadCurriculumVitae(int pageIndex, int pageSize, int cityId)
        {
            using (var httpClient = new HttpClient())
            {
                var rawJson = await httpClient.GetStringAsync(string.Format("{0}?city_id={1}&limit={2}&offset={3}", _rootUri, cityId, pageSize, pageSize*pageIndex));

                var data = JsonConvert.DeserializeObject<dynamic>(rawJson);

                return ((IEnumerable<dynamic>) data.resumes).Select(rawResume => new CurriculumVitae()).ToArray();
            }
        }
    }
}