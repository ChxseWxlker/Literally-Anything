using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CAA_CrossPlatform.UWP.Templates
{
    public sealed partial class EventTemplate : UserControl
    {
        public Models.Event Events { get { return this.DataContext as Models.Event; } }

        public EventTemplate()
        {
            this.InitializeComponent();
            
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        string nameSubstring(string name)
        {
            name = name.Substring(0, name.Length - 5);
            //int wordCount = name.Split(" ").Length;
            //if (name.Length > 28)
            //{
            //    string nameSub = name.Substring(0, 25);
            //    return nameSub.Insert(nameSub.Length, "...");
            //}

            //else
            test1 = name;
                return name;
        }

        string dateLong(DateTime date)
        {
            return date.ToString("MMMM dd, yyyy");
        }

        string test1 = "";
        private void lblEventName_Loaded(object sender, RoutedEventArgs e)
        {
            string test11 = test1;
            string test2 = lblEventName.Text;
            int i = 0;
        }
    }
}