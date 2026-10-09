#include "pch.h"
#include "WinGetProjectionFactory.h"
#include "WinGetProjectionFactory.g.cpp"

using namespace winrt::Windows::Foundation;
using namespace winrt::Microsoft::Management::Deployment;

namespace winrt::AppInstallerCaller::WinRT::implementation
{
    // CLSIDs for WinGet package
    const CLSID CLSID_PackageManager = { 0xC53A4F16, 0x787E, 0x42A4, 0xB3, 0x04, 0x29, 0xEF, 0xFB, 0x4B, 0xF5, 0x97 };  //C53A4F16-787E-42A4-B304-29EFFB4BF597
    const CLSID CLSID_InstallOptions = { 0x1095f097, 0xEB96, 0x453B, 0xB4, 0xE6, 0x16, 0x13, 0x63, 0x7F, 0x3B, 0x14 };  //1095F097-EB96-453B-B4E6-1613637F3B14
    const CLSID CLSID_FindPackagesOptions = { 0x572DED96, 0x9C60, 0x4526,{ 0x8F, 0x92, 0xEE, 0x7D, 0x91, 0xD3, 0x8C, 0x1A } }; //572DED96-9C60-4526-8F92-EE7D91D38C1A
    const CLSID CLSID_PackageMatchFilter = { 0xD02C9DAF, 0x99DC, 0x429C,{ 0xB5, 0x03, 0x4E, 0x50, 0x4E, 0x4A, 0xB0, 0x00 } }; //D02C9DAF-99DC-429C-B503-4E504E4AB000
    const CLSID CLSID_CreateCompositePackageCatalogOptions = { 0x526534B8, 0x7E46, 0x47C8,{ 0x84, 0x16, 0xB1, 0x68, 0x5C, 0x32, 0x7D, 0x37 } }; //526534B8-7E46-47C8-8416-B1685C327D37
    const CLSID CLSID_DownloadOptions = { 0X4CBABE76, 0X7322, 0X4BE4,{ 0X9C, 0XEA, 0X25, 0X89, 0XA8, 0X06, 0X82, 0XDC } };  //4CBABE76-7322-4BE4-9CEA-2589A80682DC

    // CLSIDs for WinGetDev package
    const CLSID CLSID_PackageManager2 = { 0x74CB3139, 0xB7C5, 0x4B9E,{ 0x93, 0x88, 0xE6, 0x61, 0x6D, 0xEA, 0x28, 0x8C } };  //74CB3139-B7C5-4B9E-9388-E6616DEA288C
    const CLSID CLSID_InstallOptions2 = { 0x44FE0580, 0x62F7, 0x44D4, 0x9E, 0x91, 0xAA, 0x96, 0x14, 0xAB, 0x3E, 0x86 };  //44FE0580-62F7-44D4-9E91-AA9614AB3E86
    const CLSID CLSID_FindPackagesOptions2 = { 0x1BD8FF3A, 0xEC50, 0x4F69,{ 0xAE, 0xEE, 0xDF, 0x4C, 0x9D, 0x3B, 0xAA, 0x96 } }; //1BD8FF3A-EC50-4F69-AEEE-DF4C9D3BAA96
    const CLSID CLSID_PackageMatchFilter2 = { 0x3F85B9F4, 0x487A, 0x4C48,{ 0x90, 0x35, 0x29, 0x03, 0xF8, 0xA6, 0xD9, 0xE8 } }; //3F85B9F4-487A-4C48-9035-2903F8A6D9E8
    const CLSID CLSID_CreateCompositePackageCatalogOptions2 = { 0xEE160901, 0xB317, 0x4EA7,{ 0x9C, 0xC6, 0x53, 0x55, 0xC6, 0xD7, 0xD8, 0xA7 } }; //EE160901-B317-4EA7-9CC6-5355C6D7D8A7
    const CLSID CLSID_DownloadOptions2 = { 0X8EF324ED, 0X367C, 0X4880,{ 0X83, 0XE5, 0XBB, 0X2A, 0XBD, 0X0B, 0X72, 0XF6 } };  //8EF324ED-367C-4880-83E5-BB2ABD0B72F6

    PackageManager WinGetProjectionFactory::CreatePackageManager(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<PackageManager>(CLSID_PackageManager2, CLSCTX_ALL);
        }
        return winrt::create_instance<PackageManager>(CLSID_PackageManager, CLSCTX_ALL);
    }

    FindPackagesOptions WinGetProjectionFactory::CreateFindPackagesOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<FindPackagesOptions>(CLSID_FindPackagesOptions2, CLSCTX_ALL);
        }
        return winrt::create_instance<FindPackagesOptions>(CLSID_FindPackagesOptions, CLSCTX_ALL);
    }

    CreateCompositePackageCatalogOptions WinGetProjectionFactory::CreateCreateCompositePackageCatalogOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<CreateCompositePackageCatalogOptions>(CLSID_CreateCompositePackageCatalogOptions2, CLSCTX_ALL);
        }
        return winrt::create_instance<CreateCompositePackageCatalogOptions>(CLSID_CreateCompositePackageCatalogOptions, CLSCTX_ALL);
    }

    InstallOptions WinGetProjectionFactory::CreateInstallOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<InstallOptions>(CLSID_InstallOptions2, CLSCTX_ALL);
        }
        return winrt::create_instance<InstallOptions>(CLSID_InstallOptions, CLSCTX_ALL);
    }

    PackageMatchFilter WinGetProjectionFactory::CreatePackageMatchFilter(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<PackageMatchFilter>(CLSID_PackageMatchFilter2, CLSCTX_ALL);
        }
        return winrt::create_instance<PackageMatchFilter>(CLSID_PackageMatchFilter, CLSCTX_ALL);
    }

    DownloadOptions WinGetProjectionFactory::CreateDownloadOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::create_instance<DownloadOptions>(CLSID_DownloadOptions2, CLSCTX_ALL);
        }
        return winrt::create_instance<DownloadOptions>(CLSID_DownloadOptions, CLSCTX_ALL);
    }

    PackageManager WinGetProjectionFactory::TryCreatePackageManager(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<PackageManager>(CLSID_PackageManager2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<PackageManager>(CLSID_PackageManager, CLSCTX_ALL);
    }

    FindPackagesOptions WinGetProjectionFactory::TryCreateFindPackagesOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<FindPackagesOptions>(CLSID_FindPackagesOptions2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<FindPackagesOptions>(CLSID_FindPackagesOptions, CLSCTX_ALL);
    }

    CreateCompositePackageCatalogOptions WinGetProjectionFactory::TryCreateCreateCompositePackageCatalogOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<CreateCompositePackageCatalogOptions>(CLSID_CreateCompositePackageCatalogOptions2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<CreateCompositePackageCatalogOptions>(CLSID_CreateCompositePackageCatalogOptions, CLSCTX_ALL);
    }

    InstallOptions WinGetProjectionFactory::TryCreateInstallOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<InstallOptions>(CLSID_InstallOptions2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<InstallOptions>(CLSID_InstallOptions, CLSCTX_ALL);
    }

    PackageMatchFilter WinGetProjectionFactory::TryCreatePackageMatchFilter(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<PackageMatchFilter>(CLSID_PackageMatchFilter2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<PackageMatchFilter>(CLSID_PackageMatchFilter, CLSCTX_ALL);
    }

    DownloadOptions WinGetProjectionFactory::TryCreateDownloadOptions(const bool useDev)
    {
        if (useDev)
        {
            return winrt::try_create_instance<DownloadOptions>(CLSID_DownloadOptions2, CLSCTX_ALL);
        }
        return winrt::try_create_instance<DownloadOptions>(CLSID_DownloadOptions, CLSCTX_ALL);
    }
}
