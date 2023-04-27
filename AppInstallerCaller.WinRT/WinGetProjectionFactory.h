#pragma once

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
        FindPackagesOptions CreateFindPackagesOptions(bool useDev);
        CreateCompositePackageCatalogOptions CreateCreateCompositePackageCatalogOptions(bool useDev);
        PackageMatchFilter CreatePackageMatchFilter(bool useDev);

        IAsyncOperation<bool> GetPackageCatalog();

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
