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

        //list of event items
        List<EventItem> eventItems;

        public PageEventManager()
        {
            this.InitializeComponent();
            this.Loaded += EventManager_Loaded;
        }
        
        private async void EventManager_Loaded(object sender, RoutedEventArgs e)
        {
            //put focus on member number for easy card swiping
            txtMemberNum.Focus(FocusState.Keyboard);
            
            ////set username
            //txtAccount.Text = Environment.GetEnvironmentVariable("activeUser");

            //populate elements
            lblEventName.Text = selectedEvent.displayName;
            
            //get all tracking values
            eventItems = await Connection.Get("EventItem");
            if (eventItems != null)
                foreach (EventItem eventItem in eventItems)
                    if (eventItem.EventId == selectedEvent.Id)
                    {
                        //add to list
                        Item item = await Connection.Get("Item", eventItem.ItemId);

                        //create stackpanel
                        StackPanel spTrack = new StackPanel();
                        spTrack.Orientation = Orientation.Vertical;

                        //create label
                        TextBlock lblTrack = new TextBlock();
                        lblTrack.Text = item.name;
                        lblTrack.Margin = new Thickness(0, 20, 0, 0);
                        lblTrack.TextWrapping = TextWrapping.Wrap;
                        lblTrack.FontSize = 25;

                        StackPanel spControls = new StackPanel();
                        spTrack.Orientation = Orientation.Horizontal;

                        //create button
                        Button btnMinus = new Button();
                        btnMinus.Name = $"btnMinus_{trackingPanel.Children.Count + 1}";
                        btnMinus.Click += BtnControl_Click;
                        btnMinus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        btnMinus.Content = "\uE738";
                        btnMinus.Margin = new Thickness(105, -40, 0, 0);
                        btnMinus.Height = 40;
                        btnMinus.Width = 40;

                        //create textbox
                        TextBox txtTrack = new TextBox();
                        txtTrack.Name = $"txtTrack_{trackingPanel.Children.Count + 1}";
                        txtTrack.Text = "0";
                        txtTrack.HorizontalAlignment = HorizontalAlignment.Left;
                        txtTrack.Margin = new Thickness(0, 5, 0, 0);
                        txtTrack.TextWrapping = TextWrapping.Wrap;
                        txtTrack.Height = 40;
                        txtTrack.Width = 100;
                        txtTrack.FontSize = 22;

                        //create button
                        Button btnPlus = new Button();
                        btnPlus.Name = $"btnPlus_{trackingPanel.Children.Count + 1}";
                        btnPlus.Click += BtnControl_Click;
                        btnPlus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        btnPlus.Content = "\uE710";
                        btnPlus.Margin = new Thickness(105, -40, 0, 0);
                        btnPlus.Height = 40;
                        btnPlus.Width = 40;

                        //add items to panel
                        spTrack.Children.Add(lblTrack);
                        spTrack.Children.Add(btnMinus);
                        spTrack.Children.Add(txtTrack);
                        spTrack.Children.Add(btnPlus);
                        trackingPanel.Children.Add(spTrack);
                    }
        }

        private void BtnControl_Click(object sender, RoutedEventArgs e)
        {
            Button btnCaller = (Button)sender;
            string btn = btnCaller.Name.ToString();
            int Id = Convert.ToInt32(btn.Substring(btn.IndexOf('_') + 1));
            TextBox txt = null;
            foreach (StackPanel sp in trackingPanel.Children)
            {
                TextBox spTxt = (TextBox)sp.Children[2];
                if (Convert.ToInt32(spTxt.Name.Substring(spTxt.Name.IndexOf('_') + 1)) == Id)
                    txt = spTxt;
            }

            //plus button
            if (btn.Contains("Plus"))
                txt.Text = (Convert.ToInt32(txt.Text) + 1).ToString();

            //minus button
            else if (btn.Contains("Minus"))
                if (Convert.ToInt32(txt.Text) > 0)
                    txt.Text = (Convert.ToInt32(txt.Text) - 1).ToString();

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
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

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
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
                    a.arriveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
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
                    
                    //create items
                    foreach (StackPanel sp in trackingPanel.Children)
                    {
                        TextBox txt = (TextBox)sp.Children[2];
                        int value = Convert.ToInt32(txt.Text);

                        //create item
                        if (value > 0)
                        {
                            AttendanceItem attendanceItem = new AttendanceItem();
                            attendanceItem.AttendanceId = a.Id;
                            attendanceItem.EventItemId = eventItems[trackingPanel.Children.IndexOf(sp)].Id;
                            attendanceItem.input = value;
                            attendanceItem.Id = await Connection.Insert(attendanceItem);
                        }
                    }
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

                    a.arriveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
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

                    //create items
                    foreach (StackPanel sp in trackingPanel.Children)
                    {
                        TextBox txt = (TextBox)sp.Children[2];
                        int value = Convert.ToInt32(txt.Text);

                        //create item
                        if (value > 0)
                        {
                            AttendanceItem attendanceItem = new AttendanceItem();
                            attendanceItem.AttendanceId = a.Id;
                            attendanceItem.EventItemId = eventItems[trackingPanel.Children.IndexOf(sp)].Id;
                            attendanceItem.input = value;
                            attendanceItem.Id = await Connection.Insert(attendanceItem);
                        }
                    }
                }

                //re-enable other inputs
                txtMemberFirst.IsEnabled = true;
                txtMemberLast.IsEnabled = true;
                txtMemberPhone.IsEnabled = true;

                //reset fields
                foreach (StackPanel sp in trackingPanel.Children)
                {
                    TextBox txt = (TextBox)sp.Children[2];
                    txt.Text = "0";
                }
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

            a.arriveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            a.phone = txtMemberPhone.Text.Replace("", null);
            a.EventID = selectedEvent.Id;

            //verify card number
            if (!Luhn(a.memberNumber))
            {
                await new MessageDialog("Invalid card number, please try again").ShowAsync();

                txtMemberNum.Focus(FocusState.Keyboard);
                return;
            }

            a.Id = await Connection.Insert(a);

            //create items
            foreach (StackPanel sp in trackingPanel.Children)
            {
                TextBox txt = (TextBox)sp.Children[2];
                int value = Convert.ToInt32(txt.Text);

                //create item
                if (value > 0)
                {
                    AttendanceItem attendanceItem = new AttendanceItem();
                    attendanceItem.AttendanceId = a.Id;
                    attendanceItem.EventItemId = eventItems[trackingPanel.Children.IndexOf(sp)].Id;
                    attendanceItem.input = value;
                    attendanceItem.Id = await Connection.Insert(attendanceItem);
                }
            }

            //reset fields
            foreach (StackPanel sp in trackingPanel.Children)
            {
                TextBox txt = (TextBox)sp.Children[2];
                txt.Text = "0";
            }
            txtMemberNum.Text = "";
            txtMemberFirst.Text = "";
            txtMemberLast.Text = "";
            txtMemberPhone.Text = "";
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void TxtSearch_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
