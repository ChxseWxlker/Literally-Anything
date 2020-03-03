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
    public sealed partial class PageEventManager : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //setup selected event
        Event selectedEvent;

        //setup list of trackable items
        List<TrackingInfo> trackingInfo = new List<TrackingInfo>();

        public PageEventManager()
        {
            this.InitializeComponent();
            this.Loaded += EventManager_Loaded;
        }
        
        private async void EventManager_Loaded(object sender, RoutedEventArgs e)
        {
            //put focus on member number for easy card swiping
            txtMemberNum.Focus(FocusState.Keyboard);
            
            //set username
            txtAccount.Text = Environment.GetEnvironmentVariable("activeUser");

            //populate elements
            lblEventName.Text = selectedEvent.displayName;

            //get all tracking values
            List<TrackingInfo> ti = await Connection.Get("TrackingInfo");
            if (ti != null)
                foreach (TrackingInfo t in ti)
                    if (t.EventID == selectedEvent.Id && t.hidden == false)
                    {
                        //add to list
                        trackingInfo.Add(t);

                        //create label
                        TextBlock lblTrack = new TextBlock();
                        lblTrack.Name = $"lblTrack_{trackingPanel.Children.Count + 1}";
                        lblTrack.Text = t.item;
                        lblTrack.Margin = new Thickness(0, 20, 0, 0);
                        lblTrack.TextWrapping = TextWrapping.Wrap;
                        lblTrack.FontSize = 25;

                        //create textbox
                        TextBox txtTrack = new TextBox();
                        txtTrack.Name = $"txtTrack_{trackingPanel.Children.Count + 1}";
                        txtTrack.PlaceholderText = "0";
                        txtTrack.HorizontalAlignment = HorizontalAlignment.Left;
                        txtTrack.Margin = new Thickness(0, 5, 0, 0);
                        txtTrack.TextWrapping = TextWrapping.Wrap;
                        txtTrack.Height = 40;
                        txtTrack.Width = 100;
                        txtTrack.FontSize = 22;

                        //create button
                        Button btnTrack = new Button();
                        btnTrack.Name = $"btnTrack_{trackingPanel.Children.Count + 1}";
                        btnTrack.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        btnTrack.Content = "\uE710";
                        btnTrack.Margin = new Thickness(105, -40, 0, 0);
                        btnTrack.Height = 40;
                        btnTrack.Width = 40;

                        //add items to panel
                        trackingPanel.Children.Add(lblTrack);
                        trackingPanel.Children.Add(txtTrack);
                        trackingPanel.Children.Add(btnTrack);
                    }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get selected event from previous page
            selectedEvent = (Event)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private async void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            //prompt user
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout?",
                Content = "You will be redirected to the home page and locked out until you log back in. Are you sure you want to logout?",
                PrimaryButtonText = "Logout",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult logoutRes = await logoutDialog.ShowAsync();

            //log user out
            if (logoutRes == ContentDialogResult.Primary)
            {
                //reset active username
                Environment.SetEnvironmentVariable("activeUser", "");

                //update menu
                txtAccount.Text = "";

                //logout
                api.Logout();

                //redirect to index
                Frame.Navigate(typeof(PageIndex));
            }

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private void btnShowPane_Click(object sender, RoutedEventArgs e)
        {
            svMenu.IsPaneOpen = !svMenu.IsPaneOpen;
            if (svMenu.IsPaneOpen)
            {
                btnShowPane.Content = "\uE00E";
                btnEventMenu.Visibility = Visibility.Visible;
                btnGameMenu.Visibility = Visibility.Visible;
                btnQuestionMenu.Visibility = Visibility.Visible;
            }
            else
            {
                btnShowPane.Content = "\uE00F";
                btnEventMenu.Visibility = Visibility.Collapsed;
                btnGameMenu.Visibility = Visibility.Collapsed;
                btnQuestionMenu.Visibility = Visibility.Collapsed;

                //focus membership
                txtMemberNum.Focus(FocusState.Keyboard);
            }
        }

        private void svMenu_PaneClosing(SplitView sender, object args)
        {
            //hide buttons
            btnShowPane.Content = "\uE00F";
            btnEventMenu.Visibility = Visibility.Collapsed;
            btnGameMenu.Visibility = Visibility.Collapsed;
            btnQuestionMenu.Visibility = Visibility.Collapsed;
        }

        private void btnMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //get menu button
            Button btn = (Button)sender;

            //event
            if (btn.Content.ToString().Contains("Event"))
                Frame.Navigate(typeof(PageEvent));

            //game
            else if (btn.Content.ToString().Contains("Game"))
                Frame.Navigate(typeof(PageGame));

            //question
            else if (btn.Content.ToString().Contains("Question"))
                Frame.Navigate(typeof(PageQuestion));
        }

        private static bool Luhn(string digits)
        {
            return digits.All(char.IsDigit) && digits.Reverse()
                .Select(c => c - 48)
                .Select((thisNum, i) => i % 2 == 0
                    ? thisNum
                    : ((thisNum *= 2) > 9 ? thisNum - 9 : thisNum)
                ).Sum() % 10 == 0;
        }

        private async void txtMemberNum_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //create attendance
            Attendance a = new Attendance();
            
            //get textbox
            TextBox txtBox = (TextBox)sender;

            //finished entering
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                //card swipe entry
                if (txtBox.Text.Substring(0, 2) == "%B")
                {
                    //disable other inputs
                    txtMemberFirst.IsEnabled = false;
                    txtMemberLast.IsEnabled = false;
                    txtMemberPhone.IsEnabled = false;

                    //setup attendance object and set properties
                    a.memberNumber = txtBox.Text.Substring(2, 16);
                    char[] lastName = txtBox.Text.Substring(19, txtBox.Text.IndexOf("/") - 19).ToLower().ToCharArray();
                    lastName[0] = char.ToUpper(lastName[0]);
                    a.lastName = new string(lastName);
                    char[] firstName = txtBox.Text.Substring(txtBox.Text.IndexOf("/") + 1, txtBox.Text.IndexOf(" ") - txtBox.Text.IndexOf("/") + 1)
                        .ToLower().Trim().Replace(".", "").ToCharArray();
                    firstName[0] = char.ToUpper(firstName[0]);
                    a.firstName = new string(firstName);
                    a.arriveTime = DateTime.Now;
                    a.isMember = true;
                    a.EventID = selectedEvent.Id;

                    //verify card number
                    if (!Luhn(a.memberNumber))
                    {
                        await new MessageDialog("Invalid card number, please try again").ShowAsync();

                        //re-enable other inputs
                        txtMemberFirst.IsEnabled = true;
                        txtMemberLast.IsEnabled = true;
                        txtMemberPhone.IsEnabled = true;

                        return;
                    }

                    a.Id = await Connection.Insert(a);
                }

                //manual entry
                else
                {
                    //setup attendance object and set properties
                    a.memberNumber = txtMemberNum.Text;
                    if (txtMemberLast.Text != "")
                    {
                        char[] lastName = txtMemberLast.Text.ToLower().Trim().ToCharArray();
                        lastName[0] = char.ToUpper(lastName[0]);
                        a.lastName = new string(lastName);
                    }

                    //replace first name with null
                    else
                        txtMemberFirst.Text = null;

                    if (txtMemberFirst.Text != "")
                    {
                        char[] firstName = txtMemberFirst.Text.ToLower().Trim().ToCharArray();
                        firstName[0] = char.ToUpper(firstName[0]);
                        a.firstName = new string(firstName);
                    }

                    //replace last name with null
                    else
                        txtMemberLast.Text = null;

                    a.arriveTime = DateTime.Now;
                    a.phone = txtMemberPhone.Text.Replace("", null);
                    a.EventID = selectedEvent.Id;

                    //verify card number
                    if (!Luhn(a.memberNumber))
                    {
                        await new MessageDialog("Invalid card number, please try again").ShowAsync();

                        //focus membership
                        txtMemberNum.Focus(FocusState.Keyboard);

                        return;
                    }

                    a.Id = await Connection.Insert(a);
                }

                //re-enable other inputs
                txtMemberFirst.IsEnabled = true;
                txtMemberLast.IsEnabled = true;
                txtMemberPhone.IsEnabled = true;

                //reset fields
                txtMemberNum.Text = "";
                txtMemberFirst.Text = "";
                txtMemberLast.Text = "";
                txtMemberPhone.Text = "";
                txtMemberNum.Focus(FocusState.Keyboard);
            }
        }

        private async void btnMemberSubmit_Click(object sender, RoutedEventArgs e)
        {
            //setup attendance object and set properties
            Attendance a = new Attendance();
            a.memberNumber = txtMemberNum.Text.Replace("", null); ;
            if (txtMemberLast.Text != "")
            {
                char[] lastName = txtMemberLast.Text.ToLower().Trim().ToCharArray();
                lastName[0] = char.ToUpper(lastName[0]);
                a.lastName = new string(lastName);
            }

            //replace first name with null
            else
                txtMemberFirst.Text = null;

            if (txtMemberFirst.Text != "")
            {
                char[] firstName = txtMemberFirst.Text.ToLower().Trim().ToCharArray();
                firstName[0] = char.ToUpper(firstName[0]);
                a.firstName = new string(firstName);
            }

            //replace last name with null
            else
                txtMemberLast.Text = null;

            a.arriveTime = DateTime.Now;
            a.phone = txtMemberPhone.Text.Replace("", null);
            a.EventID = selectedEvent.Id;

            //verify card number
            if (!Luhn(a.memberNumber))
            {
                await new MessageDialog("Invalid card number, please try again").ShowAsync();
                //reset fields
                txtMemberNum.Text = "";
                txtMemberFirst.Text = "";
                txtMemberLast.Text = "";
                txtMemberPhone.Text = "";
                txtMemberNum.Focus(FocusState.Keyboard);
                return;
            }

            a.Id = await Connection.Insert(a);

            //reset fields
            txtMemberNum.Text = "";
            txtMemberFirst.Text = "";
            txtMemberLast.Text = "";
            txtMemberPhone.Text = "";
            txtMemberNum.Focus(FocusState.Keyboard);

            //update recent member
        }

        private void svMenu_PaneClosed(SplitView sender, object args)
        {
            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }
    }
}
