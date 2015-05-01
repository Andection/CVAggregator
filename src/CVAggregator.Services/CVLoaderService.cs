using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AggregatorService.Domain;
using Common.Logging;
using Newtonsoft.Json;

namespace CVAggregator.Services
{
    public class CvLoaderService
    {
        private readonly string _rootUri;

        private static ILog _log = LogManager.GetLogger<CvLoaderService>();

        public CvLoaderService(string rootUri)
        {
            _rootUri = rootUri;
        }

        public async Task<Page<CurriculumVitae>> LoadCurriculumVitae(int pageIndex, int pageSize, int cityId)
        {
            using (var httpClient = new HttpClient())
            {
                var rawJson = await httpClient.GetStringAsync(string.Format("{0}?city_id={1}&limit={2}&offset={3}", _rootUri, cityId, pageSize, pageSize*pageIndex));
                _log.Trace(m => m("loaded json {0}", rawJson));
           
                var data = JsonConvert.DeserializeObject<dynamic>(rawJson);

                var total = Convert.ToInt32(data.metadata.resultset.count);
                var result = ((IEnumerable<dynamic>) data.resumes).Select<dynamic, CurriculumVitae>(rawResume => Map(rawResume)).ToArray();

                return new Page<CurriculumVitae>(pageIndex, result, total);
            }
        }

        private static CurriculumVitae Map(dynamic rawResume)
        {
            return new CurriculumVitae
            {
                ExternalId = rawResume.id,
                Birthday = rawResume.birthday,
                CvHeader = rawResume.header,
                Education = rawResume.education != null ? rawResume.education.title : string.Empty,
                Skills = rawResume.skills,
                FullDataUri = rawResume.url,
                Name = rawResume.contact != null ? rawResume.contact.name : string.Empty,
                PersonalQualities = rawResume.personal_qualities,
                PhotoUri = rawResume.photo != null ? rawResume.photo.url : string.Empty,
                WorkingType = rawResume.working_type != null ? rawResume.working_type.title : string.Empty,
                WantedSalary = rawResume.wanted_salary_rub,
                ExperienceLength = rawResume.experience_length != null ? rawResume.experience_length.title : string.Empty
            };
        }
    }

    public class Page<T>
    {
        public Page(int index, T[] data, int total)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            
            Index = index;
            Data = data;
            Total = total;
        }

        public int Index { get; private set; }

        public int Size
        {
            get { return Data.Length; }
        }

        public int Total { get; private set; }
        public T[] Data { get; private set; }
    }
}