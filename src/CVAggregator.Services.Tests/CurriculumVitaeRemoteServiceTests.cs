using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace CVAggregator.Services.Tests
{
    [TestFixture]
    public class CurriculumVitaeRemoteServiceTests
    {
        private CurriculumVitaeRemoteService _service;
        private const int PageSize=100;
        //Екатеринбурга
        private const int CityId = 994;

        [SetUp]
        public void SetUp()
        {
            _service = new CurriculumVitaeRemoteService("http://rabota.e1.ru/api/v1/resumes/");
        }

        [Test]
        public  void should_load_page_of_resume()
        {
            var page =  _service.LoadCurriculumVitae(0, PageSize, CityId);

            page.Should().NotBeNull();
            page.Data.Should().HaveCount(PageSize);
        }

        [Test]
        public async void should_parse_resume()
        {
            var page = _service.LoadCurriculumVitae(0, PageSize, CityId).Data;

            page.Any(p => string.IsNullOrEmpty(p.ExternalId)).Should().BeFalse();
            page.Any(p => !string.IsNullOrWhiteSpace(p.Name)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.CvHeader)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.Education)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.ExperienceLength)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.FullDataUri)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.Name)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.PersonalQualities)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.PhotoUri)).Should().BeTrue();

            page.Any(p => !string.IsNullOrWhiteSpace(p.Skills)).Should().BeTrue();
            page.Any(p => !string.IsNullOrWhiteSpace(p.WorkingType)).Should().BeTrue();
            page.Any(p => p.Birthday.HasValue).Should().BeTrue();
            page.Any(p => p.WantedSalary.HasValue).Should().BeTrue();
        }
    }
}