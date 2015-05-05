/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:CVAggregator.Host"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System.Configuration;
using System.Linq;
using CVAggregator.Host.ViewModel;
using CVAggregator.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace CVAggregator.Host.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //todo: refactoring
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString);

            //new api do not compatible with linq a while
            var server = client.GetServer();
            var database = server.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
            var convention = new ConventionPack();
            convention.AddClassMapConvention("IdConvention", c =>
            {
                if (c.IdMemberMap != null)
                {
                    c.IdMemberMap.SetIdGenerator(new StringObjectIdGenerator());
                }
            });
            ConventionRegistry.Register("convention", convention, t => t.Name.Any());
            SimpleIoc.Default.Register<ICurriculumVitaeService>(() => new ResumeService(database));
            SimpleIoc.Default.Register<IUiSynchronizationService>(() => new UiSynchronizationService());
            SimpleIoc.Default.Register(() => new ResumeService(database));
            SimpleIoc.Default.Register(() => new ResumeRemoteService(ConfigurationManager.AppSettings["ResumesApiUrl"]));

            SimpleIoc.Default.Register<IAggregationService>(
                () => new ResumeAggregatorService(ServiceLocator.Current.GetInstance<ResumeService>(), ServiceLocator.Current.GetInstance<ResumeRemoteService>()));

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}