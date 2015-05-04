using System;
using System.Collections.Generic;
using System.Linq;
using AggregatorService.Domain;
using FluentAssertions;
using MongoDB.Driver.Linq;
using NUnit.Framework;

namespace CVAggregator.Services.Tests
{
    [TestFixture]
    public class CurriculumVitaeServiceTests : AbstractPersistenceTests
    {
        private CurriculumVitaeService _service;

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();
            _service = new CurriculumVitaeService(Database);
        }

        [Test]
        public void should_insert_resumes()
        {
            var expectedCv = new CurriculumVitae()
            {
                ExternalId = "123",
                ExperienceLength = "ExperienceLength",
                Birthday = DateTime.UtcNow.Date,
                Skills = "skillss",
                WorkingType = "working type",
                CvHeader = "programmer",
                Education = "education",
                FullDataUri = "some uri",
                Name = "name surname",
                PersonalQualities = "qualities",
                PhotoUri = "uri",
                WantedSalary = 100000,
                UpdateDate = DateTime.Now
            };

            _service.Insert(new[]
            {
                expectedCv,
            });

            var actualCv = GetCollection<CurriculumVitae>().AsQueryable().FirstOrDefault(v => v.ExternalId == expectedCv.ExternalId);
            actualCv.Should().NotBeNull();
            actualCv.ExperienceLength.Should().Be(expectedCv.ExperienceLength);
            actualCv.Birthday.Should().Be(expectedCv.Birthday);
            actualCv.Skills.Should().Be(expectedCv.Skills);
            actualCv.WorkingType.Should().Be(expectedCv.WorkingType);
            actualCv.CvHeader.Should().Be(expectedCv.CvHeader);
            actualCv.Education.Should().Be(expectedCv.Education);
            actualCv.FullDataUri.Should().Be(expectedCv.FullDataUri);
            actualCv.Name.Should().Be(expectedCv.Name);
            actualCv.PersonalQualities.Should().Be(expectedCv.PersonalQualities);
            actualCv.PhotoUri.Should().Be(expectedCv.PhotoUri);
            actualCv.WantedSalary.Should().Be(expectedCv.WantedSalary);
        }

        [Test]
        public void should_load_resumes()
        {
            var curriculumVitaes = ExistsResumes(5);

            var loadedResumes = _service.Load(new QueryCriteria("3"));

            loadedResumes.Data.Should().HaveCount(1);
        }

        private IEnumerable<CurriculumVitae> ExistsResumes(int count = 3)
        {
            var result = Enumerable.Range(0, count).Select(i => new CurriculumVitae()
            {
                Name = "name" + i,
                CvHeader = "header" + i
            }).ToArray();

            foreach (var curriculumVitae in result)
            {
                GetCollection<CurriculumVitae>().Save(curriculumVitae);
            }

            return result;
        }
    }
}