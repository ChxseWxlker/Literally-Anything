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
    public sealed partial class PageItemEditCreate : Page
    {
        //setup selected item
        Item selectedItem = new Item();

        //setup value types
        string[] valueTypes = new string[] { "int", "string" };

        public PageItemEditCreate()
        {
            this.InitializeComponent();
            this.Loaded += PageItemEditCreate_Loaded;
        }

        private void PageItemEditCreate_Loaded(object sender, RoutedEventArgs e)
        {
            //get selected item
            selectedItem = EnvironmentModel.Item;
            EnvironmentModel.Item = new Item();

            //focus element
            txtItem.Focus(FocusState.Keyboard);

            //set properties
            if (selectedItem.Id != 0)
            {
                //setup button
                if (selectedItem.Id != -1)
                    btnSubmit.Content = "Save";

                //set properties
                txtItem.Text = selectedItem.name;

                if (!string.IsNullOrEmpty(selectedItem.valueType))
                    cmbValueType.SelectedIndex = Array.IndexOf(valueTypes, selectedItem.valueType);

                if (!string.IsNullOrEmpty(selectedItem.valueType))
                    cmbValueType.SelectedItem = selectedItem.valueType;
            }
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //check name
            if (string.IsNullOrEmpty(txtItem.Text))
            {
                await new MessageDialog("Item name is required.").ShowAsync();
                return;
            }

            //check value type
            if (cmbValueType.SelectedIndex == -1)
            {
                await new MessageDialog("Item data type is required.").ShowAsync();
                return;
            }

            Item item = new Item();
            item.name = txtItem.Text;
            item.valueType = valueTypes[cmbValueType.SelectedIndex];

            if (selectedItem.Id == 0 || selectedItem.Id == -1)
                item.Id = await Connection.Insert(item);

            else
            {
                item.Id = selectedItem.Id;
                await Connection.Update(item);
            }

            //navigate
            Frame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
