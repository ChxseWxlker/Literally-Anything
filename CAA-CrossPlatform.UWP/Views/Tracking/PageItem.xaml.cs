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
using CAA_CrossPlatform.UWP.Models;
using Windows.UI.Popups;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageItem : Page
    {
        //get list of items
        List<Item> visibleItems = new List<Item>();

        public PageItem()
        {
            this.InitializeComponent();
            this.Loaded += PageItem_Loaded;
        }

        private async void PageItem_Loaded(object sender, RoutedEventArgs e)
        {
            //reset environment vars
            EnvironmentModel.Reset();

            //get all items
            List<Item> items = await Connection.Get("Item");

            //add question if visible
            foreach (Item item in items)
                if (item.hidden == false)
                {
                    lbItem.Items.Add(item.name);
                    visibleItems.Add(item);
                }
        }

        private async void btnControls_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            if (btnSender.Name.Contains("Create"))
                Frame.Navigate(typeof(PageItemEditCreate));

            else if (btnSender.Name.Contains("Edit"))
            {
                if (lbItem.SelectedIndex == -1)
                {
                    PageIndex.ShowError("Choose an item to edit.");
                    return;
                }

                EnvironmentModel.Item = visibleItems[lbItem.SelectedIndex];
                Frame.Navigate(typeof(PageItemEditCreate));
            }

            else if (btnSender.Name.Contains("Delete"))
            {
                if (lbItem.SelectedIndex == -1)
                {
                    PageIndex.ShowError("Choose an item to delete.");
                    return;
                }

                await Connection.Delete(visibleItems[lbItem.SelectedIndex]);

                Frame.Navigate(typeof(PageItem));
            }
        }
    }
}
