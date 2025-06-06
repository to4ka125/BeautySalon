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
    /// Interaction logic for sertifacateBlock.xaml
    /// </summary>
    public partial class sertifacateBlock : UserControl
    {
        public static readonly DependencyProperty PriceProperty = DependencyProperty.Register("Price", typeof(string), typeof(sertifacateBlock));
        public static readonly DependencyProperty IDproperty = DependencyProperty.Register("ID", typeof(string), typeof(sertifacateBlock));

        public sertifacateBlock()
        {
            InitializeComponent();
        }

        public string Price
        {
            get => (string)GetValue(PriceProperty);
            set => SetValue(PriceProperty,value);
        }

        public string ID
        {
            get => (string)GetValue(IDproperty);
            set => SetValue(IDproperty, value);
        }
    }
}
