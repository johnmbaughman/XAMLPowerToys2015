namespace XamlPowerToys.Commands {
    using System;
    using System.IO;
    using EnvDTE;
    using EnvDTE80;
    using XamlPowerToys.Infrastructure;
    using XamlPowerToys.Model;
    using XamlPowerToys.Reflection;
    using XamlPowerToys.UI.DynamicForm;
    using XamlPowerToys.UI.Infrastructure;

    internal class DataFormGenerator {

        readonly Project _activeProject;
        readonly DTE2 _dte2;
        readonly String _projectFrameworkVersion;
        readonly ProjectType _projectType;
        Boolean _hasBeenApplied = false;

        public DataFormGenerator(DTE2 dte2) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (dte2 == null) {
                throw new ArgumentNullException(nameof(dte2));
            }
            _dte2 = dte2;
            _activeProject = dte2.ActiveDocument.ActiveWindow.Project;
            _projectType = AssemblyAssistant.GetProjectType(dte2.ActiveDocument.ActiveWindow.Project);
            _projectFrameworkVersion = AssemblyAssistant.GetProjectFrameworkVersion(dte2.ActiveDocument.ActiveWindow.Project);
        }

        public void Generate() {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            try {
                var xamlFileClassName = Path.GetFileNameWithoutExtension(_dte2.ActiveDocument.Name);
                var typeReflector = new TypeReflector();
                var typeReflectorResult = typeReflector.SelectClassFromAllReferencedAssemblies(_activeProject, xamlFileClassName, "Data Form Generator", _projectType, _projectFrameworkVersion);
                if (typeReflectorResult != null) {
                    var win = new XamlPowerToysWindow();
                    var vm = new CreateFormViewModel(typeReflectorResult.ClassEntity, typeReflectorResult.AvailableConverters, ApplyChanges);
                    var view = new CreateFormView();
                    win.DataContext = vm;
                    win.rootGrid.Children.Add(view);
                    win.ShowDialog();
                    if (vm.SelectedAction == SelectedAction.Generate) {
                        InsertXaml(vm.ResultXaml);
                    }
                }
            } catch (Exception ex) {
                DialogAssistant.ShowExceptionMessage(ex);
            }
        }

        void ApplyChanges(String xaml) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            InsertXaml(xaml);
            _hasBeenApplied = true;
        }

        void InsertXaml(String xaml) {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (_hasBeenApplied) {
                _dte2.ExecuteCommand("Edit.Undo");
                _dte2.ExecuteCommand("Edit.Undo");
            }
            var ts = (TextSelection)_dte2.ActiveDocument.Selection;
            ts.Insert(xaml);
            _dte2.ExecuteCommand("Edit.FormatDocument");
        }

    }
}
