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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CAA_CrossPlatform.UWP
{
    public sealed partial class PageEventManager : Page
    {
        //setup selected event
        Event selectedEvent = new Event();

        //list of event items
        List<EventItem> eventItems = new List<EventItem>();

        //setup total member count
        int memberCount = 0;

        //setup all attendance
        List<Attendance> attendanceHistory = new List<Attendance>();

        //setup game stuff
        Game game = new Game();
        List<Question> questions = new List<Question>();
        List<Answer> answers = new List<Answer>();

        //setup auto play
        static bool gamePlayAuto = true;

        public PageEventManager()
        {
            this.InitializeComponent();
            this.Loaded += EventManager_Loaded;
        }
        
        private async void EventManager_Loaded(object sender, RoutedEventArgs e)
        {
            //get event
            selectedEvent = EnvironmentModel.Event;

            //put focus on member number for easy card swiping
            txtMemberNum.Focus(FocusState.Keyboard);

            //get attendance history
            List<Attendance> attendanceList = await Connection.Get("Attendance");
            foreach (Attendance attendance in attendanceList)
                if (attendance.EventID == selectedEvent.Id)
                    AddHistory(attendance);

            //get game and questions
            game = await Connection.Get("Game", selectedEvent.GameID);
            List<GameQuestion> gameQuestions = await Connection.Get("GameQuestion");
            List<Answer> gameAnswers = await Connection.Get("Answer");
            foreach (GameQuestion gameQuestion in gameQuestions)
                if (gameQuestion.GameID == game.Id)
                {
                    Question question = await Connection.Get("Question", gameQuestion.QuestionID);
                    if (question.hidden == false)
                    {
                        questions.Add(question);
                        foreach (Answer answer in gameAnswers)
                            if (answer.QuestionID == question.Id && question.hidden == false)
                                answers.Add(answer);
                    }
                }

            //set auto play
            chkPlayGameAuto.IsChecked = gamePlayAuto;

            //populate elements
            lblEventName.Text = selectedEvent.displayName;
            
            //get all tracking values
            List<EventItem> tempEventItems = await Connection.Get("EventItem");
            if (eventItems != null)
                foreach (EventItem eventItem in tempEventItems)
                    if (eventItem.EventId == selectedEvent.Id)
                    {
                        //get item
                        Item item = await Connection.Get("Item", eventItem.ItemId);

                        //check if deleted
                        if (item.hidden)
                            return;

                        //add to list
                        eventItems.Add(eventItem);

                        //create stackpanel for item
                        StackPanel spTrack = new StackPanel();
                        spTrack.Orientation = Orientation.Vertical;

                        //create label
                        TextBlock lblTrack = new TextBlock();
                        lblTrack.Text = item.name;
                        lblTrack.Margin = new Thickness(0, 20, 0, 0);
                        lblTrack.TextWrapping = TextWrapping.Wrap;
                        lblTrack.FontSize = 25;
                        lblTrack.HorizontalAlignment = HorizontalAlignment.Center;

                        //crate stackpanel for controls
                        StackPanel spControls = new StackPanel();
                        spControls.Orientation = Orientation.Horizontal;

                        //create button
                        Button btnMinus = new Button();
                        btnMinus.Name = $"btnMinus_{trackingPanel.Children.Count + 1}";
                        btnMinus.Click += BtnControl_Click;
                        btnMinus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        btnMinus.Content = "\uE738";
                        btnMinus.Margin = new Thickness(0, 0, 5, 0);
                        btnMinus.Style = (Style)Application.Current.Resources["ButtonTemplate"];
                        btnMinus.Height = 40;
                        btnMinus.Width = 40;

                        //create textbox
                        TextBox txtTrack = new TextBox();
                        txtTrack.Name = $"txtTrack_{trackingPanel.Children.Count + 1}";

                        if (item.valueType == "int")
                        {
                            txtTrack.Text = "0";
                            txtTrack.KeyTipHorizontalOffset = 1;
                        }

                        else if (item.valueType == "string")
                        {
                            txtTrack.Text = "";
                            txtTrack.KeyTipHorizontalOffset = 0;
                        }

                        txtTrack.HorizontalAlignment = HorizontalAlignment.Left;
                        txtTrack.TextWrapping = TextWrapping.Wrap;
                        txtTrack.Margin = new Thickness(0, 5, 0, 0);
                        txtTrack.Height = 40;
                        txtTrack.Width = 100;
                        txtTrack.FontSize = 22;

                        //create button
                        Button btnPlus = new Button();
                        btnPlus.Name = $"btnPlus_{trackingPanel.Children.Count + 1}";
                        btnPlus.Click += BtnControl_Click;
                        btnPlus.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        btnPlus.Margin = new Thickness(5, 0, 0, 0);
                        btnPlus.Style = (Style)Application.Current.Resources["ButtonTemplate"];
                        btnPlus.Content = "\uE710";
                        btnPlus.Height = 40;
                        btnPlus.Width = 40;

                        if (item.valueType == "string")
                        {
                            btnMinus.Visibility = Visibility.Collapsed;
                            btnPlus.Visibility = Visibility.Collapsed;
                            txtTrack.Width = 190;
                        }

                        //add items to panel
                        spTrack.Children.Add(lblTrack);
                        spControls.Children.Add(btnMinus);
                        spControls.Children.Add(txtTrack);
                        spControls.Children.Add(btnPlus);
                        spTrack.Children.Add(spControls);
                        trackingPanel.Children.Add(spTrack);
                    }

            //hide tracking if empty
            if (trackingPanel.Children.Count == 1)
                trackingPanel.Visibility = Visibility.Collapsed;
        }

        private void btnPlayGame_Click(object sender, RoutedEventArgs e)
        {
            //setup environment vars
            EnvironmentModel.Event = selectedEvent;
            EnvironmentModel.Game = game;
            EnvironmentModel.QuestionList = questions;
            EnvironmentModel.AnswerList = answers;

            //open game player
            Frame.Navigate(typeof(PageGamePlayer));
        }

        private void chkPlayGameAuto_Click(object sender, RoutedEventArgs e)
        {
            //set public auto var
            gamePlayAuto = chkPlayGameAuto.IsChecked.GetValueOrDefault();

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
        }

        private void BtnControl_Click(object sender, RoutedEventArgs e)
        {
            Button btnCaller = (Button)sender;
            string btn = btnCaller.Name.ToString();
            string Id = btn.Substring(btn.IndexOf('_') + 1);
            TextBox txtBox = null;

            foreach (var element in trackingPanel.Children)
            {
                if (element.GetType() == typeof(StackPanel))
                {
                    StackPanel spTrack = (StackPanel)element;
                    StackPanel spControls = (StackPanel)spTrack.Children[1];
                    TextBox txtTrack = (TextBox)spControls.Children[1];
                    if (txtTrack.Name.Substring(txtTrack.Name.IndexOf('_') + 1) == Id)
                        txtBox = txtTrack;
                }
            }

            //plus button
            if (btn.Contains("Plus"))
                txtBox.Text = (Convert.ToInt32(txtBox.Text) + 1).ToString();

            //minus button
            else if (btn.Contains("Minus"))
                if (Convert.ToInt32(txtBox.Text) > 0)
                    txtBox.Text = (Convert.ToInt32(txtBox.Text) - 1).ToString();

            //focus membership
            txtMemberNum.Focus(FocusState.Keyboard);
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

        private void AddHistory(Attendance attendance)
        {
            //increment total members
            memberCount++;
            lblMemberCount.Text = $"Total Guests: {memberCount}";

            //add to history
            attendanceHistory.Add(attendance);

            //get item count
            int historyItemCount = spHistoryMemNumber.Children.Count;

            //remove last if full
            if (historyItemCount == 4)
            {
                spHistoryMemNumber.Children.RemoveAt(3);
                spHistoryMemName.Children.RemoveAt(3);
                spHistoryMemTime.Children.RemoveAt(3);
                historyItemCount = 3;
            }

            //create items
            TextBlock lblNumber = new TextBlock();
            lblNumber.Text = attendance.memberNumber;
            lblNumber.Padding = new Thickness(5);
            lblNumber.SetValue(Grid.RowProperty, historyItemCount);

            TextBlock lblName = new TextBlock();
            lblName.Text = $"{attendance.firstName} {attendance.lastName}";
            lblName.Padding = new Thickness(5);
            lblName.SetValue(Grid.RowProperty, historyItemCount);

            TextBlock lblTime = new TextBlock();
            lblTime.Text = attendance.arriveTime.ToString("MMMM dd, hh:mm:ss");
            lblTime.Padding = new Thickness(5);
            lblTime.SetValue(Grid.RowProperty, historyItemCount);

            //append items
            spHistoryMemNumber.Children.Insert(1, lblNumber);
            spHistoryMemName.Children.Insert(1, lblName);
            spHistoryMemTime.Children.Insert(1, lblTime);
        }

        private Attendance SwipeMember(string card)
        {
            try
            {
                //create attendance
                Attendance a = new Attendance();

                //remove other text
                string memberSwipe = card.Substring(card.IndexOf("%B"));

                //disable other inputs
                txtMemberFirst.IsEnabled = false;
                txtMemberLast.IsEnabled = false;
                txtMemberPhone.IsEnabled = false;

                //setup attendance object and set properties
                a.memberNumber = memberSwipe.Substring(2, 16);
                char[] lastName = memberSwipe.Substring(19, memberSwipe.IndexOf("/") - 19).ToLower().ToCharArray();
                lastName[0] = char.ToUpper(lastName[0]);
                a.lastName = new string(lastName);
                char[] firstName = memberSwipe.Substring(memberSwipe.IndexOf("/") + 1, memberSwipe.IndexOf(" ") - memberSwipe.IndexOf("/") + 1)
                    .ToLower().Trim().Replace(".", "").ToCharArray();
                firstName[0] = char.ToUpper(firstName[0]);
                a.firstName = new string(firstName);
                a.arriveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                a.isMember = true;
                a.EventID = selectedEvent.Id;

                //verify card number
                if (!Luhn(a.memberNumber))
                {
                    PageIndex.ShowError("Invalid card number, try again.");

                    //re-enable other inputs
                    txtMemberFirst.IsEnabled = true;
                    txtMemberLast.IsEnabled = true;
                    txtMemberPhone.IsEnabled = true;

                    return new Attendance() { Id = -1 };
                }

                return a;
            }

            //catch swipe error
            catch
            {
                PageIndex.ShowError("Error swiping card, try again.");

                return new Attendance() { Id = -1 };
            }
        }

        private Attendance EnterMember(string memberNumber, string memberFirst, string memberLast, string memberPhone)
        {
            //setup attendance object and set properties
            Attendance a = new Attendance();
            a.memberNumber = memberNumber.Replace(" ", "");
            string first = memberFirst.Replace(" ", "");
            string last = memberLast.Replace(" ", "");
            string phone = memberPhone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            //verify card number
            if (!Luhn(a.memberNumber))
            {
                PageIndex.ShowError("Invalid card number, try again.");

                //focus membership
                txtMemberNum.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            //verify first name
            if (string.IsNullOrEmpty(first))
            {
                PageIndex.ShowError("First name is required.");

                //focus first name
                txtMemberFirst.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            //verify first name alphabetical
            else if (!Regex.IsMatch(first, @"^[a-zA-Z]+$"))
            {
                PageIndex.ShowError("First name must be alphanumeric.");

                //focus first name
                txtMemberFirst.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            //get first name
            else if (!string.IsNullOrEmpty(first) && Regex.IsMatch(first, @"^[a-zA-Z]+$"))
            {
                char[] firstName = first.ToLower().ToCharArray();
                firstName[0] = char.ToUpper(firstName[0]);
                a.firstName = new string(firstName);
            }

            //verify last name
            if (string.IsNullOrEmpty(last))
            {
                PageIndex.ShowError("Last name is required.");

                //focus last name
                txtMemberLast.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            //verify last name alphabetical
            else if (!Regex.IsMatch(last, @"^[a-zA-Z]+$"))
            {
                PageIndex.ShowError("Last name must be alphanumeric.");

                //focus last name
                txtMemberLast.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            //get last name
            else if (!string.IsNullOrEmpty(last) && Regex.IsMatch(last, @"^[a-zA-Z]+$"))
            {
                char[] lastName = last.ToLower().ToCharArray();
                lastName[0] = char.ToUpper(lastName[0]);
                a.lastName = new string(lastName);
            }

            //verify phone numeric
            if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^[0-9]+$"))
            {
                PageIndex.ShowError("Phone number must be numeric.");

                //focus last name
                txtMemberPhone.Focus(FocusState.Keyboard);

                return new Attendance() { Id = -1 };
            }

            a.arriveTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            a.phone = phone;
            a.EventID = selectedEvent.Id;
            return a;
        }

        //this is to check if the user presses enter or if the enter is from the card swiper
        int swipeEnter = 0;
        
        private async void txtMemberNum_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //create attendance
            Attendance a = new Attendance();
            
            //get textbox
            TextBox txtBox = (TextBox)sender;

            //add if no tracking
            if (e.Key == Windows.System.VirtualKey.Enter && trackingPanel.Children.Count == 1)
            {
                //card swipe entry
                if (txtBox.Text.Contains("%B"))
                {
                    a = SwipeMember(txtBox.Text);
                    if (a.Id == -1)
                        return;
                }

                //manual entry
                else
                {
                    a = EnterMember(txtMemberNum.Text, txtMemberFirst.Text, txtMemberLast.Text, txtMemberPhone.Text);
                    if (a.Id == -1)
                        return;
                }

                a.Id = await Connection.Insert(a);
            }

            //add with tracking
            else if (e.Key == Windows.System.VirtualKey.Enter && trackingPanel.Children.Count > 1)
            {
                //card swipe entry
                if (txtBox.Text.Contains("%B"))
                {
                    //check if user presses enter
                    if (swipeEnter == 0)
                    {
                        swipeEnter = 1;
                        return;
                    }
                    
                    //return if card swiper isn't finished
                    if (!txtBox.Text.Contains("?"))
                    {
                        swipeEnter = 0;
                        return;
                    }

                    //reset swipe enter
                    else if (swipeEnter == 1)
                        swipeEnter = 0;

                    a = SwipeMember(txtBox.Text);
                    if (a.Id == -1)
                        return;
                }

                //manual entry
                else
                {
                    a = EnterMember(txtMemberNum.Text, txtMemberFirst.Text, txtMemberLast.Text, txtMemberPhone.Text);
                    if (a.Id == -1)
                        return;
                }

                a.Id = await Connection.Insert(a);

                //create items
                foreach (var element in trackingPanel.Children)
                {
                    if (element.GetType() == typeof(StackPanel))
                    {
                        StackPanel spTrack = (StackPanel)element;
                        StackPanel spControls = (StackPanel)spTrack.Children[1];
                        TextBox txtTrack = (TextBox)spControls.Children[1];

                        //create item
                        if (txtTrack.Text != "0" && txtTrack.Text != "")
                        {
                            AttendanceItem attendanceItem = new AttendanceItem();
                            attendanceItem.AttendanceId = a.Id;
                            attendanceItem.EventItemId = eventItems[trackingPanel.Children.IndexOf(spTrack) - 1].Id;
                            attendanceItem.input = txtTrack.Text;
                            attendanceItem.Id = await Connection.Insert(attendanceItem);
                        }
                    }
                }
            }

            //do nothing
            else
                return;

            //add history
            AddHistory(a);

            //re-enable other inputs
            txtMemberFirst.IsEnabled = true;
            txtMemberLast.IsEnabled = true;
            txtMemberPhone.IsEnabled = true;

            //reset fields
            foreach (var element in trackingPanel.Children)
            {
                if (element.GetType() == typeof(StackPanel))
                {
                    StackPanel spTrack = (StackPanel)element;
                    StackPanel spControls = (StackPanel)spTrack.Children[1];
                    TextBox txtTrack = (TextBox)spControls.Children[1];
                    if (txtTrack.KeyTipHorizontalOffset == 1)
                        txtTrack.Text = "0";

                    else if (txtTrack.KeyTipHorizontalOffset == 0)
                        txtTrack.Text = "";
                }
            }
            txtMemberNum.Text = "";
            txtMemberFirst.Text = "";
            txtMemberLast.Text = "";
            txtMemberPhone.Text = "";
            txtMemberNum.Focus(FocusState.Keyboard);

            //play game after swipe if desired
            if (chkPlayGameAuto.IsChecked.GetValueOrDefault())
                btnPlayGame_Click(sender, e);
        }

        private async void btnMemberSubmit_Click(object sender, RoutedEventArgs e)
        {
            //create attendance
            Attendance a = new Attendance();

            //add if no tracking
            if (trackingPanel.Children.Count == 1)
            {
                //card swipe entry
                if (txtMemberNum.Text.Contains("%B"))
                {
                    a = SwipeMember(txtMemberNum.Text);
                    if (a.Id == -1)
                        return;
                }

                //manual entry
                else
                {
                    a = EnterMember(txtMemberNum.Text, txtMemberFirst.Text, txtMemberLast.Text, txtMemberPhone.Text);
                    if (a.Id == -1)
                        return;
                }

                a.Id = await Connection.Insert(a);
            }

            //add with tracking
            else if (trackingPanel.Children.Count > 1)
            {
                //card swipe entry
                if (txtMemberNum.Text.Contains("%B"))
                {
                    //return if card swiper isn't finished
                    if (!txtMemberNum.Text.Contains("?"))
                    {
                        swipeEnter = 0;
                        return;
                    }

                    a = SwipeMember(txtMemberNum.Text);
                    if (a.Id == -1)
                        return;
                }

                //manual entry
                else
                {
                    a = EnterMember(txtMemberNum.Text, txtMemberFirst.Text, txtMemberLast.Text, txtMemberPhone.Text);
                    if (a.Id == -1)
                        return;
                }

                a.Id = await Connection.Insert(a);

                //create items
                foreach (var element in trackingPanel.Children)
                {
                    if (element.GetType() == typeof(StackPanel))
                    {
                        StackPanel spTrack = (StackPanel)element;
                        StackPanel spControls = (StackPanel)spTrack.Children[1];
                        TextBox txtTrack = (TextBox)spControls.Children[1];

                        //create item
                        if (txtTrack.Text != "0" && txtTrack.Text != "")
                        {
                            AttendanceItem attendanceItem = new AttendanceItem();
                            attendanceItem.AttendanceId = a.Id;
                            attendanceItem.EventItemId = eventItems[trackingPanel.Children.IndexOf(spTrack) - 1].Id;
                            attendanceItem.input = txtTrack.Text;
                            attendanceItem.Id = await Connection.Insert(attendanceItem);
                        }
                    }
                }
            }

            //do nothing
            else
                return;

            //add history
            AddHistory(a);

            //re-enable other inputs
            txtMemberFirst.IsEnabled = true;
            txtMemberLast.IsEnabled = true;
            txtMemberPhone.IsEnabled = true;

            //reset fields
            foreach (var element in trackingPanel.Children)
            {
                if (element.GetType() == typeof(StackPanel))
                {
                    StackPanel spTrack = (StackPanel)element;
                    StackPanel spControls = (StackPanel)spTrack.Children[1];
                    TextBox txtTrack = (TextBox)spControls.Children[1];

                    if (txtTrack.KeyTipHorizontalOffset == 1)
                        txtTrack.Text = "0";

                    else if (txtTrack.KeyTipHorizontalOffset == 0)
                        txtTrack.Text = "";
                }
            }
            txtMemberNum.Text = "";
            txtMemberFirst.Text = "";
            txtMemberLast.Text = "";
            txtMemberPhone.Text = "";
            txtMemberNum.Focus(FocusState.Keyboard);

            //play game after swipe if desired
            if (chkPlayGameAuto.IsChecked.GetValueOrDefault())
                btnPlayGame_Click(sender, e);
        }

        private void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                btnSearch_Click(sender, new RoutedEventArgs());
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string search = txtSearch.Text.ToLower().Replace(" ", "");
            SolidColorBrush btnBg = (SolidColorBrush)btnSearch.Background;

            //change to clear
            if (btnBg.Color.G.ToString() == "82")
            {


                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplateRed"];
                btnSearch.Content = "\uE894";
            }

            //change to search
            else if (btnBg.Color.G.ToString() == "14")
            {


                txtSearch.Text = "";
                btnSearch.Style = (Style)Application.Current.Resources["ButtonTemplate"];
                btnSearch.Content = "\uE1A3";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();
        }

        private void btnWinner_Click(object sender, RoutedEventArgs e)
        {
            //check if members
            if (memberCount == 0)
            {
                PageIndex.ShowError("There are no winners to pick! Start swiping.");
                return;
            }

            //create randomizer
            Random ran = new Random();

            //pick winner
            Attendance winner = attendanceHistory[ran.Next(0, memberCount)];

            //display winner
            lblWinner.Text = winner.memberNumber + " " + winner.firstName + " " + winner.lastName;
            lblWinner.Visibility = Visibility.Visible;
        }
    }
}
