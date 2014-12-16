using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MyExt;

namespace MyExt
{
    public static class MyLinq
    {
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            var rnd
                = new Random();

            return source.OrderBy(item => rnd.Next());
        }
    }
}

namespace DynamicColumnGrid
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<DynamicColumn> dnyColList
            = new ObservableCollection<DynamicColumn>();

        private static int globalIndex = 10;
        private static readonly Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            var testDataList
                = Enumerable
                    .Range(1, 20)
                    .Select(func => new TestData(func))
                    .Randomize()
                    .ToList();

            testDataList
                .SelectMany(func => func.DynamicItem.Keys)
                .Distinct()
                .Select(func =>
                    new DynamicColumn(
                        "DynamicItem",
                        func,
                        typeof(DynamicData)
                        )
                    )
                .ToList()
                .ForEach(dnyColList.Add);

            testGrid.ItemsSource
                = testDataList;

            testGrid.Loaded += (s, e) =>
            {
                testGrid.DynamicColumnList
                 = dnyColList;
            };
        }

        private void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            testGrid.ItemsSource
                = null;
            testGrid.Columns.Clear();

            LoadData();
        }

        private void OnAddClicked(object sender, RoutedEventArgs e)
        {
            var index = ++globalIndex;
            var key = "Key" + index;

            dnyColList.Add(
                new DynamicColumn(
                    "DynamicItem",
                    key,
                    typeof(DynamicData)
                    )
                );

            testGrid.ItemsSource
                .Cast<TestData>()
                .ToList()
                .ForEach(data =>
                    {
                        if (rand.Next() % 2 == 0)
                        {
                            data.DynamicItem.Add(key, new DynamicData(index));
                        }
                    });
        }
    }

    public class TestData
    {
        public string A { get; set; }

        public string B { get; set; }

        public DynamicItem<string, DynamicData> DynamicItem { get; set; }

        public TestData(int index)
        {
            A = "A" + index;
            B = "B" + index;

            DynamicItem
                = new DynamicItem<string, DynamicData>
                    {
                        {   "Pos" + index % 5, new DynamicData(index)  }
                    };
        }
    }

    public class DynamicData
    {
        public string X { get; set; }

        public string Y { get; set; }

        public DynamicData(int index)
        {
            X = "X" + index;
            Y = "Y" + index;
        }
    }
}
