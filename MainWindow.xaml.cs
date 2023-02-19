using ReactiveUI;
using SOMCalculator.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace SOMCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow,  IViewFor<MianWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
       .Register(nameof(ViewModel), typeof(MianWindowViewModel), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                Wpf.Ui.Appearance.Watcher.Watch(
                    this,                                  // Window class
                    Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
                    true                                   // Whether to change accents automatically
                );
            };

            var viewmodel = Locator.Current.GetService<MianWindowViewModel>();
            if (viewmodel != null)
            {
                ViewModel = viewmodel;
            }
            else
            {
                ViewModel = new MianWindowViewModel();
            }
            DataContext = ViewModel;
            this.WhenActivated(disposable =>
            {
                this.Bind(this.ViewModel, x => x.Text, x => x.textBox.Text).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, x=>x.History, x=>x.textBox.ItemsSource).DisposeWith(disposable);
                this.Bind(this.ViewModel, x=>x.SelectedValue, x=>x.textBox.SelectedValue).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.Result, x => x.resulBox.Text).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.ResultHex, x => x.hexBox.Text).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.ResultBin, x => x.binBox.Text).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.ResultOct, x => x.octBox.Text).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.HexFloat, x => x.hexFloat.Text).DisposeWith(disposable);
                this.Bind(this.ViewModel, x => x.HexInt, x => x.hexInt.Text).DisposeWith(disposable);
                this.OneWayBind(this.ViewModel, x => x.DataBits, x => x.virt.ItemsSource).DisposeWith(disposable);
                textBox.Events().KeyDown.Select(x=>x.Key).Where(x=>x == Key.Enter).InvokeCommand(this, x=>x.ViewModel.KeyEnterCommand).DisposeWith(disposable);
            });
        }

        public MianWindowViewModel ViewModel
        {
            get => (MianWindowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MianWindowViewModel)value;
        }
    }
}
