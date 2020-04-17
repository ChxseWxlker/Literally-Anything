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

        //find element
        public static dynamic GetElement(string elementName)
        {
            return instance.FindName(elementName);
        }

        //initial visual state
        public static void InitiateVisualState(VisualState mobile, VisualState middle, VisualState tablet, VisualState desktop)
        {
            //mobile
            if (Window.Current.Bounds.Width < 501)
            {
                instance.header.Height = 70;
                instance.relativePanel.Height = 70;
                instance.textBlock.FontSize = 20;
                instance.textBlock.Margin = new Thickness(10, 0, 0, 0);
                instance.CAAImg.Height = 60;
                instance.CAAImg.Width = 70;
                instance.btnLoginPopup.Height = 25;
                instance.btnLoginPopup.Width = 75;
                instance.btnLoginPopup.FontSize = 11;
            }

            //middle
            else if (Window.Current.Bounds.Width < 651 && Window.Current.Bounds.Width > 500)
            {
                instance.header.Height = 80;
                instance.relativePanel.Height = 80;
                instance.textBlock.FontSize = 30;
                instance.textBlock.Margin = new Thickness(15, 0, 0, 0);
                instance.CAAImg.Height = 70;
                instance.CAAImg.Width = 80;
                instance.btnLoginPopup.Height = 30;
                instance.btnLoginPopup.Width = 90;
                instance.btnLoginPopup.FontSize = 14;
            }

            //tablet
            else if (Window.Current.Bounds.Width < 951 && Window.Current.Bounds.Width > 650)
            {
                instance.header.Height = 90;
                instance.relativePanel.Height = 90;
                instance.textBlock.FontSize = 40;
                instance.textBlock.Margin = new Thickness(20, 0, 0, 0);
                instance.CAAImg.Height = 90;
                instance.CAAImg.Width = 100;
                instance.btnLoginPopup.Height = 38;
                instance.btnLoginPopup.Width = 130;
                instance.btnLoginPopup.FontSize = 18;
            }

            //desktop
            else if (Window.Current.Bounds.Width > 950)
            {
                instance.header.Height = 100;
                instance.relativePanel.Height = 100;
                instance.textBlock.FontSize = 45;
                instance.textBlock.Margin = new Thickness(20, 0, 0, 0);
                instance.CAAImg.Height = 100;
                instance.CAAImg.Width = 110;
                instance.btnLoginPopup.Height = 50;
                instance.btnLoginPopup.Width = 150;
                instance.btnLoginPopup.FontSize = 20;
            }

            //phone visual state
            VisualStateChange(mobile, "Rectangle.Height", "header", 70);
            VisualStateChange(mobile, "stackpanel.Height", "relativePanel", 70);
            VisualStateChange(mobile, "textBlock.FontSize", "textBlock", 20);
            VisualStateChange(mobile, "textBlock.Margin", "textBlock", new Thickness(10, 0, 0, 0));
            VisualStateChange(mobile, "Image.Height", "CAAImg", 60);
            VisualStateChange(mobile, "Image.Width", "CAAImg", 70);
            VisualStateChange(mobile, "Button.Height", "btnLoginPopup", 25);
            VisualStateChange(mobile, "Button.Width", "btnLoginPopup", 75);
            VisualStateChange(mobile, "Button.FontSize", "btnLoginPopup", 11);

            //middle visual state
            VisualStateChange(middle, "Rectangle.Height", "header", 80);
            VisualStateChange(middle, "stackpanel.Height", "relativePanel", 80);
            VisualStateChange(middle, "textBlock.FontSize", "textBlock", 30);
            VisualStateChange(middle, "textBlock.Margin", "textBlock", new Thickness(15, 0, 0, 0));
            VisualStateChange(middle, "Image.Height", "CAAImg", 70);
            VisualStateChange(middle, "Image.Width", "CAAImg", 80);
            VisualStateChange(middle, "Button.Height", "btnLoginPopup", 30);
            VisualStateChange(middle, "Button.Width", "btnLoginPopup", 90);
            VisualStateChange(middle, "Button.FontSize", "btnLoginPopup", 14);

            //tablet visual state
            VisualStateChange(tablet, "Rectangle.Height", "header", 90);
            VisualStateChange(tablet, "stackpanel.Height", "relativePanel", 90);
            VisualStateChange(tablet, "textBlock.Margin", "textBlock", new Thickness(20, 0, 0, 0));
            VisualStateChange(tablet, "textBlock.FontSize", "textBlock", 40);
            VisualStateChange(tablet, "Image.Height", "CAAImg", 90);
            VisualStateChange(tablet, "Image.Width", "CAAImg", 100);
            VisualStateChange(tablet, "Button.Height", "btnLoginPopup", 38);
            VisualStateChange(tablet, "Button.Width", "btnLoginPopup", 130);
            VisualStateChange(tablet, "Button.FontSize", "btnLoginPopup", 18);

            //desktop visual state
            VisualStateChange(desktop, "Rectangle.Height", "header", 100);
            VisualStateChange(desktop, "stackpanel.Height", "relativePanel", 100);
            VisualStateChange(desktop, "textBlock.Margin", "textBlock", new Thickness(20, 0, 0, 0));
            VisualStateChange(desktop, "textBlock.FontSize", "textBlock", 45);
            VisualStateChange(desktop, "Image.Height", "CAAImg", 100);
            VisualStateChange(desktop, "Image.Width", "CAAImg", 110);
            VisualStateChange(desktop, "Button.Height", "btnLoginPopup", 50);
            VisualStateChange(desktop, "Button.Width", "btnLoginPopup", 150);
            VisualStateChange(desktop, "Button.FontSize", "btnLoginPopup", 20);
        }

        //change visual state
        public static void VisualStateChange(VisualState visualState, string propertyPath, string elementName, object value)
        {
            visualState.Setters.Add(new Setter
            {
                Target = new TargetPropertyPath
                {
                    Path = new PropertyPath($"({propertyPath})"),
                    Target = instance.FindName(elementName),
                },
                Value = value
            });
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
