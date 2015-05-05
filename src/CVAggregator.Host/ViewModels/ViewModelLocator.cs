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

    public class ViewModelLocator
    {

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MongoDatabase>(GetDatabase);

            SimpleIoc.Default.Register<ICurriculumVitaeService>(() => new ResumeService(ServiceLocator.Current.GetInstance<MongoDatabase>()));
            SimpleIoc.Default.Register<IUiSynchronizationService>(() => new UiSynchronizationService());
            SimpleIoc.Default.Register(() => new ResumeService(ServiceLocator.Current.GetInstance<MongoDatabase>()));
            SimpleIoc.Default.Register(() => new ResumeRemoteService(ConfigurationManager.AppSettings["ResumesApiUrl"]));

            SimpleIoc.Default.Register<IAggregationService>(
                () => new ResumeAggregatorService(ServiceLocator.Current.GetInstance<ResumeService>(), ServiceLocator.Current.GetInstance<ResumeRemoteService>()));

            SimpleIoc.Default.Register<MainViewModel>();
        }

        private MongoDatabase GetDatabase()
        {
            var client = new MongoClient(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString);
            var convention = new ConventionPack();
            convention.AddClassMapConvention("IdConvention", c =>
            {
                if (c.IdMemberMap != null)
                {
                    c.IdMemberMap.SetIdGenerator(new StringObjectIdGenerator());
                }
            });
            ConventionRegistry.Register("convention", convention, t => t.Name.Any());

            //new api do not compatible with linq a while
            var server = client.GetServer();
            return server.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
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