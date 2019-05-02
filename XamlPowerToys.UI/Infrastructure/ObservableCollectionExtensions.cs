namespace XamlPowerToys.UI.Infrastructure {
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class ObservableCollectionExtensions {

        public static void Sort<T, TKey>(this ObservableCollection<T> observable, Func<T, TKey> keySelector) {
            var sorted = observable.OrderBy(keySelector).ToList();
            observable.Clear();
            foreach (var item in sorted) {
                observable.Add(item);
            }
        }
    }
}
