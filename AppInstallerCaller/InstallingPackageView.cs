using Microsoft.Management.Deployment;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;

namespace AppInstallerCaller
{
    internal class InstallingPackageView : INotifyPropertyChanged
    {
        public CatalogPackage Package { get; set; }

        private IAsyncOperationWithProgress<InstallResult, InstallProgress> m_asyncOperation;
        public IAsyncOperationWithProgress<InstallResult, InstallProgress> AsyncOperation
        {
            get => m_asyncOperation;
            set
            {
                m_asyncOperation = value;
                m_asyncOperation.Progress += (UnnamedParameter, progress) => UpdateUIProgress(progress, this);
            }
        }

        private async void UpdateUIProgress(InstallProgress progress, InstallingPackageView view)
        {
            await view.Dispatcher.ResumeForegroundAsync();
            view.Progress = progress.DownloadProgress * 100;
        }

        private double m_progress = 0;
        public double Progress
        {
            get => m_progress;
            set
            {
                m_progress = value;
                RaisePropertyChangedEvent();
            }
        }

        public string StatusText { get; set; }
        public CoreDispatcher Dispatcher { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            if (name != null) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
        }
    }
}
