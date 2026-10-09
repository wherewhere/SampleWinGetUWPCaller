#pragma once

#include "WinGetProjectionFactory.g.h"
#include "winrt/Microsoft.Management.Deployment.h"

using namespace winrt::Windows::Foundation;
using namespace winrt::Microsoft::Management::Deployment;

namespace winrt::AppInstallerCaller::WinRT::implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory>
    {
        static PackageManager CreatePackageManager(const bool useDev);
        static FindPackagesOptions CreateFindPackagesOptions(const bool useDev);
        static CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions(const bool useDev);
        static InstallOptions CreateInstallOptions(const bool useDev);
        static UninstallOptions CreateUninstallOptions(const bool useDev);
        static PackageMatchFilter CreatePackageMatchFilter(const bool useDev);
        static DownloadOptions CreateDownloadOptions(const bool useDev);
        static PackageManagerSettings CreatePackageManagerSettings();

        static PackageManager TryCreatePackageManager(const bool useDev);
        static FindPackagesOptions TryCreateFindPackagesOptions(const bool useDev);
        static CreateCompositePackageCatalogOptions TryCreateCreateCompositePackageCatalogOptions(const bool useDev);
        static InstallOptions TryCreateInstallOptions(const bool useDev);
        static UninstallOptions TryCreateUninstallOptions(const bool useDev);
        static PackageMatchFilter TryCreatePackageMatchFilter(const bool useDev);
        static DownloadOptions TryCreateDownloadOptions(const bool useDev);
        static PackageManagerSettings TryCreatePackageManagerSettings();
    };
}

namespace winrt::AppInstallerCaller::WinRT::factory_implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory, implementation::WinGetProjectionFactory>
    {
    };
}
