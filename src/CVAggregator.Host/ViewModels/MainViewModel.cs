using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Input;
using CVAggregator.Domain;
using CVAggregator.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace CVAggregator.Host.ViewModel
{
    public class MainViewModel : ViewModelBase, IProgressIndication
    {
        private readonly IAggregationService _aggregationService;
        private readonly ICurriculumVitaeService _curriculumVitaeService;
        private readonly IUiSynchronizationService _uiSynchronizationService;
        private RelayCommand _aggregateCommand;
        private string _positionToken;
        private ICommand _findCommand;
        private int? _maxSalary;
        private string _desiredSkills;
        private string _currentProgressMessage;
        private bool _isBusy;
        private int _maxProgressValue;
        private int _currentProgressValue;
        private bool _isIndeterminable;
        private bool _onlyWithPhoto=true;
        private bool _onlyWithSalary;

        public MainViewModel(IAggregationService aggregationService, ICurriculumVitaeService curriculumVitaeService,IUiSynchronizationService uiSynchronizationService)
        {
            _aggregationService = aggregationService;
            _curriculumVitaeService = curriculumVitaeService;
            _uiSynchronizationService = uiSynchronizationService;

            Resumes = new ObservableCollection<Resume>();

            OnFindResumes();
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
                OnFindResumes();
            }
        }

        public bool OnlyWithSalary
        {
            get { return _onlyWithSalary; }
            set
            {
                _onlyWithSalary = value;
                RaisePropertyChanged(() => OnlyWithSalary);
                OnFindResumes();
            }
        }

        public ObservableCollection<Resume> Resumes { get; set; }

        public ICommand FindCommad
        {
            get { return _findCommand ?? (_findCommand = new RelayCommand(async () => await OnFindResumes().ConfigureAwait(false))); }
        }

        private Task OnFindResumes()
        {
            return BusyIndication(async () =>
            {
                //В общем случае лучше не грузить все на клиента, а добавить pagination или динамически заружать только видимые данные
                var newCvs = await _curriculumVitaeService.Load(new QueryCriteria(PositionToken, DesiredSkills, MaxSalary, OnlyWithPhoto, OnlyWithSalary, 0, 100000));
                Resumes.Clear();
                foreach (var cv in newCvs.Data)
                {
                    Resumes.Add(cv);
                }
            });
        }

        public ICommand AggregateCommand
        {
            get
            {
                return _aggregateCommand ?? (_aggregateCommand = new RelayCommand(async () =>
                {
                    await OnAggregate().ConfigureAwait(false);
                }));
            }
        }

        private Task OnAggregate()
        {
            return BusyIndication(async () =>
            {
                await _aggregationService.Aggregate(this);
                await OnFindResumes().ConfigureAwait(false);
            });
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

        protected override void RaisePropertyChanged(string propertyName = null)
        {
            _uiSynchronizationService.Execute(() => base.RaisePropertyChanged(propertyName));
        }
    }
}