using PocketLLM.Services;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PocketLLM
{
    public partial class MainWindow : Window
    {
        private NotifyIcon trayIcon;

        private double hideOffset;

        public ObservableCollection<ChatMessage> Messages { get; }
        = new ObservableCollection<ChatMessage>();

        public Config.ConfigClass config = new Config.ConfigClass();
        private LLMService llmService;

        public MainWindow()
        {
            InitializeComponent();
            InitializeIcon();
            PositionWindow();

            config = Config.ConfigService.Load();
            llmService = new LLMService(config);

            SettingsPopup.Child = new SettingsMenu
            {
                config = this.config,
                DataContext = this.config,
                ParentPopup = this.SettingsPopup
            };

            Loaded += (_, __) =>
            {
                hideOffset = ActualHeight - TopBar.Height;

                if (RootGrid.RenderTransform is not TranslateTransform transform)
                {
                    transform = new TranslateTransform();
                    RootGrid.RenderTransform = transform;
                }
                transform.Y = hideOffset;
            };

            this.MouseEnter += Window_MouseEnter;
            this.MouseLeave += Window_MouseLeave;

            DataContext = this;
        }

        private void InitializeIcon()
        {
            trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("icon.ico"),
                Visible = true,
                Text = "PocketLLM"
            };

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Close", null, (s, e) => this.Close());
            contextMenu.Items.Add("Settings", null, (s, e) => SettingsPopup.IsOpen = true);
            trayIcon.ContextMenuStrip = contextMenu;
            
            trayIcon.DoubleClick += (s, e) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        // Сворачивание
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Minimized)
                this.Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            base.OnClosing(e);
        }

        private void PositionWindow()
        {
            var workArea = SystemParameters.WorkArea;

            Left = workArea.Right - Width - 10;

            Top = workArea.Bottom - Height;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HideWindowService.HideFromAltTab(this);
            SystemParameters.StaticPropertyChanged += (_, __) => PositionWindow();
        }

        // Анимация появления и скрытия
        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateContent(0);
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateContent(hideOffset);
        }


        private void AnimateContent(double to)
        {
            if (RootGrid.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                RootGrid.RenderTransform = transform;
            }

            var animation = new DoubleAnimation
            {
                To = to,
                Duration = TimeSpan.FromMilliseconds(300),
                AccelerationRatio = 0.3,
                DecelerationRatio = 0.3
            };

            transform.BeginAnimation(TranslateTransform.YProperty, animation);
        }

        // Отправка сообщения
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var message = InputField.Text.Trim();

            if (!string.IsNullOrEmpty(message))
            {
                Messages.Clear();
                //System.Windows.MessageBox.Show($"Отправлено: {message}", "PocketLLM", MessageBoxButton.OK, MessageBoxImage.Information);

                Messages.Add(new ChatMessage { Text = message, IsUser = true });
                InputField.Clear();

                var tempMessage = new ChatMessage { Text = "Thinking...", IsUser = false };
                Messages.Add(tempMessage);

                var response = await SendToAgent(message);
                
                tempMessage.Text = response;
            }
        }

        private async void EnterClickHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendButton_Click(sender, e);
                e.Handled = true;
            }
        }

        private async Task<string> SendToAgent(string message)
        {
            return await llmService.SendMessage(message);
        }

        // Настройка

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup.IsOpen = true;
        }

        // Закрытие
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
