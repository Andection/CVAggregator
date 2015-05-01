using System;
using AggregatorService.Domain;
using FluentAssertions;
using NUnit.Framework;

namespace CVAggregator.Services.Tests
{
    [TestFixture]
    public class AgregatorServiceTests:AbstractPersistenceTests
    {
        private AggregatorService _service;

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();
            _service = new AggregatorService(new CurriculumVitaeService(Database), new CurriculumVitaeRemoteService("http://rabota.e1.ru/api/v1/resumes/"));
        }

        [Test]
        public async void should_aggregate_all_resumes()
        {
            await _service.Aggregate(new ProgressIndicationImp());

            var collection = GetCollection<CurriculumVitae>();
            collection.Count().Should().BeGreaterThan(0);
        }

        private class ProgressIndicationImp : IProgressIndication
        {
            public void Indeterminate(string message)
            {
                Console.WriteLine("indeterminate: {0}",  message);
            }

            public void Progress(int current, int max, string message)
            {
                Console.WriteLine("{0}/{1}: {2}", current, max, message);
            }
        }
    }
}
