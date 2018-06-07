namespace XamlPowerToys.UI.SelectClassFromAssemblies {
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using XamlPowerToys.Model;

    public partial class SelectClassFromAssembliesView : Window {

        public ClassEntity SelectedClassEntity { get; private set; }

        public SelectClassFromAssembliesView(ClassEntities classEntities, String sourceCommandName, String xamlFileClassName) {
            if (classEntities == null) {
                throw new ArgumentNullException(nameof(classEntities));
            }
            if (sourceCommandName == null) {
                throw new ArgumentNullException(nameof(sourceCommandName));
            }

            var classEntitiesCollectionView = CollectionViewSource.GetDefaultView(classEntities) as CollectionView;
            if (classEntitiesCollectionView == null) {
                throw new NullReferenceException();
            }

            // ReSharper disable PossibleNullReferenceException
            classEntitiesCollectionView.GroupDescriptions.Clear();

            // ReSharper restore PossibleNullReferenceException
            classEntitiesCollectionView.SortDescriptions.Clear();
            classEntitiesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("AssemblyName"));
            classEntitiesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("NamespaceName"));
            classEntitiesCollectionView.SortDescriptions.Add(new SortDescription("AssemblyName", ListSortDirection.Ascending));
            classEntitiesCollectionView.SortDescriptions.Add(new SortDescription("NamespaceName", ListSortDirection.Ascending));
            classEntitiesCollectionView.SortDescriptions.Add(new SortDescription("ClassName", ListSortDirection.Ascending));

            InitializeComponent();

            this.tvObjects.ItemsSource = classEntitiesCollectionView.Groups;
            this.tbCommandCaption.Text = String.Concat("For ", sourceCommandName);

            this.tvObjects.SelectedItemChanged += tvObjects_SelectedItemChanged;
            this.lbViewModels.SelectionChanged += LbViewModels_SelectionChanged;

            this.lbViewModels.ItemsSource = classEntities.Where(x => x.ClassName.ToLower(CultureInfo.InvariantCulture).EndsWith("viewmodel") || x.ClassName.ToLower().EndsWith("pagemodel")).OrderBy(x => x.ClassName).ToList();
            foreach (ClassEntity item in this.lbViewModels.Items) {
                if (item.ClassName.StartsWith(xamlFileClassName)) {
                    this.lbViewModels.SelectedItem = item;
                    break;
                }
            }
        }

        void btnCancel_Click(Object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        void btnNext_Click(Object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }

        void LbViewModels_SelectionChanged(Object sender, SelectionChangedEventArgs e) {
            if (this.lbViewModels.SelectedItem != null) {
                this.SelectedClassEntity = (ClassEntity)this.lbViewModels.SelectedItem;
                this.btnNext.IsEnabled = true;
            } else {
                this.btnNext.IsEnabled = false;
                this.SelectedClassEntity = null;
            }
        }

        void tvObjects_SelectedItemChanged(Object sender, RoutedPropertyChangedEventArgs<Object> e) {
            if (e.NewValue is ClassEntity) {
                this.lbViewModels.SelectedItem = null;
                this.btnNext.IsEnabled = true;
                this.SelectedClassEntity = (ClassEntity)e.NewValue;
            } else {
                this.btnNext.IsEnabled = false;
                this.SelectedClassEntity = null;
            }
        }

    }
}
