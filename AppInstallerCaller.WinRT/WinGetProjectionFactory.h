#pragma once

#include "WinGetProjectionFactory.g.h"
#include "winrt/Microsoft.Management.Deployment.h"

using namespace winrt::Windows::Foundation;
using namespace winrt::Microsoft::Management::Deployment;

namespace winrt::AppInstallerCaller::WinRT::implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory>
    {
        static PackageManager CreatePackageManager(bool useDev);
        static FindPackagesOptions CreateFindPackagesOptions(bool useDev);
        static CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions(bool useDev);
        static InstallOptions CreateInstallOptions(bool useDev);
        static UninstallOptions CreateUninstallOptions(bool useDev);
        static PackageMatchFilter CreatePackageMatchFilter(bool useDev);
        static DownloadOptions CreateDownloadOptions(bool useDev);
        static PackageManagerSettings CreatePackageManagerSettings();

        static PackageManager TryCreatePackageManager(bool useDev);
        static FindPackagesOptions TryCreateFindPackagesOptions(bool useDev);
        static CreateCompositePackageCatalogOptions TryCreateCreateCompositePackageCatalogOptions(bool useDev);
        static InstallOptions TryCreateInstallOptions(bool useDev);
        static UninstallOptions TryCreateUninstallOptions(bool useDev);
        static PackageMatchFilter TryCreatePackageMatchFilter(bool useDev);
        static DownloadOptions TryCreateDownloadOptions(bool useDev);
        static PackageManagerSettings TryCreatePackageManagerSettings();
    };
}

namespace winrt::AppInstallerCaller::WinRT::factory_implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory, implementation::WinGetProjectionFactory>
    {
    };
}
