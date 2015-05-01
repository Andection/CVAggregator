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

                return ((IEnumerable<dynamic>) data.resumes).Select<dynamic, CurriculumVitae>(rawResume => Map(rawResume)).ToArray();
            }
        }

        private static CurriculumVitae Map(dynamic rawResume)
        {
            return new CurriculumVitae()
            {
                Id = rawResume.id,
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
}