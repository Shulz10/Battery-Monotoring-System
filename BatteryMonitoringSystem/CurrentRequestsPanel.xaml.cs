﻿using BatteryMonitoringSystem.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace BatteryMonitoringSystem
{
    /// <summary>
    /// Interaction logic for CurrentRequestsPanel.xaml
    /// </summary>
    public partial class CurrentRequestsPanel : UserControl
    {
        public CurrentRequestsPanel()
        {
            InitializeComponent();
        }

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(CurrentRequestsPanel),
                new PropertyMetadata(false, new PropertyChangedCallback(IsPressedChanged)));

        private static void IsPressedChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            CurrentRequestsPanel panel = (CurrentRequestsPanel)depObj;
            if(!panel.IsPressed && panel.Parent != null)
            {
                ThicknessAnimation animate = new ThicknessAnimation()
                {
                    To = new Thickness(-350, 0, 0, 0),
                    Duration = TimeSpan.FromSeconds(0.75)
                };
                animate.Completed += (o, s) => { RemoveUserControl(panel); };
                panel.BeginAnimation(MarginProperty, animate);
            }
        }

        private static void RemoveUserControl(CurrentRequestsPanel panel)
        {
            var parent = (Panel)LogicalTreeHelper.GetParent(panel);
            parent.Children.Remove(panel);
        }

        private void CurrentRequestsListPanel_Loaded(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation animate = new ThicknessAnimation()
            {
                From = new Thickness(-700, 0, 0, 0),
                To = new Thickness(0, 0, 0, 0),
                Duration = TimeSpan.FromSeconds(1)
            };
            animate.Completed += (o, s) => { RemoveOldPanel(); };
            BeginAnimation(MarginProperty, animate);
        }

        private void RemoveOldPanel()
        {
            var parent = (Panel)LogicalTreeHelper.GetParent(this);
            foreach (var element in parent.Children)
            {
                if (element is UserControl)
                {
                    if (Grid.GetRow((UserControl)element) == 1 && Grid.GetColumn((UserControl)element) == 1 && ((UserControl)element).Name != this.Name)
                    {
                        parent.Children.Remove((UserControl)element);
                        element.GetType().GetProperty("IsPressed").SetValue(element, false);
                        break;
                    }
                }
            }
        }

        public void AddNewRequest(string phoneNumber, SmsRequest smsRequest)
        {
            Label phoneNumberLabel = new Label() { Name = $"request{phoneNumber.TrimStart('+')}", Content = phoneNumber, Width = 120 };

            Label messagesCounterLabel = new Label() { Width = 40 };
            Binding binding = new Binding("StatisticsByReceivedMessage")
            {
                Source = smsRequest,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(messagesCounterLabel, ContentProperty, binding);

            Label requestTimeLabel = new Label() { Width = 120 };
            binding = new Binding("DiffRequestTime")
            {
                Source = smsRequest,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(requestTimeLabel, ContentProperty, binding);

            Image closeRequestBtn = new Image() { Source = new BitmapImage(new Uri("pack://application:,,,/Resources/closeBtn.png")) };

            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 10, 0, 5) };
            panel.Children.Add(phoneNumberLabel);
            panel.Children.Add(messagesCounterLabel);
            panel.Children.Add(requestTimeLabel);
            panel.Children.Add(closeRequestBtn);

            listRequests.Children.Add(panel);
        }

        public void NoRequest()
        {
            Label label = new Label()
            {
                Content = "Список запросов пуст!",
                FontFamily = new FontFamily("Sitka Display"),
                FontStyle = FontStyles.Italic,
                FontSize = 16,
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5)
            };

            listRequests.Children.Add(label);
        }
    }
}
