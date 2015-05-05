using System;

namespace CVAggregator.Host.ViewModel
{
    public interface IUiSynchronizationService
    {
        void Execute(Action action);
    }
}