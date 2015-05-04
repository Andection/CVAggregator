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
    public class ResumeServiceTests : AbstractPersistenceTests
    {
        private ResumeService _service;

        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();
            _service = new ResumeService(Database);
        }

        [Test]
        public void should_insert_resumes()
        {
            var expectedCv = new Resume()
            {
                ExternalId = "123",
                ExperienceLength = "ExperienceLength",
                Skills = "skillss",
                WorkingType = "working type",
                Header = "programmer",
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

            var actualCv = GetCollection<Resume>().AsQueryable().FirstOrDefault(v => v.ExternalId == expectedCv.ExternalId);
            actualCv.Should().NotBeNull();
            actualCv.ExperienceLength.Should().Be(expectedCv.ExperienceLength);
            actualCv.Skills.Should().Be(expectedCv.Skills);
            actualCv.WorkingType.Should().Be(expectedCv.WorkingType);
            actualCv.Header.Should().Be(expectedCv.Header);
            actualCv.Education.Should().Be(expectedCv.Education);
            actualCv.FullDataUri.Should().Be(expectedCv.FullDataUri);
            actualCv.Name.Should().Be(expectedCv.Name);
            actualCv.PersonalQualities.Should().Be(expectedCv.PersonalQualities);
            actualCv.PhotoUri.Should().Be(expectedCv.PhotoUri);
            actualCv.WantedSalary.Should().Be(expectedCv.WantedSalary);
        }

        [Test]
        public async void should_load_resumes()
        {
            ExistsResumes(5);

            var loadedResumes = await _service.Load(new QueryCriteria("3"));

            loadedResumes.Data.Should().HaveCount(1);
        }

        private IEnumerable<Resume> ExistsResumes(int count = 3)
        {
            var result = Enumerable.Range(0, count).Select(i => new Resume()
            {
                Name = "name" + i,
                Header = "header" + i
            }).ToArray();

            foreach (var curriculumVitae in result)
            {
                GetCollection<Resume>().Save(curriculumVitae);
            }

            return result;
        }
    }
}