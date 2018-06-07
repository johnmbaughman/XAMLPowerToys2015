namespace XamlPowerToys.UI.DynamicForm {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using XamlPowerToys.Model;
    using XamlPowerToys.Model.CodeGeneration;
    using XamlPowerToys.UI.Commands;
    using XamlPowerToys.UI.Infrastructure;

    public class CreateFormViewModel : ObservableObject {

        readonly Action<String> _applyAction;
        ICommand _backCommand;
        Boolean _childClassEntityInView;
        ClassEntity _classEntity;
        ICommand _decreaseScaleFactorCommand;
        ICommand _drillBackCommand;
        ICommand _drillIntoCommand;
        ICommand _increaseScaleFactorCommand;
        Boolean _isUIGenerationInProgress;
        Boolean _isXamarinFormsProject;
        ICommand _nextCommand;
        Int32 _numberOfColumns = 1;
        readonly Stack<ClassEntity> _parentChildEntities = new Stack<ClassEntity>();
        readonly String _projectTypeName;
        IList<PropertyInformationViewModel> _propertyInformationCollection;
        Boolean _rootObjectSelectionIsEnabled = true;
        CreateObjectDefinition _selectedCreateObjectDefinition;
        Boolean _showExpandAdvancedOptions;
        Boolean _showExpandedView;
        Boolean _showGenerateForm;
        Double _uIScaleFactor = 1.00;

        public EventHandler CloseWindow;

        public IEnumerable<String> AvailableCommands { get; }

        public IEnumerable<String> AvailableConverters { get; }

        public IEnumerable<String> AvailableDateProperties { get; }

        public IEnumerable<EnumerablePropertyItem> AvailableEnumerableProperties { get; }

        public IEnumerable<String> AvailableNumericProperties { get; }

        public ICommand BackCommand => _backCommand ?? (_backCommand = new RelayCommand(BackCommandExecute));

        public Boolean ChildClassEntityInView {
            get { return _childClassEntityInView; }
            set {
                _childClassEntityInView = value;
                RaisePropertyChanged();
            }
        }

        public ClassEntity ClassEntity {
            get { return _classEntity; }
            private set {
                _classEntity = value;
                RaisePropertyChanged();
                RaisePropertyChanged("ClassName");
            }
        }

        public String ClassName => this.ClassEntity.ClassName;

        public IEnumerable<CreateObjectDefinition> CreateObjectDefinitions { get; }

        public ICommand DecreaseScaleFactorCommand => _decreaseScaleFactorCommand ?? (_decreaseScaleFactorCommand = new RelayCommand(DecreaseScaleFactorCommandExecute, CanDecreaseScaleFactorCommandExecute));

        public ICommand DrillBackCommand => _drillBackCommand ?? (_drillBackCommand = new RelayCommand(DrillBackCommandExecute, CanDrillBackCommandExecute));

        public ICommand DrillIntoCommand => _drillIntoCommand ?? (_drillIntoCommand = new RelayCommand<PropertyInformationViewModel>(DrillIntoCommandExecute, CanDrillIntoCommandExecute));

        public String FormHeader { get; set; }

        public String FormIconImageSource { get; }

        public GenerateFormModel GenerateFormModel { get; }

        public Boolean IncludeButtonBar { get; set; }

        public ICommand IncreaseScaleFactorCommand => _increaseScaleFactorCommand ?? (_increaseScaleFactorCommand = new RelayCommand(IncreaseScaleFactorCommandExecute, CanIncreaseScaleFactorCommandExecute));

        public Boolean IsUIGenerationInProgress {
            get { return _isUIGenerationInProgress; }
            set {
                _isUIGenerationInProgress = value;
                RaisePropertyChanged();
            }
        }

        public Boolean IsXamarinFormsProject {
            get { return _isXamarinFormsProject; }
            set {
                _isXamarinFormsProject = value;
                RaisePropertyChanged();
            }
        }

        public ICommand NextCommand => _nextCommand ?? (_nextCommand = new RelayCommand(NextCommandExecute));

        public IEnumerable<PropertyInformationViewModel> NonBindingControlsCollection { get; private set; }

        public Int32 NumberOfColumns {
            get { return _numberOfColumns; }
            set {
                _numberOfColumns = value;
                if (_numberOfColumns > 1) {
                    this.GenerateFormModel.RootObject = RootObject.Grid;
                    this.RootObjectSelectionIsEnabled = false;
                } else {
                    this.GenerateFormModel.RootObject = RootObject.None;
                    this.RootObjectSelectionIsEnabled = true;
                }
            }
        }

        public IList<PropertyInformationViewModel> PropertyInformationCollection {
            get { return _propertyInformationCollection; }
            private set {
                _propertyInformationCollection = value;
                RaisePropertyChanged();
            }
        }

        public String ResultXaml { get; private set; } = String.Empty;

        public Boolean RootObjectSelectionIsEnabled {
            get { return _rootObjectSelectionIsEnabled; }
            set {
                _rootObjectSelectionIsEnabled = value;
                RaisePropertyChanged();
            }
        }

        public SelectedAction SelectedAction { get; private set; } = SelectedAction.None;

        public CreateObjectDefinition SelectedCreateObjectDefinition {
            get { return _selectedCreateObjectDefinition; }
            set {
                _selectedCreateObjectDefinition = value;
                RaisePropertyChanged();
            }
        }

        public Boolean ShowExpandAdvancedOptions {
            get { return _showExpandAdvancedOptions; }
            set {
                _showExpandAdvancedOptions = value;
                RaisePropertyChanged();
            }
        }

        public Boolean ShowExpandedView {
            get { return _showExpandedView; }
            set {
                _showExpandedView = value;
                RaisePropertyChanged();
            }
        }

        public Boolean ShowGenerateForm {
            get { return _showGenerateForm; }
            set {
                _showGenerateForm = value;
                RaisePropertyChanged();
            }
        }

        public Double UIScaleFactor {
            get { return _uIScaleFactor; }
            set {
                _uIScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<String> UwpTickPlacements { get; }

        public IEnumerable<String> WpfTickPlacements { get; }

        public CreateFormViewModel(ClassEntity classEntity, IEnumerable<String> availableConverters, Action<String> applyAction) {
            if (classEntity == null) {
                throw new ArgumentNullException(nameof(classEntity));
            }
            if (applyAction == null) {
                throw new ArgumentNullException(nameof(applyAction));
            }

            this.ClassEntity = classEntity;
            _projectTypeName = classEntity.ProjectType.ToString();
            _applyAction = applyAction;

            switch (classEntity.ProjectType) {
                case ProjectType.Wpf:
                    this.FormIconImageSource = "../Images/wpfLogo.png";
                    break;
                case ProjectType.Xamarin:
                    this.FormIconImageSource = "../Images/xamarinLogo.png";
                    this.IsXamarinFormsProject = true;
                    break;
                case ProjectType.Uwp:
                    this.FormIconImageSource = "../Images/uwpLogo.png";
                    break;
                case ProjectType.Silverlight:
                    this.FormIconImageSource = "../Images/silverlightLogo.png";
                    break;
            }

            classEntity.PropertyInformationCollection.Sort();
            this.PropertyInformationCollection = classEntity.PropertyInformationCollection;
            this.CreateObjectDefinitions = GetCreateObjectDefinitions();
            this.ShowExpandedView = true;
            this.AvailableDateProperties = GetAvailableDateProperties();
            this.AvailableCommands = GetAvailableCommands();
            this.AvailableEnumerableProperties = GetAvailableEnumerableProperties();
            this.AvailableNumericProperties = GetAvailableNumericProperties();
            this.AvailableConverters = availableConverters;
            this.GenerateFormModel = new GenerateFormModel(classEntity.ProjectType);
            this.NonBindingControlsCollection = GetNonBindingControlsCollection();
            this.UwpTickPlacements = GetUwpTickPlacements();
            this.WpfTickPlacements = GetWpfTickPlacements();
        }

        public void GenerateUI(SelectedAction selectedAction, IList<PropertyInformationViewModel> columnZeroItems, IList<PropertyInformationViewModel> columnOneItems = null, IList<PropertyInformationViewModel> columnTwoItems = null) {
            if (columnZeroItems == null) {
                throw new ArgumentNullException(nameof(columnZeroItems));
            }

            this.ResultXaml = String.Empty;

            this.SelectedAction = selectedAction;

            this.IsUIGenerationInProgress = true;

            this.GenerateFormModel.ColumnLayouts.Clear();

            this.GenerateFormModel.ColumnLayouts.Add(columnZeroItems);
            if (columnOneItems != null && columnOneItems.Count > 0) {
                this.GenerateFormModel.ColumnLayouts.Add(columnOneItems);
            }
            if (columnTwoItems != null && columnTwoItems.Count > 0) {
                this.GenerateFormModel.ColumnLayouts.Add(columnTwoItems);
            }

            this.GenerateFormModel.CreateObjectDefinition = this.SelectedCreateObjectDefinition;

            var codeGenerator = new UIGeneration();

            try {
                var xaml = codeGenerator.Generate(this.GenerateFormModel).Trim();
                if (!String.IsNullOrWhiteSpace(xaml)) {
                    this.ResultXaml = xaml;
                    if (selectedAction == SelectedAction.Generate) {
                        this.RaiseCloseWindow();
                    } else if (selectedAction == SelectedAction.Apply) {
                        _applyAction(this.ResultXaml);
                        this.SelectedAction = SelectedAction.None;
                    }
                } else {
                    MessageBox.Show("No XAML was returned by the UI Code Generator.", "UI Code Generation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } catch (NotImplementedException ex) {
                this.SelectedAction = SelectedAction.None;
                MessageBox.Show(ex.ToString(), "Programmer Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (InvalidOperationException ex) {
                this.SelectedAction = SelectedAction.None;
                MessageBox.Show(ex.ToString(), "Compiler Error", MessageBoxButton.OK, MessageBoxImage.Error);
            } catch (Exception ex) {
                this.SelectedAction = SelectedAction.None;
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.IsUIGenerationInProgress = false;
        }

        protected virtual void RaiseCloseWindow() {
            this.CloseWindow?.Invoke(this, EventArgs.Empty);
        }

        void BackCommandExecute() {
            this.ShowGenerateForm = false;
        }

        Boolean CanDecreaseScaleFactorCommandExecute() {
            return this.UIScaleFactor > 0.5;
        }

        Boolean CanDrillBackCommandExecute() {
            return _parentChildEntities.Count > 0;
        }

        Boolean CanDrillIntoCommandExecute(PropertyInformationViewModel propertyInformationViewModel) {
            return propertyInformationViewModel.IsDrillableProperty;
        }

        Boolean CanIncreaseScaleFactorCommandExecute() {
            return this.UIScaleFactor < 2.0;
        }

        void DecreaseScaleFactorCommandExecute() {
            if (!CanDecreaseScaleFactorCommandExecute()) {
                return;
            }
            this.UIScaleFactor -= 0.1;
        }

        void DrillBackCommandExecute() {
            if (CanDrillBackCommandExecute()) {
                this.ClassEntity = _parentChildEntities.Pop();
                this.PropertyInformationCollection = this.ClassEntity.PropertyInformationCollection;
                this.ChildClassEntityInView = _parentChildEntities.Count > 0;
            }
        }

        void DrillIntoCommandExecute(PropertyInformationViewModel propertyInformationViewModel) {
            if (CanDrillIntoCommandExecute(propertyInformationViewModel)) {
                _parentChildEntities.Push(this.ClassEntity);

                this.ClassEntity = propertyInformationViewModel.ClassEntity;

                if (_parentChildEntities.Count > 1) {
                    var prefix = String.Empty;
                    foreach (var entry in _parentChildEntities.ToArray().Reverse().Skip(1).ToList()) {
                        prefix += $"{entry.ClassName}.";
                    }

                    foreach (var model in propertyInformationViewModel.ClassEntity.PropertyInformationCollection) {
                        if (!model.BindingPath.StartsWith(prefix)) {
                            model.SetNestedBindingPath($"{prefix}{model.BindingPath}");
                        }
                    }
                }

                propertyInformationViewModel.ClassEntity.PropertyInformationCollection.Sort();
                this.PropertyInformationCollection = propertyInformationViewModel.ClassEntity.PropertyInformationCollection;
                this.ChildClassEntityInView = true;
            }
        }

        IEnumerable<String> GetAvailableCommands() {
            var list = new List<String>();
            foreach (var item in this.ClassEntity.PropertyInformationCollection.Where(x => x.TypeName.StartsWith("ICommand"))) {
                list.Add(item.Name);
            }

            return list;
        }

        IEnumerable<String> GetAvailableDateProperties() {
            var list = new List<String>();
            foreach (var item in this.ClassEntity.PropertyInformationCollection.Where(x => x.TypeName.StartsWith("Date"))) {
                list.Add(item.Name);
            }

            return list;
        }

        IEnumerable<EnumerablePropertyItem> GetAvailableEnumerableProperties() {
            var list = new List<EnumerablePropertyItem>();
            foreach (var item in this.ClassEntity.PropertyInformationCollection.Where(x => x.IsEnumerable)) {
                if (item.GenericCollectionClassPropertyNames.Count > 0) {
                    list.Add(new EnumerablePropertyItem(item.Name, item.GenericCollectionClassPropertyNames));
                } else {
                    list.Add(new EnumerablePropertyItem(item.Name));
                }
            }

            return list;
        }

        IEnumerable<String> GetAvailableNumericProperties() {
            var numericNames = "Int16 Int32 Int64 Double Single Decimal";
            var list = new List<String>();
            foreach (var item in this.ClassEntity.PropertyInformationCollection.Where(x => numericNames.IndexOf(x.TypeName, StringComparison.Ordinal) > -1)) {
                list.Add(item.Name);
            }

            return list;
        }

        IEnumerable<CreateObjectDefinition> GetCreateObjectDefinitions() {
            var list = new List<CreateObjectDefinition>();
            foreach (var name in Enum.GetNames(typeof(CreateObject))) {
                if (name.StartsWith(_projectTypeName)) {
                    list.Add(new CreateObjectDefinition((CreateObject)Enum.Parse(typeof(CreateObject), name)));
                }
            }
            return list;
        }

        IEnumerable<PropertyInformationViewModel> GetNonBindingControlsCollection() {
            var list = new List<PropertyInformationViewModel>();
            foreach (var name in Enum.GetNames(typeof(ControlType))) {
                if (name.StartsWith(_projectTypeName)) {
                    if (name.Contains("Button") || name.Contains("Label") || name.Contains("Image")) {
                        //var viewModel = new PropertyInformationViewModel(true, name, String.Empty, String.Empty, this.ClassEntity.ProjectType, String.Empty, false, true);
                        var viewModel = new PropertyInformationViewModel(true, name, String.Empty, String.Empty, this.ClassEntity.ProjectType, String.Empty, false, true);
                        viewModel.ControlDefinition = viewModel.ControlDefinitions.First(x => x.ControlType.ToString() == name);

                        list.Add(viewModel);
                    }
                }
            }
            return list;
        }

        IEnumerable<String> GetUwpTickPlacements() {
            var list = new List<String>();
            foreach (var name in Enum.GetNames(typeof(UwpTickPlacement))) {
                list.Add(name);
            }
            return list;
        }

        IEnumerable<String> GetWpfTickPlacements() {
            var list = new List<String>();
            foreach (var name in Enum.GetNames(typeof(WpfTickPlacement))) {
                list.Add(name);
            }
            return list;
        }

        void IncreaseScaleFactorCommandExecute() {
            if (!CanIncreaseScaleFactorCommandExecute()) {
                return;
            }
            this.UIScaleFactor += 0.1;
        }

        void NextCommandExecute() {
            this.ShowGenerateForm = true;
        }

    }
}
