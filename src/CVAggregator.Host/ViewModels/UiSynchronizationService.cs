using System;
using System.Windows;

namespace CVAggregator.Host.ViewModel
{
    public class UiSynchronizationService : IUiSynchronizationService
    {
        public void Execute(Action action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                App.Current.Dispatcher.Invoke(action);
            }
        }
    }
}