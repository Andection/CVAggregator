﻿using System.Collections.Generic;
using AggregatorService.Domain;
using MongoDB.Driver;

namespace CVAggregator.Services
{
    public class CurriculumVitaeService
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
    }
}