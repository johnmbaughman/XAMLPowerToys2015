//namespace XamlPowerToys.PackageCommands {

//    using System;
//    using System.ComponentModel.Design;
//    using System.IO;
//    using EnvDTE;
//    using EnvDTE80;
//    using Microsoft.VisualStudio.Shell;
//    using XamlPowerToys.Commands;
//    using XamlPowerToys.Infrastructure;
//    using XamlPowerToys.Model;
//    using Task = System.Threading.Tasks.Task;

//    internal sealed class XamarinXamlPowerToysCommand {
//        public const Int32 CommandId = 256;
//        public static readonly Guid CommandSet = new Guid("D309F791-903F-11D0-9EFC-00A0C911004F");

//        XamarinXamlPowerToysCommand(DTE2 dte2, OleMenuCommandService commandService) {
//            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
//            this.Dte2 = dte2 ?? throw new ArgumentNullException(nameof(dte2));
//            if (commandService != null) {
//                var menuCommandID = new CommandID(CommandSet, CommandId);
//                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
//                menuItem.BeforeQueryStatus += BeforeQueryStatusCallback;
//                commandService.AddCommand(menuItem);
//            }
//        }

//        public static XamarinXamlPowerToysCommand Instance { get; private set; }
//        public DTE2 Dte2 { get; }

//        void BeforeQueryStatusCallback(Object sender, EventArgs e) {
//            ThreadHelper.ThrowIfNotOnUIThread();
//            var result = AssemblyAssistant.GetProjectType(this.Dte2.ActiveDocument.ActiveWindow.Project);
//            var cmd = (OleMenuCommand)sender;
//            cmd.Visible = result == ProjectType.Xamarin && Path.GetExtension(this.Dte2.ActiveDocument.FullName) == ".xaml";
//        }

//        void MenuItemCallback(Object sender, EventArgs e) {
//            ThreadHelper.ThrowIfNotOnUIThread();
//            var dataFormGenerator = new DataFormGenerator(this.Dte2, this.Dte2.ActiveDocument.ActiveWindow.Project);
//            dataFormGenerator.Generate();
//        }

//        public static async Task InitializeAsync(AsyncPackage package) {
//            // Switch to the main thread - the call to AddCommand requires the UI thread.
//            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
//            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
//            var dte = package.GetServiceAsync(typeof(DTE));
//            if (dte == null) {
//                return;
//            }
//            var dte2 = (DTE2)dte;
//            Instance = new XamarinXamlPowerToysCommand(dte2, commandService);
//        }
//    }
//}
