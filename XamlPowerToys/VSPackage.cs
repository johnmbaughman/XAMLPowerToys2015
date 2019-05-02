namespace XamlPowerToys {

    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.VisualStudio.Shell;
    using XamlPowerToys.PackageCommands;
    using Task = System.Threading.Tasks.Task;

    // The ProvideUIContextRule does prevent the code from running when menu item is clicked.
    // I can't figure out how to hide the button based on these settings that should work.
    // Followed this example: https://github.com/Microsoft/VSSDK-Extensibility-Samples/tree/master/SingleFileGenerator/src

    //[ProvideUIContextRule(PackageGuids.UIContextGuidString, // Must match the GUID in the .vsct file
    //        name: "UIContext",
    //        expression: "xaml", // This will make the button only show on .xaml files
    //        termNames: new[] { "xaml" },
    //        termValues: new[] { "HierSingleSelectionName:.xaml$" })]


    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidXamlPowerToysCommandPackageString)]
    [ProvideAutoLoad(PackageGuids.UIContextGuidString, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideUIContextRule(PackageGuids.UIContextGuidString, // Must match the GUID in the .vsct file
        name: "UIContext",
        expression: "XAML", // This will make the button only show on .xaml files, does not work
        termNames: new[] { "XAML" },
        termValues: new[] { "HierSingleSelectionName:.xaml$" })]
    public sealed class VSPackage : AsyncPackage {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            //await XamarinXamlPowerToysCommand.InitializeAsync(this);
            await XamlPowerToysCommand.InitializeAsync(this);
        }
    }
}
