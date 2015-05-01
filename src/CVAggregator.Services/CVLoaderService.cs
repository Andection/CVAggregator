using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AggregatorService.Domain;
using HtmlAgilityPack;

namespace CVAggregator.Services
{
    public class CvLoaderService
    {
        private readonly string _rootUri;

        public CvLoaderService(string rootUri)
        {
            _rootUri = rootUri;
        }

        public async Task<CurriculumVitae[]> LoadCurriculumVitae(int pageIndex, int pageSize)
        {
            using (var httpClient = new HttpClient())
            {
                var rawHtml = await httpClient.GetStringAsync(string.Format("{0}?limit={1}&offset={2}", _rootUri, pageSize, pageSize*pageIndex));

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(rawHtml);

                var allContent = htmlDocument.GetElementbyId("ra-content");
                return allContent.SelectSingleNode("//div[@class='ra-elements-container']")
                                 .SelectSingleNode("//ul[@class='ra-elements-list-hidden']")
                                 .Descendants("li")
                                 .Select(Map)
                                 .ToArray();

            }
        }

        private static CurriculumVitae Map(HtmlNode item)
        {
            return new CurriculumVitae();
        }
    }
}