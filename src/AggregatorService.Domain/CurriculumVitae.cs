using System;
using System.Collections.Generic;

namespace AggregatorService.Domain
{
    public class CurriculumVitae
    {
        private IEnumerable<PreviousPosition> _previousPositions;
        private IEnumerable<DetailBlock> _details;
        public string Id { get; set; }
        public string PhotoUri { get; set; }
        public string Position { get; set; }
        public string Fio { get; set; }
        public string Brief { get; set; }
        public decimal WantedSalary { get; set; }
        public string AllExperienceTime { get; set; }
        public string SphereExperienceTime { get; set; }

        public IEnumerable<PreviousPosition> PreviousPositions
        {
            get { return _previousPositions ?? new PreviousPosition[0]; }
            set { _previousPositions = value; }
        }

        public IEnumerable<DetailBlock> Details
        {
            get { return _details ?? new DetailBlock[0]; }
            set { _details = value; }
        }
    }

    public class PreviousPosition
    {
        public string Diration { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string Details { get; set; }
    }

    public class DetailBlock
    {
        private IEnumerable<string> _detailsValue;
        public string Header { get; set; }

        public IEnumerable<string> DetailsValue
        {
            get { return _detailsValue ?? new String[0]; }
            set { _detailsValue = value; }
        }
    }
}