namespace XamlPowerToys.PackageCommands {

    using System;
    using System.ComponentModel.Design;
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using XamlPowerToys.Commands;
    using Task = System.Threading.Tasks.Task;

    internal sealed class XamarinXamlPowerToysCommand {
        static DTE2 _dte2;
        public const Int32 CommandId = 256;
        public static readonly Guid CommandSet = new Guid("D309F791-903F-11D0-9EFC-00A0C911004F");

        static void MenuItemCallback(Object sender, EventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_dte2.ActiveDocument.Name.EndsWith(".xaml")) {  // this is not needed but more insurance since I can't hide the menu item.
                var dataFormGenerator = new DataFormGenerator(_dte2);
                dataFormGenerator.Generate();
            }
        }

        public static async Task InitializeAsync(AsyncPackage package) {
            var dte = await package.GetServiceAsync(typeof(DTE));
            if (dte == null) {
                return;
            }
            _dte2 = (DTE2)dte;

            if (await package.GetServiceAsync(typeof(IMenuCommandService)) is IMenuCommandService commandService) {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID) {
                    // this does not work, found here https://github.com/Microsoft/VSSDK-Extensibility-Samples/blob/master/SingleFileGenerator/src/Commands/ApplyCustomTool.cs
                    // This will defer visibility control to the VisibilityConstraints section in the .vsct file
                    Supported = false
                };

                commandService.AddCommand(menuItem);
            }
        }
    }
}
