using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiBody
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool color = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        void OnClick1(object sender, RoutedEventArgs e)
        {
            if (color)
            {
                btn1.Background = new SolidColorBrush(ColorHelper.FromArgb(255, 127, 127, 127));
                color = !color;
            }
            else
            {
                btn1.Background = new SolidColorBrush(ColorHelper.FromArgb(60, 100, 23, 27));
                color = !color;
            }
        }
    }
}
