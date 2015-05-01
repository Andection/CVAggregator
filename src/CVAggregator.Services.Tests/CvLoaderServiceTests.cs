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
            _service = new CvLoaderService("http://rabota.e1.ru/resume");
        }

        [Test]
        public async void should_load_page_of_resume()
        {
            const int pageSize = 100;
            var page = await _service.LoadCurriculumVitae(0, pageSize);

            page.Should().NotBeNull();
            page.Should().HaveCount(pageSize);
        }
    }
}