using Jot;
using Jot.Storage;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using SOMCalculator.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SOMCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
     
        private Tracker _tracker;

        public App()
        {
            _tracker = new Tracker(new JsonFileStore(Environment.SpecialFolder.MyDocuments));
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindow(), typeof(IViewFor<MianWindowViewModel>));
            Locator.CurrentMutable.RegisterLazySingleton(() => new MianWindowViewModel(), typeof(MianWindowViewModel));
            Locator.CurrentMutable.RegisterConstant<Tracker>(_tracker);

        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            _tracker.PersistAll();
        }
        

        }
    }
