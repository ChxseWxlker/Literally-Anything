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
using CAA_CrossPlatform.UWP.Models;
using Microsoft.Data.Sqlite;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageExcel
    {
        List<Event> visibleEvents = new List<Event>();

        public PageExcel()
        {
            this.InitializeComponent();
            this.Loaded += PageExcel_Loaded;
        }

        private void PageExcel_Loaded(object sender, RoutedEventArgs e)
        {
            //get all events
            List<Event> events = Json.Read("event.json");

            //create list of visible events
            foreach (Event ev in events)
                if (ev.hidden == false)
                {
                    lstEvents.Items.Add(ev.name);
                    visibleEvents.Add(ev);
                }
        }

        private void Events_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageEvent));
        }

        private void Quizes_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageGame));
        }

        private void Questions_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageQuestion));
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            string abbreviation = "";
            string input = "Meet Me There: Doc Magilligan’s Irish Pub";
            DateTime date = new DateTime(2020, 02, 04);
            string[] strSplit = input.Split();
            foreach (string word in strSplit)
            {
                char[] letters = word.ToCharArray();
                abbreviation += char.ToUpper(letters[0]);
            }
            abbreviation += $"{date.Month}{date.Year}";
            await new MessageDialog(abbreviation).ShowAsync();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            List<Event> ev = await Connection.Get("Event");
            Answer ev1 = await Connection.Get("Answer", 2);

            string hi = "";
            foreach (Event yo in ev)
                hi += $"{yo.Id} {yo.name}\n";

            await new MessageDialog(hi).ShowAsync();

            await new MessageDialog(ev1.name).ShowAsync();
        }

        private void chkAllEvents_Checked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectAll();
        }

        private void chkAllEvents_Unchecked(object sender, RoutedEventArgs e)
        {
            lstEvents.SelectedIndex = -1;
        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PageExcel));
        }
    }
}
