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
            lblUsername.Text = "Welcome Guest";

            //remove focus
            TextBox txtBox = new TextBox();
            txtBox.Focus(FocusState.Pointer);
            txtBox.IsFocusEngaged = false;
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

                //navigate to question page
                else if (invoked == "Questions")
                {
                    if (TemplateFrame.SourcePageType != typeof(PageQuestion))
                        TemplateFrame.Navigate(typeof(PageQuestion));
                }
            }
        }

        private void navMenu_Loaded(object sender, RoutedEventArgs e)
        {
            navMenu.IsPaneOpen = false;
            TemplateFrame.Navigate(typeof(PageEvent));
            navMenu.SelectedItem = navMenu.MenuItems[0];
        }

        private void btnLoginPopup_Click(object sender, RoutedEventArgs e)
        {
            Button btnSender = (Button)sender;

            //login user
            if (btnSender.Content.ToString() == "Login")
            {
                popupLogin.IsOpen = true;
                popupLogin.Height = Window.Current.Bounds.Height;
                panelPopup.Height = Window.Current.Bounds.Height;
                txtUsername.Focus(FocusState.Keyboard);
            }

            //logout user
            else if (btnSender.Content.ToString() == "Logout")
            {
                Environment.SetEnvironmentVariable("activeUser", "Guest");
                lblUsername.Text = "Welcome Guest";
                btnLoginPopup.Content = "Login";
                btnGamePage.Visibility = Visibility.Collapsed;
                btnQuestionPage.Visibility = Visibility.Collapsed;
                if (TemplateFrame.SourcePageType != typeof(PageEventManager))
                    TemplateFrame.Navigate(typeof(PageEvent));
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                txtUsername.Focus(FocusState.Keyboard);
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                txtPassword.Focus(FocusState.Keyboard);
                return;
            }

            popupLogin.IsOpen = false;
            Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);
            lblUsername.Text = $"Welcome {txtUsername.Text}";
            btnLoginPopup.Content = "Logout";
            btnGamePage.Visibility = Visibility.Visible;
            btnQuestionPage.Visibility = Visibility.Visible;
            txtUsername.Text = "";
            txtPassword.Password = "";
            if (TemplateFrame.SourcePageType != typeof(PageEventManager))
                TemplateFrame.Navigate(typeof(PageEvent));
        }

        private void txtAccount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnLogin_Click(sender, new RoutedEventArgs());
        }
    }
}
