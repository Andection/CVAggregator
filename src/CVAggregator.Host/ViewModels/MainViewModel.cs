using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AggregatorService.Domain;
using CVAggregator.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CVAggregator.Host.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IProgressIndication
    {
        private readonly IAggregationService _aggregationService;
        private readonly ICurriculumVitaeService _curriculumVitaeService;
        private RelayCommand _aggregateCommand;
        private string _nameToken;
        private string _positionToken;
        private ICommand _findCommand;
        private decimal? _maxSalary;
        private string _desiredSkills;

        public MainViewModel(IAggregationService aggregationService,ICurriculumVitaeService curriculumVitaeService)
        {
            _aggregationService = aggregationService;
            _curriculumVitaeService = curriculumVitaeService;

            CurriculumVitaes = new ObservableCollection<CurriculumVitae>();
            if (IsInDesignMode)
            {
                CurriculumVitaes.Add(new CurriculumVitae()
                {
                    Birthday = DateTime.Now.AddYears(-25),
                    CvHeader = "Дворник",
                    ExperienceLength = "10 лет",
                    WantedSalary = 25000,
                    Name = "Иванов Иван Иванович",
                    Skills = "Метла, лопата, ведро",
                    WorkingType = "Полный день",
                    PersonalQualities = "Трудолюбие, находнчивость, трезвость",
                    Education = "Два и более высших образования",
                    FullDataUri = "some uri",
                    PhotoUri = "Some photo"
                });
                CurriculumVitaes.Add(new CurriculumVitae()
                {
                    CvHeader = "Программист",
                    ExperienceLength = "10 лет",
                    WantedSalary = 100000,
                    Name = "Петров Петр Петрович",
                    Skills = "DDD, TDD, BDD, C#, Unit testing",
                    WorkingType = "Полный день",
                    PersonalQualities = "Обучаемость, трудолюбие",
                    Education = "Начальная школа",
                    FullDataUri = "some uri",
                    PhotoUri = "Some photo"
                });
                CurriculumVitaes.Add(new CurriculumVitae()
                {
                    CvHeader = "Бухгалтер",
                    ExperienceLength = "10 лет",
                    Name = "Петров Петр Петрович",
                    Skills = "Квартальная отчетность",
                    WorkingType = "Полный день",
                    PersonalQualities = "Обучаемость, трудолюбие",
                    Education = "Средне-специальное образование",
                    FullDataUri = "some uri",
                    PhotoUri = "Some photo"
                });
            }

        }

        public string PositionToken
        {
            get { return _positionToken; }
            set
            {
                _positionToken = value;
                RaisePropertyChanged(() => PositionToken);
            }
        }

        public decimal? MaxSalary
        {
            get { return _maxSalary; }
            set
            {
                _maxSalary = value;
                RaisePropertyChanged(() => MaxSalary);
            }
        }

        public string DesiredSkills
        {
            get { return _desiredSkills; }
            set
            {
                _desiredSkills = value;
                RaisePropertyChanged(() => DesiredSkills);
            }
        }

        public ObservableCollection<CurriculumVitae> CurriculumVitaes { get; set; }

        public ICommand FindCommad
        {
            get
            {
                return _findCommand ?? (_findCommand = new RelayCommand(() =>
                {
                      var newCvs = _curriculumVitaeService.Load(new QueryCriteria(PositionToken, "", 0, 10000));
                      CurriculumVitaes.Clear();
                      foreach (var cv in newCvs.Data)
                      {
                          CurriculumVitaes.Add(cv);
                      }
                }));
            }
        }

        public ICommand AggregateCommand
        {
            get { return _aggregateCommand ?? (_aggregateCommand = new RelayCommand(() => _aggregationService.Aggregate(this))); }
        }

        public void Indeterminate(string message)
        {
            throw new System.NotImplementedException();
        }

        public void Progress(int current, int max, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}