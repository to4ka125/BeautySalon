using System;
using System.Collections.Generic;
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

namespace BeautySalon.Model
{
    /// <summary>
    /// Interaction logic for productBlock.xaml
    /// </summary>
    public partial class productBlock : UserControl
    {
        public static readonly DependencyProperty tytleProperty =  DependencyProperty.Register("Tytle",typeof(string),typeof(productBlock));
        public static readonly DependencyProperty StockProperty =  DependencyProperty.Register("Stock", typeof(string),typeof(productBlock));
        public static readonly DependencyProperty PriceProperty =  DependencyProperty.Register("Price", typeof(string),typeof(productBlock));
        public static readonly DependencyProperty IdProperty =  DependencyProperty.Register("ID", typeof(string),typeof(productBlock));
        private static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(productBlock));
        public productBlock()
        {
            InitializeComponent();
        }
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public string Tytle
        {
            get => (string)GetValue(tytleProperty);
            set => SetValue(tytleProperty, value);
        }

        public string Stock
        {
            get => (string)GetValue(StockProperty);
            set => SetValue(StockProperty, value);
        }

        public string Price
        {
            get => (string)GetValue(PriceProperty);
            set => SetValue(PriceProperty, value);
        }

        public string ID
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }
    }
}

