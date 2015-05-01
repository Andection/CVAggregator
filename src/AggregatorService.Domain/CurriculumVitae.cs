using System;

namespace AggregatorService.Domain
{
    //todo: rename to resume
    public class CurriculumVitae
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string FullDataUri { get; set; }
        public string Name { get; set; }
        public string PhotoUri { get; set; }
        public DateTime? Birthday { get; set; }
        public decimal? WantedSalary { get; set; }
        public string CvHeader { get; set; }
        public string PersonalQualities { get; set; }
        public string Skills { get; set; }
        public string WorkingType { get; set; }
        public string Education { get; set; }
        public string ExperienceLength { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}