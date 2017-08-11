using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using SharpGraphEditor.Models;

namespace SharpGraphEditor.Behaviors
{
    public class ListViewWithGridViewBehavior
    {
        public static readonly DependencyProperty MatrixSourceProperty =
            DependencyProperty.RegisterAttached("MatrixSource", typeof(ICollection<TableRow>), typeof(ListViewWithGridViewBehavior),
                new PropertyMetadata(default(ICollection<TableRow>), new PropertyChangedCallback(OnMatrixSourceChanged)));

        public static ICollection<TableRow> GetMatrixSource(DependencyObject d)
        {
            return (ICollection<TableRow>)d.GetValue(MatrixSourceProperty);
        }

        public static void SetMatrixSource(DependencyObject d, ICollection<TableRow> value)
        {
            d.SetValue(MatrixSourceProperty, value);
        }

        private static void OnMatrixSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as ListView;
            var gridView = listView.View as GridView;
            if (listView == null || gridView == null)
            {
                throw new ArgumentNullException($"{nameof(listView)} or {nameof(gridView)} is null");
            }

            var tableRows = e.NewValue as ICollection<TableRow>;
            if (tableRows.Count == 0)
            {
                return;
            }
            listView.ItemsSource = tableRows;

            gridView.Columns.Clear();

            var columnsCount = tableRows.First().Values.Length;

            // In my case, GridView dont suport same size of cells.
            const int GRIDVIEW_CELL_WIDTH = 45;

            for (int i = 0; i < columnsCount; i++)
            {
                gridView.Columns.Add(new GridViewColumn() { Width = GRIDVIEW_CELL_WIDTH });
                foreach (var row in tableRows)
                {
                    if (row != null && row.Values != null && row.Values.Length > 0)
                    {
                        gridView.Columns[i].DisplayMemberBinding = new Binding($"Values[{i}]");
                    }
                }
            }
        }
    }
}
