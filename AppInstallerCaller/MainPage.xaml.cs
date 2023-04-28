using AppInstallerCaller.WinRT;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace AppInstallerCaller
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            PackageCatalogs = new ObservableCollection<PackageCatalogReference>();
            InstalledApps = new ObservableCollection<CatalogPackage>();
            InstallingPackages = new ObservableCollection<InstallingPackageView>();
        }

        private PackageManager TryCreatePackageManager() => WinGetProjectionFactory.Instance.TryCreatePackageManager(m_useDev);

        private InstallOptions TryCreateInstallOptions() => WinGetProjectionFactory.Instance.TryCreateInstallOptions(m_useDev);

        private FindPackagesOptions TryCreateFindPackagesOptions() => WinGetProjectionFactory.Instance.TryCreateFindPackagesOptions(m_useDev);

        private CreateCompositePackageCatalogOptions TryCreateCreateCompositePackageCatalogOptions() => WinGetProjectionFactory.Instance.TryCreateCreateCompositePackageCatalogOptions(m_useDev);

        private PackageMatchFilter TryCreatePackageMatchFilter() => WinGetProjectionFactory.Instance.TryCreatePackageMatchFilter(m_useDev);

        private async Task<PackageCatalog> FindSourceAsync(string packageSource)
        {
            PackageManager packageManager = TryCreatePackageManager();
            PackageCatalogReference catalogRef = packageManager.GetPackageCatalogByName(packageSource);
            if (catalogRef != null)
            {
                ConnectResult connectResult = await catalogRef.ConnectAsync();
                // PackageCatalog will be null if connectResult.ErrorCode() is a failure
                PackageCatalog catalog = connectResult.PackageCatalog;
                return catalog;
            }
            return null;
        }

        private async Task<FindPackagesResult> TryFindPackageInCatalogAsync(PackageCatalog catalog, string packageId)
        {
            FindPackagesOptions findPackagesOptions = TryCreateFindPackagesOptions();
            PackageMatchFilter filter = TryCreatePackageMatchFilter();
            filter.Field = PackageMatchField.Id;
            filter.Option = PackageFieldMatchOption.Equals;
            filter.Value = packageId;
            findPackagesOptions.Filters.Add(filter);
            return await catalog.FindPackagesAsync(findPackagesOptions);
        }

        private async Task<CatalogPackage> FindPackageInCatalogAsync(PackageCatalog catalog, string packageId)
        {
            FindPackagesOptions findPackagesOptions = TryCreateFindPackagesOptions();
            PackageMatchFilter filter = TryCreatePackageMatchFilter();
            filter.Field = PackageMatchField.Id;
            filter.Option = PackageFieldMatchOption.Equals;
            filter.Value = packageId;
            findPackagesOptions.Filters.Add(filter);
            FindPackagesResult findPackagesResult = await catalog.FindPackagesAsync(findPackagesOptions);

            List<MatchResult> matches = findPackagesResult.Matches.ToList();
            return matches.Count == 0 ? null : matches.FirstOrDefault().CatalogPackage;
        }

        private IAsyncOperationWithProgress<InstallResult, InstallProgress> InstallPackage(CatalogPackage package)
        {
            PackageManager packageManager = TryCreatePackageManager();
            InstallOptions installOptions = TryCreateInstallOptions();

            // Passing PackageInstallScope::User causes the install to fail if there's no installer that supports that.
            installOptions.PackageInstallScope = PackageInstallScope.Any;
            installOptions.PackageInstallMode = PackageInstallMode.Silent;

            return packageManager.InstallPackageAsync(package, installOptions);
        }

        private async void UpdateUIProgress(InstallProgress progress, ProgressBar progressBar, TextBlock statusText)
        {
            await progressBar.Dispatcher.ResumeForegroundAsync();
            progressBar.Value = progress.DownloadProgress * 100;

            string downloadText = "Downloading. ";
            switch (progress.State)
            {
                case PackageInstallProgressState.Queued:
                    statusText.Text = "Queued";
                    break;
                case PackageInstallProgressState.Downloading:
                    downloadText += Convert.ToString(progress.BytesDownloaded) + " bytes of " + Convert.ToString(progress.BytesRequired);
                    statusText.Text = downloadText;
                    break;
                case PackageInstallProgressState.Installing:
                    statusText.Text = "Installing";
                    progressBar.IsIndeterminate = true;
                    break;
                case PackageInstallProgressState.PostInstall:
                    statusText.Text = "Finishing install";
                    break;
                case PackageInstallProgressState.Finished:
                    statusText.Text = "Finished install.";
                    progressBar.IsIndeterminate = false;
                    break;
                default:
                    statusText.Text = "";
                    break;
            }
        }

        // This method is called from a background thread.
        private async void UpdateUIForInstall(IAsyncOperationWithProgress<InstallResult, InstallProgress> installPackageOperation, Button installButton, Button cancelButton, ProgressBar progressBar, TextBlock statusText)
        {
            installPackageOperation.Progress += (UnnamedParameter, progress) =>
            {
                UpdateUIProgress(progress, progressBar, statusText);
            };

            long installOperationHr = 0L;
            string errorMessage = "Unknown Error";
            InstallResult installResult = null;
            try
            {
                installResult = await installPackageOperation.AsTask();
            }
            catch (OperationCanceledException)
            {
                errorMessage = "Cancelled";
                Debug.WriteLine("Operation was cancelled");
            }
            catch (Exception ex)
            {
                // Operation failed
                // Example: HRESULT_FROM_WIN32(ERROR_DISK_FULL).
                installOperationHr = ex.HResult;
                // Example: "There is not enough space on the disk."
                errorMessage = ex.Message;
                Debug.WriteLine("Operation failed");
            }

            // Switch back to ui thread context.
            await progressBar.Dispatcher.ResumeForegroundAsync();

            cancelButton.IsEnabled = false;
            installButton.IsEnabled = true;
            progressBar.IsIndeterminate = false;

            if (installPackageOperation.Status == AsyncStatus.Canceled)
            {
                installButton.Content = "Retry";
                statusText.Text = "Install cancelled.";
            }
            if (installPackageOperation.Status == AsyncStatus.Error || installResult == null)
            {
                installButton.Content = "Retry";
                statusText.Text = errorMessage;
            }
            else if (installResult.RebootRequired)
            {
                installButton.Content = "Install";
                statusText.Text = "Reboot to finish installation.";
            }
            else if (installResult.Status == InstallResultStatus.Ok)
            {
                installButton.Content = "Install";
                statusText.Text = "Finished.";
            }
            else
            {
                string failText;
                failText = "Install failed: " + installResult.ExtendedErrorCode + " [" + installResult.InstallerErrorCode + "]";
                installButton.Content = "Install";
                statusText.Text = failText;
            }
        }

        private async void GetSources(Button button)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            PackageManager packageManager = TryCreatePackageManager();
            List<PackageCatalogReference> catalogs = packageManager.GetPackageCatalogs().ToList();
            PackageCatalogReference storeCatalog = packageManager.GetPredefinedPackageCatalog(PredefinedPackageCatalog.MicrosoftStore);

            await button.Dispatcher.ResumeForegroundAsync();

            PackageCatalogs.Clear();
            foreach (PackageCatalogReference catalog in catalogs)
            {
                PackageCatalogs.Add(catalog);
            }
            PackageCatalogs.Add(storeCatalog);

            return;
        }

        private async void GetInstalledPackages(TextBlock statusText)
        {
            int selectedIndex = catalogsListBox.SelectedIndex;
            await ThreadSwitcher.ResumeBackgroundAsync();

            PackageManager packageManager = TryCreatePackageManager();

            PackageCatalogReference installedSearchCatalogRef;

            if (selectedIndex < 0)
            {
                installedSearchCatalogRef = packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
            }
            else
            {
                PackageCatalogReference selectedRemoteCatalogRef = PackageCatalogs[selectedIndex];
                CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = TryCreateCreateCompositePackageCatalogOptions();
                createCompositePackageCatalogOptions.Catalogs.Add(selectedRemoteCatalogRef);

                createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                installedSearchCatalogRef = packageManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
            }

            ConnectResult connectResult = await installedSearchCatalogRef.ConnectAsync();
            PackageCatalog installedCatalog = connectResult.PackageCatalog;
            if (installedCatalog == null)
            {
                // Connect Error.
                await statusText.Dispatcher.ResumeForegroundAsync();
                statusText.Text = "Failed to connect to catalog.";
                return;
            }

            FindPackagesOptions findPackagesOptions = TryCreateFindPackagesOptions();

            FindPackagesResult findResult = await TryFindPackageInCatalogAsync(installedCatalog, m_installAppId);
            List<MatchResult> matches = findResult.Matches.ToList();

            await statusText.Dispatcher.ResumeForegroundAsync();
            InstalledApps.Clear();
            foreach (MatchResult match in matches)
            {
                // Filter to only packages that match the selectedCatalogRef
                PackageVersionInfo version = match.CatalogPackage.DefaultInstallVersion;
                if (selectedIndex < 0 || (version != null && version.PackageCatalog.Info.Id == PackageCatalogs[selectedIndex].Info.Id))
                {
                    InstalledApps.Add(match.CatalogPackage);
                }
            }

            statusText.Text = "";
            return;
        }

        private async void GetInstallingPackages(TextBlock statusText)
        {
            int selectedIndex = catalogsListBox.SelectedIndex;
            await ThreadSwitcher.ResumeBackgroundAsync();

            PackageManager packageManager = TryCreatePackageManager();

            PackageCatalogReference installingSearchCatalogRef = null;

            if (selectedIndex < 0)
            {
                // Installing package querying is only really useful if you know what Catalog you're interested in.
                await statusText.Dispatcher.ResumeForegroundAsync();
                statusText.Text = "No catalog selected.";
                return;
            }

            installingSearchCatalogRef = packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstallingPackages);

            PackageCatalogReference selectedRemoteCatalogRef = PackageCatalogs[selectedIndex];
            ConnectResult remoteConnectResult = await selectedRemoteCatalogRef.ConnectAsync();
            PackageCatalog selectedRemoteCatalog = remoteConnectResult.PackageCatalog;
            if (selectedRemoteCatalog == null)
            {
                await statusText.Dispatcher.ResumeForegroundAsync();
                statusText.Text = "Failed to connect to catalog.";
                return;
            }

            ConnectResult connectResult = await installingSearchCatalogRef.ConnectAsync();
            PackageCatalog installingCatalog = connectResult.PackageCatalog;
            if (installingCatalog == null)
            {
                await statusText.Dispatcher.ResumeForegroundAsync();
                statusText.Text = "Failed to connect to catalog.";
                return;
            }

            FindPackagesOptions findPackagesOptions = TryCreateFindPackagesOptions();

            FindPackagesResult findResult = await TryFindPackageInCatalogAsync(selectedRemoteCatalog, m_installAppId);
            List<MatchResult> matches = findResult.Matches.ToList();

            await statusText.Dispatcher.ResumeForegroundAsync();

            InstallingPackages.Clear();
            foreach (MatchResult match in matches)
            {
                InstallingPackageView installingView = new()
                {
                    Package = match.CatalogPackage
                };
                IAsyncOperationWithProgress<InstallResult, InstallProgress> installOperation = packageManager.GetInstallProgress(installingView.Package, selectedRemoteCatalog.Info);
                if (installOperation != null)
                {
                    await statusText.Dispatcher.ResumeForegroundAsync();
                    installingView.AsyncOperation = installOperation;
                    InstallingPackages.Add(installingView);
                }
            }

            statusText.Text = "";
            return;
        }

        private async void StartInstall(Button installButton, Button cancelButton, ProgressBar progressBar, TextBlock statusText)
        {
            installButton.IsEnabled = false;
            cancelButton.IsEnabled = true;
            int selectedIndex = catalogsListBox.SelectedIndex;

            await ThreadSwitcher.ResumeBackgroundAsync();

            if (selectedIndex < 0)
            {
                await installButton.Dispatcher.ResumeForegroundAsync();
                installButton.IsEnabled = false;
                statusText.Text = "No catalog selected to install.";
                return;
            }

            // Get the remote catalog
            PackageCatalogReference selectedRemoteCatalogRef = PackageCatalogs[selectedIndex];
            ConnectResult remoteConnectResult = await selectedRemoteCatalogRef.ConnectAsync();
            PackageCatalog selectedRemoteCatalog = remoteConnectResult.PackageCatalog;
            if (selectedRemoteCatalog == null)
            {
                await progressBar.Dispatcher.ResumeForegroundAsync();
                statusText.Text = "Connecting to catalog failed.";
                return;
            }
            FindPackagesResult findPackagesResult = await TryFindPackageInCatalogAsync(selectedRemoteCatalog, m_installAppId);
            List<MatchResult> matches = findPackagesResult.Matches.ToList();
            if (matches.Count > 0)
            {
                m_installPackageOperation = InstallPackage(matches.FirstOrDefault().CatalogPackage);
                UpdateUIForInstall(m_installPackageOperation, installButton, cancelButton, progressBar, statusText);
            }
        }

        private async void FindPackage(Button installButton, ProgressBar progressBar, TextBlock statusText)
        {
            int selectedIndex = catalogsListBox.SelectedIndex;
            if (selectedIndex < 0)
            {
                await installButton.Dispatcher.ResumeForegroundAsync();
                installButton.IsEnabled = false;
                statusText.Text = "No catalog selected to search.";
                return;
            }

            await ThreadSwitcher.ResumeBackgroundAsync();

            // Get the remote catalog
            PackageCatalogReference selectedRemoteCatalogRef = PackageCatalogs[selectedIndex];
            // Create the composite catalog
            CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = TryCreateCreateCompositePackageCatalogOptions();
            createCompositePackageCatalogOptions.Catalogs.Add(selectedRemoteCatalogRef);
            PackageManager packageManager = TryCreatePackageManager();
            PackageCatalogReference catalogRef = packageManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
            ConnectResult connectResult = await catalogRef.ConnectAsync();
            PackageCatalog compositeCatalog = connectResult.PackageCatalog;
            if (compositeCatalog == null)
            {
                await installButton.Dispatcher.ResumeForegroundAsync();
                installButton.IsEnabled = false;
                statusText.Text = "Failed to connect to catalog.";
                return;
            }

            // Do the search.
            FindPackagesResult findPackagesResult = await TryFindPackageInCatalogAsync(compositeCatalog, m_installAppId);

            List<MatchResult> matches = findPackagesResult.Matches.ToList();
            if (matches.Count > 0)
            {
                PackageVersionInfo installedVersion = matches.FirstOrDefault().CatalogPackage.InstalledVersion;
                if (installedVersion != null)
                {
                    await installButton.Dispatcher.ResumeForegroundAsync();
                    installButton.IsEnabled = false;
                    statusText.Text = "Already installed. Product code: " + installedVersion.ProductCodes.FirstOrDefault();
                }
                else
                {
                    await installButton.Dispatcher.ResumeForegroundAsync();
                    installButton.IsEnabled = true;
                    statusText.Text = "Found the package to install.";
                }
            }
            else
            {
                await installButton.Dispatcher.ResumeForegroundAsync();
                installButton.IsEnabled = false;
                statusText.Text = "Did not find package.";
            }
            return;
        }

        private void SearchButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            m_installAppId = catalogIdTextBox.Text;
            installButton.IsEnabled = false;
            cancelButton.IsEnabled = false;
            installStatusText.Text = "Looking for package.";
            FindPackage(installButton, installProgressBar, installStatusText);
        }

        private void InstallButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            if (m_installPackageOperation == null || m_installPackageOperation.Status != AsyncStatus.Started)
            {
                StartInstall(installButton, cancelButton, installProgressBar, installStatusText);
            }
        }

        private void CancelButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            if (m_installPackageOperation != null && m_installPackageOperation.Status == AsyncStatus.Started)
            {
                m_installPackageOperation.Cancel();
            }
        }

        private void RefreshInstalledButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            GetInstalledPackages(installedStatusText);
        }

        private void ClearInstalledButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            InstalledApps.Clear();
        }

        private void RefreshInstallingButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            GetInstallingPackages(installingStatusText);
        }

        private void ClearInstallingButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            InstallingPackages.Clear();
        }

        private void ToggleDevSwitchToggled(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            m_useDev = toggleDevSwitch.IsOn;
        }

        private void FindSourcesButtonClickHandler(object UnnamedParameter, RoutedEventArgs UnnamedParameter2)
        {
            GetSources(installButton);
        }

        internal ObservableCollection<PackageCatalogReference> PackageCatalogs { get; }

        internal ObservableCollection<CatalogPackage> InstalledApps { get; }

        internal ObservableCollection<InstallingPackageView> InstallingPackages { get; }

        private IAsyncOperationWithProgress<InstallResult, InstallProgress> m_installPackageOperation;
        private string m_installAppId = string.Empty;
        private bool m_useDev = false;
    }
}
