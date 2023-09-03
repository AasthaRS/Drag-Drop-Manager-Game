using C1.WPF.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DragDropManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private C1DragDropManager _c1DragDropManager;
        private int _initRow = -1;
        private int _initCol = -1;
        private int _newRow = -1;
        private int _newCol = -1;
        private UIElement _oldEle = null;
        private UIElement _newEle = null;
        private static List<(int, int, UIElement)> elements;

        public MainWindow()
        {
            InitializeComponent();
            elements = new List<(int, int, UIElement)>();
            _c1DragDropManager = new C1DragDropManager();
            _c1DragDropManager.RegisterDropTarget(layoutRoot, true);
            foreach (UIElement ele in layoutRoot.Children)
            {
                _c1DragDropManager.RegisterDragSource(ele, DragDropEffect.Move, ModifierKeys.None);
                var row = Grid.GetRow(ele as UIElement);
                var col = Grid.GetColumn(ele as UIElement);
                elements.Add((row, col, ele));
            }
            _c1DragDropManager.DragDrop += _c1DragDropManager_DragDrop;
            //_c1DragDropManager.DragOver +=
            _c1DragDropManager.DragEnter += new DragDropEventHandler(_c1DragDropManager_DragEnter);
            _c1DragDropManager.DragLeave += new DragDropEventHandler(_c1DragDropManager_DragLeave);
        }

        private void _c1DragDropManager_DragDrop(object source, DragDropEventArgs e)
        {
            _oldEle = null;
            _newEle = null;
            _newCol = -1;
            _newRow = -1;
            bool swap = false;
            // Get mouse position
            Point pMouse = e.GetPosition(layoutRoot);
            // Translate into grid row/col coordinates
            int row, col;
            Point pGrid = new Point(0, 0);
            for (row = 0; row < layoutRoot.RowDefinitions.Count; row++)
            {
                pGrid.Y += layoutRoot.RowDefinitions[row].ActualHeight;
                if (pGrid.Y > pMouse.Y)
                    break;
            }
            for (col = 0; col < layoutRoot.ColumnDefinitions.Count; col++)
            {
                pGrid.X += layoutRoot.ColumnDefinitions[col].ActualWidth;
                if (pGrid.X > pMouse.X)
                    break;
            }
            
            // Move the element to the new position
            e.DragSource.SetValue(Grid.RowProperty, row);
            e.DragSource.SetValue(Grid.ColumnProperty, col);

            _oldEle = layoutRoot.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

            foreach (var element in elements)
            {
                _newCol = col;
                _newRow = row;
                if (element.Item1 == row && element.Item2 == col)
                {
                    _newEle = element.Item3;                    
                    //_oldEle = layoutRoot.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
                    swap = true;
                }
                else
                {
                    _newEle = null;
                    swap = false;
                }
            }


            if (_initRow != -1 && _initCol != -1 && _oldEle != null)
            {
                if(!swap)
                {
                    elements.Remove((_initRow, _initCol, _oldEle));
                    elements.Add((_initRow, _initCol, _newEle));
                }
                else
                {
                    layoutRoot.Children.Remove(_newEle);
                    Grid.SetRow(_newEle, _initRow);
                    Grid.SetColumn(_newEle, _initCol);
                    layoutRoot.Children.Add(_newEle);
                    elements.Remove((_initRow, _initCol, _oldEle));
                    elements.Remove((_newRow, _newCol, _newEle));

                    elements.Add((_initRow, _initCol, _newEle));
                    elements.Add((_newRow, _newCol, _oldEle));
                }
                

            }

            _initRow = -1;
            _initCol = -1;
            _oldEle = null;
            _newEle = null;
            _newCol = -1;
            _newRow = -1;

            

        }

        void _c1DragDropManager_DragEnter(object source, DragDropEventArgs e)
        {
            var target = (Grid)e.DropTarget;            
            _initRow = -1;
            _initCol = -1;
            target.Tag = target.Background;
            target.Background = new SolidColorBrush();
            // Get mouse position
            Point pMouse = e.GetPosition(layoutRoot);
            // Translate into grid row/col coordinates
            int row, col;
            Point pGrid = new Point(0, 0);
            for (row = 0; row < layoutRoot.RowDefinitions.Count; row++)
            {
                pGrid.Y += layoutRoot.RowDefinitions[row].ActualHeight;
                if (pGrid.Y > pMouse.Y)
                    break;
            }
            for (col = 0; col < layoutRoot.ColumnDefinitions.Count; col++)
            {
                pGrid.X += layoutRoot.ColumnDefinitions[col].ActualWidth;
                if (pGrid.X > pMouse.X)
                    break;
            }
            if(layoutRoot.Children.Cast<UIElement>().First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col) != null)
            {
                _initRow = row;
                _initCol = col;
            }

            
        }

        void _c1DragDropManager_DragLeave(object source, DragDropEventArgs e)
        {
            var target = (Grid)e.DropTarget;
            bool flag = false;
            target.Background = (Brush)target.Tag;
        }
    }
}
