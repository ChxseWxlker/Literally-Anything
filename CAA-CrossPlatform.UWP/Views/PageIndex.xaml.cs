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
using Windows.UI.ViewManagement;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageIndex
    {
        public PageIndex()
        {
            this.InitializeComponent();

            //setup name
            Environment.SetEnvironmentVariable("activeUser", "Guest");
        }

        private void navMenu_Invoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string invoked = args.InvokedItem.ToString();
            
            //check if good call
            if (invoked != null)
            {
                //navigate to event page
                if (invoked == "Events")
                {
                    if (TemplateFrame.SourcePageType != typeof(PageEvent))
                        TemplateFrame.Navigate(typeof(PageEvent));
                }

                //navigate to game page
                else if (invoked == "Games")
                {
                    if (TemplateFrame.SourcePageType != typeof(PageGame))
                        TemplateFrame.Navigate(typeof(PageGame));
                }
            }
        }

        private void navMenu_Back(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            //previous page
            if (TemplateFrame.CanGoBack)
                TemplateFrame.GoBack();
        }

        //setup api
        //static ApiHandler api = new ApiHandler();

        //private void txtLogin_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //        btnLoginPopup_Click(sender, e);
        //}

        //private async void btnLoginPopup_Click(object sender, RoutedEventArgs e)
        //{
        //    //testFrame.Navigate();

        //    //login
        //    string res = "Welcome"; // await api.Login(txtUsername.Text, txtPassword.Password);

        //    //show error message
        //    if (!res.Contains("Welcome"))
        //    {
        //        await new MessageDialog(res).ShowAsync();
        //        return;
        //    }

        //    //set active username
        //    Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

        //    //redirect to events
        //    Frame.Navigate(typeof(PageEvent));
        //}

        //private async void btnRegister_Click(object sender, RoutedEventArgs e)
        //{
        //    //register
        //    string res = await api.Register(txtUsername.Text, txtPassword.Password);

        //    //show error message
        //    if (!res.Contains("Welcome"))
        //    {
        //        await new MessageDialog(res).ShowAsync();
        //        return;
        //    }

        //    //set active username
        //    Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

        //    //redirect to events
        //    Frame.Navigate(typeof(PageEvent));
        //}

        private void navMenu_Loaded(object sender, RoutedEventArgs e)
        {
            navMenu.IsPaneOpen = false;
            TemplateFrame.Navigate(typeof(PageEvent));
        }

        private void btnLoginPopup_Click(object sender, RoutedEventArgs e)
        {
            popupLogin.IsOpen = true;
            popupLogin.Height = Window.Current.Bounds.Height;
            panelPopup.Height = Window.Current.Bounds.Height;
        }
    }
}
