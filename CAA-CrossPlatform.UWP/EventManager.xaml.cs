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
    public sealed partial class EventManager : Page
    {
        //setup api
        static ApiHandler api = new ApiHandler();

        //setup selected event
        Event selectedEvent;

        public EventManager()
        {
            this.InitializeComponent();
            this.Loaded += EventManager_Loaded;
        }
        
        private void EventManager_Loaded(object sender, RoutedEventArgs e)
        {
            //put focus on member number for easy card swiping
            txtMemberNum.Focus(FocusState.Keyboard);

            //populate elements
            //tbEventName.Text = selectedEvent.displayName;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //get selected event from previous page
            selectedEvent = new Event(); //(Event)e.Parameter;
            base.OnNavigatedTo(e);
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            //do login stuff
            if (btnAccount.Content.ToString() == "Login")
            {
                //reset values
                txtUsername.Text = "";
                txtPassword.Password = "";

                //show popup
                popupLogin.IsOpen = true;

                //focus username
                txtUsername.Focus(FocusState.Keyboard);
            }

            //do logout stuff
            else
            {
                //set login button
                btnAccount.Content = "Login";

                //reset active username
                Environment.SetEnvironmentVariable("activeUser", "");

                //update menu
                txtAccount.Text = "";

                //logout
                api.Logout();

                //focus membership
                txtMemberNum.Focus(FocusState.Keyboard);
            }
        }

        private void txtPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnLogin_Click(sender, e);
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //login
            string res = await api.Login(txtUsername.Text, txtPassword.Password);

            //show error message
            if (!res.Contains("Welcome"))
            {
                await new MessageDialog(res).ShowAsync();
                return;
            }

            //set logout button
            btnAccount.Content = "Logout";

            //set active username
            Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

            //update menu
            txtAccount.Text = $"Welcome {txtUsername.Text}";

            //close popup
            popupLogin.IsOpen = false;

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            //register
            string res = await api.Register(txtUsername.Text, txtPassword.Password);

            //show error message
            if (!res.Contains("Welcome"))
            {
                await new MessageDialog(res).ShowAsync();
                return;
            }

            //set logout button
            btnAccount.Content = "Logout";

            //set active username
            Environment.SetEnvironmentVariable("activeUser", txtUsername.Text);

            //close popup
            popupLogin.IsOpen = false;

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private void btnLoginCancel_Click(object sender, RoutedEventArgs e)
        {
            //close popup
            popupLogin.IsOpen = false;

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
                    if (txtMemberFirst.Text != "")
                    {
                        char[] firstName = txtMemberFirst.Text.ToLower().Trim().ToCharArray();
                        firstName[0] = char.ToUpper(firstName[0]);
                        a.firstName = new string(firstName);
                    }
                    a.arriveTime = DateTime.Now;
                    a.phone = txtMemberPhone.Text;
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
                }

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
            a.memberNumber = txtMemberNum.Text;
            if (txtMemberLast.Text != "")
            {
                char[] lastName = txtMemberLast.Text.ToLower().Trim().ToCharArray();
                lastName[0] = char.ToUpper(lastName[0]);
                a.lastName = new string(lastName);
            }
            if (txtMemberFirst.Text != "")
            {
                char[] firstName = txtMemberFirst.Text.ToLower().Trim().ToCharArray();
                firstName[0] = char.ToUpper(firstName[0]);
                a.firstName = new string(firstName);
            }
            a.arriveTime = DateTime.Now;
            a.phone = txtMemberPhone.Text;
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
    }
}
