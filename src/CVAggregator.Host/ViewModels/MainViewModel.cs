using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private string _positionToken;
        private ICommand _findCommand;
        private int? _maxSalary;
        private string _desiredSkills;
        private RelayCommand<string> _openResumDetailsCommand;
        private string _currentProgressMessage;
        private bool _isBusy;
        private int _maxProgressValue;
        private int _currentProgressValue;
        private bool _isIndeterminable;
        private bool _onlyWithPhoto;
        private bool _onlyWithSalary;

        public MainViewModel(IAggregationService aggregationService,ICurriculumVitaeService curriculumVitaeService)
        {
            _aggregationService = aggregationService;
            _curriculumVitaeService = curriculumVitaeService;

            CurriculumVitaes = new ObservableCollection<CurriculumVitae>();
            if (IsInDesignMode)
            {
                CurriculumVitaes.Add(new CurriculumVitae()
                {
                    Header = "Дворник",
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
                    Header = "Программист",
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
                    Header = "Бухгалтер",
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

        public int? MaxSalary
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


        public bool OnlyWithPhoto
        {
            get { return _onlyWithPhoto; }
            set
            {
                _onlyWithPhoto = value;
                RaisePropertyChanged(() => OnlyWithPhoto);
            }
        }

        public bool OnlyWithSalary
        {
            get { return _onlyWithSalary; }
            set
            {
                _onlyWithSalary = value;
                RaisePropertyChanged(() => OnlyWithSalary);
            }
        }

        public ObservableCollection<CurriculumVitae> CurriculumVitaes { get; set; }

        public ICommand FindCommad
        {
            get
            {
                return _findCommand ?? (_findCommand = new RelayCommand(OnFindResumes));
            }
        }

        private async void OnFindResumes()
        {
            await BusyIndication(async () =>
            {
                var newCvs = await _curriculumVitaeService.Load(new QueryCriteria(PositionToken, DesiredSkills, MaxSalary, OnlyWithPhoto, OnlyWithSalary, 0, 10000));
                CurriculumVitaes.Clear();
                foreach (var cv in newCvs.Data)
                {
                    CurriculumVitaes.Add(cv);
                }
            });
        }

        public ICommand AggregateCommand
        {
            get
            {
                return _aggregateCommand ?? (_aggregateCommand = new RelayCommand(async () =>
                {
                    await OnAggregate();
                }));
            }
        }

        private async Task OnAggregate()
        {
            await BusyIndication(async () =>
            {
                await _aggregationService.Aggregate(this);
            });
        }

        public ICommand OpenResumDetailsCommand
        {
            get { return _openResumDetailsCommand ?? (_openResumDetailsCommand = new RelayCommand<string>(url => OnOpenDetails(url))); }
        }

        private static Process OnOpenDetails(string url)
        {
            return Process.Start(url);
        }

        public string CurrentProgressMessage
        {
            get { return _currentProgressMessage; }
            set
            {
                if (value == _currentProgressMessage)
                    return;

                _currentProgressMessage = value;
                RaisePropertyChanged(() => CurrentProgressMessage);
            }
        }

        private async Task BusyIndication(Func<Task> action)
        {
            IsBusy = true;
            try
            {
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public int MaxProgressValue
        {
            get { return _maxProgressValue; }
            set
            {
                if (value == _maxProgressValue)
                    return;

                _maxProgressValue = value;
                RaisePropertyChanged(() => MaxProgressValue);
            }
        }

        public int CurrentProgressValue
        {
            get { return _currentProgressValue; }
            set
            {
                if (value == _currentProgressValue)
                    return;

                _currentProgressValue = value;
                RaisePropertyChanged(() => CurrentProgressValue);
            }
        }

        public bool IsIndeterminable
        {
            get { return _isIndeterminable; }
            set
            {
                if (value == _isIndeterminable) 
                    return;

                _isIndeterminable = value;
                RaisePropertyChanged(() => IsIndeterminable);
            }
        }

        public void Indeterminate(string message)
        {
            IsIndeterminable = true;
            CurrentProgressMessage = message;
        }

        public void Progress(int current, int max, string message)
        {
            IsIndeterminable = false;
            CurrentProgressMessage = message;

            CurrentProgressValue = current;
            MaxProgressValue = max;
        }
    }
}