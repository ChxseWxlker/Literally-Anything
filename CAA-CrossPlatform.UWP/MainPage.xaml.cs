using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Events));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Games));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Questions));
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Event> events = await Excel.Load();
                string eventsStr = "";
                foreach (Event ev in events)
                {
                    eventsStr += $"{ev.id} {ev.name} {ev.location} {ev.startDate} {ev.endDate} {ev.game}\n";
                }

                await new MessageDialog(eventsStr).ShowAsync();
            }
            catch(Exception ex) { }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<Event> events = Json.Read("event.json");
            Excel.Save(events);
        }
    }
}
