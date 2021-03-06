using System;
using System.Windows;
using Sidekick.Presentation.Wpf.Views;

namespace Sidekick.Presentation.Wpf.Setup
{
    public partial class SetupView : BaseView
    {
        private readonly SetupViewModel viewModel;

        public SetupView(IServiceProvider serviceProvider, SetupViewModel viewModel)
            : base(Domain.Views.View.Setup, serviceProvider)
        {
            this.viewModel = viewModel;

            InitializeComponent();
            DataContext = viewModel;

            Topmost = true;
            Dispatcher.Invoke(async () =>
            {
                await System.Threading.Tasks.Task.Delay(1000);
                Topmost = false;
            });
        }

        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.HasErrors) return;
            await viewModel.Save();
        }

        private async void DiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.Close();
        }
    }
}
