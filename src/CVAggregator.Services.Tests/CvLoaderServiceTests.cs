using FluentAssertions;
using NUnit.Framework;

namespace CVAggregator.Services.Tests
{
    [TestFixture]
    public class CvLoaderServiceTests
    {
        private CvLoaderService _service;

        [SetUp]
        public void SetUp()
        {
            //только для екатеринбурга
            _service = new CvLoaderService("http://rabota.e1.ru/api/v1/resumes/");
        }

        [Test]
        public async void should_load_page_of_resume()
        {
            const int pageSize = 100;
            var page = await _service.LoadCurriculumVitae(0, pageSize,994);

            page.Should().NotBeNull();
            page.Should().HaveCount(pageSize);
        }
    }
}