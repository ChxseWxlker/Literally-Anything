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
        //setup scroller vertical offset
        private static double scrollerVerticalOffset = 0;

        //setup index instance
        private static PageIndex instance;

        public PageIndex()
        {
            this.InitializeComponent();
            this.Loaded += PageIndex_Loaded;

            //setup name
            Environment.SetEnvironmentVariable("activeUser", "Guest");
            lblUsername.Text = "Welcome Guest";

            //setup instance
            instance = this;
        }

        private void PageIndex_Loaded(object sender, RoutedEventArgs e)
        {
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

                //navigate to item page
                else if (invoked == "Items")
                {
                    if (TemplateFrame.SourcePageType != typeof(PageItem))
                        TemplateFrame.Navigate(typeof(PageItem));
                }
            }
        }

        private void navMenu_Loaded(object sender, RoutedEventArgs e)
        {
            navMenu.IsPaneOpen = false;
            TemplateFrame.Navigate(typeof(PageEvent));
            navMenu.SelectedItem = navMenu.MenuItems[0];
        }

        //show error popup
        public static void ShowError(string error)
        {
            scrollerVerticalOffset = instance.svIndex.VerticalOffset;
            instance.svIndex.ChangeView(instance.svIndex.HorizontalOffset, 0, instance.svIndex.ZoomFactor);
            instance.lblError.Text = error;
            instance.popupError.Width = Window.Current.Bounds.Width - 300;
            instance.spError.Width = Window.Current.Bounds.Width - 300;
            instance.popupError.IsOpen = true;
        }

        //scroll back to previous position
        private void popupError_Closed(object sender, object e)
        {
            svIndex.ChangeView(svIndex.HorizontalOffset, scrollerVerticalOffset, svIndex.ZoomFactor);
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
                ApiHandler.apiKey = "";
                Environment.SetEnvironmentVariable("activeUser", "Guest");
                lblUsername.Text = "Welcome Guest";
                btnLoginPopup.Content = "Login";
                btnGamePage.Visibility = Visibility.Collapsed;
                btnQuestionPage.Visibility = Visibility.Collapsed;
                btnItemPage.Visibility = Visibility.Collapsed;
                if (TemplateFrame.SourcePageType != typeof(PageEventManager))
                    TemplateFrame.Navigate(typeof(PageEvent));
            }
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                lblLoginError.Text = "Please enter a username.";
                lblLoginError.Visibility = Visibility.Visible;
                txtUsername.Focus(FocusState.Keyboard);
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                lblLoginError.Text = "Please enter a password.";
                lblLoginError.Visibility = Visibility.Visible;
                txtPassword.Focus(FocusState.Keyboard);
                return;
            }

            //try logging in
            string loginRes = await Connection.Login(txtUsername.Text, txtPassword.Password);

            //unable to login
            if (loginRes == "_unable")
            {
                lblLoginError.Text = "Incorrect username and/or password, please try again.";
                lblLoginError.Visibility = Visibility.Visible;
                txtUsername.Focus(FocusState.Keyboard);
            }

            //error logging in
            else if (loginRes == "_error")
            {
                lblLoginError.Text = "There was an error trying to login, please try again. If the problem persists contact a server administrator.";
                lblLoginError.Visibility = Visibility.Visible;
            }

            //succesful login
            else
            {
                lblLoginError.Text = "";
                lblLoginError.Visibility = Visibility.Visible;
                popupLogin.IsOpen = false;
                Environment.SetEnvironmentVariable("activeUser", loginRes);
                lblUsername.Text = $"Welcome {loginRes}";
                btnLoginPopup.Content = "Logout";
                btnGamePage.Visibility = Visibility.Visible;
                btnQuestionPage.Visibility = Visibility.Visible;
                btnItemPage.Visibility = Visibility.Visible;
                txtUsername.Text = "";
                txtPassword.Password = "";
                if (TemplateFrame.SourcePageType != typeof(PageEventManager))
                    TemplateFrame.Navigate(typeof(PageEvent));
            }
        }

        private void txtAccount_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnLogin_Click(sender, new RoutedEventArgs());
        }
    }
}
