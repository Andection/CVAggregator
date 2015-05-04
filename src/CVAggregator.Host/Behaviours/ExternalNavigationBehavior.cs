using System;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace CVAggregator.Host.Behaviours
{
    internal class ExternalNavigationBehavior : Behavior<Hyperlink>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.RequestNavigate += AssociatedObject_RequestNavigate;
        }

        private void AssociatedObject_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                // todo: log it
            }

            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.RequestNavigate -= AssociatedObject_RequestNavigate;
            }
            base.OnDetaching();
        }
    }
}