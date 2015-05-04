using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AggregatorService.Domain;
using Common.Logging;
using Newtonsoft.Json;

namespace CVAggregator.Services
{
    public class CurriculumVitaeRemoteService
    {
        private readonly string _rootUri;

        private static readonly ILog Log = LogManager.GetLogger<CurriculumVitaeRemoteService>();
        private const string DirectoryPath = "/api/v1/resumes/";

        public CurriculumVitaeRemoteService(string rootUri)
        {
            _rootUri = rootUri;
        }

        public Page<CurriculumVitae> LoadCurriculumVitae(int pageIndex, int pageSize, int cityId)
        {
            using (var httpClient = new HttpClient())
            {
                var rawJson = httpClient.GetStringAsync(string.Format("{0}/{1}?city_id={2}&limit={3}&offset={4}", _rootUri, DirectoryPath, cityId, pageSize, pageSize*pageIndex)).Result;
                Log.Trace(m => m("loaded json {0}", rawJson));
           
                var data = JsonConvert.DeserializeObject<dynamic>(rawJson);

                var total = Convert.ToInt32(data.metadata.resultset.count);
                var result = ((IEnumerable<dynamic>) data.resumes).Select<dynamic, CurriculumVitae>(rawResume => Map(rawResume)).ToArray();

                return new Page<CurriculumVitae>(pageIndex, result, total);
            }
        }

        private  CurriculumVitae Map(dynamic rawResume)
        {
            return new CurriculumVitae
            {
                ExternalId = rawResume.id,
                Header = rawResume.header,
                Education = rawResume.education != null ? rawResume.education.title : string.Empty,
                Skills = rawResume.skills,
                FullDataUri = ConstructUri((string)rawResume.url),
                Name = rawResume.contact != null ? rawResume.contact.name : string.Empty,
                PersonalQualities = rawResume.personal_qualities,
                PhotoUri = rawResume.photo != null && rawResume.photo.url != null ? ConstructUri((string)rawResume.photo.url) : string.Empty,
                WorkingType = rawResume.working_type != null ? rawResume.working_type.title : string.Empty,
                WantedSalary = ParseSalary(rawResume.wanted_salary_rub),
                ExperienceLength = rawResume.experience_length != null ? rawResume.experience_length.title : string.Empty,
                UpdateDate = (DateTime?)rawResume.mod_date
            };
        }

        private int? ParseSalary(dynamic salary)
        {
            if (salary == null)
                return null;

            var doubleSalary = Convert.ToDouble(salary);

            return (int) doubleSalary;
        }

        private string ConstructUri(string photoUrl)
        {
            return photoUrl.ToLower().StartsWith("http") ? photoUrl : string.Format("{0}{1}", _rootUri, photoUrl);
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