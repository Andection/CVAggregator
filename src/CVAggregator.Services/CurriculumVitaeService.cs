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

        //todo: add tokens. splited words
        public Task<Page<CurriculumVitae>> Load(QueryCriteria criteria = null)
        {
            return Task.Run(() =>
            {
                var currentCriteria = criteria ?? new QueryCriteria();

                var query = _database.GetCollection<CurriculumVitae>(typeof (CurriculumVitae).FullName).AsQueryable();
                if (!string.IsNullOrWhiteSpace(currentCriteria.CvHeader))
                {
                    query = query.Where(c => c.Header.Contains(currentCriteria.CvHeader));
                }
                if (!string.IsNullOrWhiteSpace(currentCriteria.Name))
                {
                    query = query.Where(c => c.Name.Contains(currentCriteria.Name));
                }

                var result = query.OrderByDescending(c => c.UpdateDate).Take(currentCriteria.PageSize).Skip(currentCriteria.PageIndex*currentCriteria.PageSize).ToArray();

                return new Page<CurriculumVitae>(currentCriteria.PageIndex, result, 0);
            });
        }
    }

    public class QueryCriteria
    {
        public QueryCriteria(string cvHeader = null, string name = null, int? pageIndex = null, int? pageSize = null)
        {
            PageIndex = pageIndex ?? 0;
            PageSize = pageSize ?? 20;
            CvHeader = cvHeader ?? string.Empty;
            Name = name ?? string.Empty;
        }

        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public string CvHeader { get; private set; }

        public string Name { get; private set; }
    }
}