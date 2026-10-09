using Microsoft.Management.Deployment;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Core;

namespace AppInstallerCaller
{
    public class InstallingPackageView(CatalogPackage package, CoreDispatcher dispatcher) : INotifyPropertyChanged
    {
        public CatalogPackage Package => package;

        public IAsyncOperationWithProgress<InstallResult, InstallProgress> AsyncOperation
        {
            get;
            set => (field = value).Progress += (_, progress) => UpdateUIProgress(progress, this);
        }

        private static void UpdateUIProgress(InstallProgress progress, InstallingPackageView view) => view.Progress = progress.DownloadProgress * 100;

        public double Progress { get; private set => SetProperty(ref field, value); } = 0;

        public string StatusText { get; private set; }
        public CoreDispatcher Dispatcher => dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        private async void RaisePropertyChangedEvent([CallerMemberName] string name = null)
        {
            if (name != null)
            {
                await Dispatcher.ResumeForegroundAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        private void SetProperty<TProperty>(ref TProperty property, TProperty value, [CallerMemberName] string name = null)
        {
            if (!property?.Equals(value) ?? (value != null))
            {
                property = value;
                RaisePropertyChangedEvent(name);
            }
        }
    }
}
