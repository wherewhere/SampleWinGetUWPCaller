﻿#pragma once

#include "WinGetProjectionFactory.g.h"
#include "winrt/Microsoft.Management.Deployment.h"

using namespace winrt::Windows::Foundation;
using namespace winrt::Microsoft::Management::Deployment;

namespace winrt::AppInstallerCaller::WinRT::implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory>
    {
        static AppInstallerCaller::WinRT::WinGetProjectionFactory WinGetProjectionFactory::Instance();

        WinGetProjectionFactory() = default;

        PackageManager CreatePackageManager(bool useDev);
        InstallOptions CreateInstallOptions(bool useDev);
        UninstallOptions CreateUninstallOptions(bool useDev);
        FindPackagesOptions CreateFindPackagesOptions(bool useDev);
        CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions(bool useDev);
        PackageMatchFilter CreatePackageMatchFilter(bool useDev);

        PackageManagerSettings CreatePackageManagerSettings();

        PackageManager CreatePackageManager() { return CreatePackageManager(false); }
        InstallOptions CreateInstallOptions() { return CreateInstallOptions(false); }
        UninstallOptions CreateUninstallOptions() { return CreateUninstallOptions(false); }
        FindPackagesOptions CreateFindPackagesOptions() { return CreateFindPackagesOptions(false); }
        CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions() { return CreateCreateCompositePackageCatalogOptions(false); }
        PackageMatchFilter CreatePackageMatchFilter() { return CreatePackageMatchFilter(false); }

        PackageManager TryCreatePackageManager(bool useDev);
        InstallOptions TryCreateInstallOptions(bool useDev);
        UninstallOptions TryCreateUninstallOptions(bool useDev);
        FindPackagesOptions TryCreateFindPackagesOptions(bool useDev);
        CreateCompositePackageCatalogOptions TryCreateCreateCompositePackageCatalogOptions(bool useDev);
        PackageMatchFilter TryCreatePackageMatchFilter(bool useDev);
        
        PackageManagerSettings TryCreatePackageManagerSettings();

        PackageManager TryCreatePackageManager() { return TryCreatePackageManager(false); }
        InstallOptions TryCreateInstallOptions() { return TryCreateInstallOptions(false); }
        UninstallOptions TryCreateUninstallOptions() { return TryCreateUninstallOptions(false); }
        FindPackagesOptions TryCreateFindPackagesOptions() { return TryCreateFindPackagesOptions(false); }
        CreateCompositePackageCatalogOptions TryCreateCreateCompositePackageCatalogOptions() { return TryCreateCreateCompositePackageCatalogOptions(false); }
        PackageMatchFilter TryCreatePackageMatchFilter() { return TryCreatePackageMatchFilter(false); }

    private:
        static AppInstallerCaller::WinRT::WinGetProjectionFactory instance;
    };
}

namespace winrt::AppInstallerCaller::WinRT::factory_implementation
{
    struct WinGetProjectionFactory : WinGetProjectionFactoryT<WinGetProjectionFactory, implementation::WinGetProjectionFactory>
    {
    };
}
