using System;
using System.Configuration;
using System.Linq;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using NUnit.Framework;

namespace CVAggregator.Services.Tests
{
    public abstract class AbstractPersistenceTests
    {
        private MongoClient _client;
        protected MongoDatabase Database;
        private string _databaseName;

        [SetUp]
        public void SetUp()
        {
            _client = new MongoClient(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString);
            _databaseName = Guid.NewGuid().ToString();

            //new api do not compatible with linq a while
            var server = _client.GetServer();
            Database = server.GetDatabase(_databaseName);
            var convention = new ConventionPack();
            convention.AddClassMapConvention("IdConvention", c =>
            {
                if (c.IdMemberMap != null)
                {
                    c.IdMemberMap.SetIdGenerator(new StringObjectIdGenerator());
                }
            });
            ConventionRegistry.Register("convention", convention, t => t.Name.Any());
            BeforeEachTest();
        }

        protected MongoCollection<T> GetCollection<T>()
        {
           return Database.GetCollection<T>(typeof(T).FullName);
        }

        protected virtual void BeforeEachTest()
        {

        }

        [TearDown]
        public void TearDown()
        {
            _client.DropDatabaseAsync(_databaseName);
        }
    }
}