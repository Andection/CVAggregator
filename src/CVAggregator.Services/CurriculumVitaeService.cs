using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AggregatorService.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CVAggregator.Services
{
    public class CurriculumVitaeService : ICurriculumVitaeService
    {
        private readonly MongoDatabase _database;

        public CurriculumVitaeService(MongoDatabase database)
        {
            _database = database;
        }

        public void Insert(IEnumerable<CurriculumVitae> resumes)
        {
            _database.GetCollection<CurriculumVitae>(typeof (CurriculumVitae).FullName).InsertBatch(resumes);
        }

        public void Clear()
        {
            _database.DropCollection(typeof (CurriculumVitae).FullName);
        }

        public Task<Page<CurriculumVitae>> Load(QueryCriteria criteria = null)
        {
            return Task.Run(() =>
            {
                var currentCriteria = criteria ?? new QueryCriteria();

                var query = _database.GetCollection<CurriculumVitae>(typeof (CurriculumVitae).FullName).AsQueryable();
                if (!string.IsNullOrWhiteSpace(currentCriteria.CvHeader))
                {
                    query = query.Where(c => c.Header.ToLower().Contains(currentCriteria.CvHeader.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(currentCriteria.Skill))
                {
                    var skills = currentCriteria.Skill.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var skill in skills)
                    {
                        query = query.Where(c => c.Skills.ToLower().Contains(skill.ToLower()));
                    }
                }
                if (currentCriteria.MaxSalary.HasValue)
                {
                    if (currentCriteria.OnlyWithSalary)
                    {
                        query = query.Where(c => c.WantedSalary != null && c.WantedSalary <= currentCriteria.MaxSalary.Value);
                    }
                    else
                    {
                        query = query.Where(c => c.WantedSalary == null || c.WantedSalary <= currentCriteria.MaxSalary.Value);
                    }
                }
                else if (currentCriteria.OnlyWithSalary)
                {
                    query = query.Where(c => c.WantedSalary != null);
                }

                if (currentCriteria.OnlyWithPhoto)
                {
                    query = query.Where(c => !string.IsNullOrEmpty(c.PhotoUri));
                }

                var result = query.OrderByDescending(c => c.UpdateDate).Take(currentCriteria.PageSize).Skip(currentCriteria.PageIndex*currentCriteria.PageSize).ToArray();

                return new Page<CurriculumVitae>(currentCriteria.PageIndex, result, 0);
            });
        }
    }

    public class QueryCriteria
    {
        public QueryCriteria(string cvHeader = null, string skill = null, int? maxSalary = null, bool onlyWithPhoto = false, bool onlyWithSalary = false, int pageIndex = 0,
            int pageSize = 20)
        {
            MaxSalary = maxSalary;
            OnlyWithPhoto = onlyWithPhoto;
            OnlyWithSalary = onlyWithSalary;
            PageIndex = pageIndex;
            PageSize = pageSize;
            CvHeader = cvHeader ?? string.Empty;
            Skill = skill ?? string.Empty;
        }

        public int? MaxSalary { get; private set; }
        public bool OnlyWithPhoto { get; private set; }
        public bool OnlyWithSalary { get;private set; }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public string CvHeader { get; private set; }

        public string Skill { get; private set; }
    }
}