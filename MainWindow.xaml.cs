using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
namespace PostIt
{
    public partial class MainWindow : Window {
        private DateTime lastClickTime = DateTime.MinValue;
        private int clickCount = 0;
        public MainWindow()
        {
            InitializeComponent();

            // ランダムな位置に表示
            Random rand = new Random();
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            this.Left = rand.NextDouble() * (screenWidth - this.Width);
            this.Top = rand.NextDouble() * (screenHeight - this.Height);
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DateTime currentTime = DateTime.Now;

            // 500ミリ秒以内のクリックをカウント
            if ((currentTime - lastClickTime).TotalMilliseconds < 500)
            {
                clickCount++;

                // トリプルクリックで終了
                if (clickCount >= 3)
                {
                    this.Close();
                    return;
                }
            }
            else
            {
                clickCount = 1;
            }

            lastClickTime = currentTime;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MainTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProgress();
        }

        private void GoalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);

        }
        private void GoalTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProgress();
        }
        private void UpdateProgress()
        {
            var goalTextBox = (TextBox)FindName("GoalTextBox");
            var mainTextBox = (TextBox)FindName("MainTextBox");
            var progressBackground = (Border)FindName("ProgressBackground");

            if (int.TryParse(goalTextBox.Text, out int goalCount) && goalCount > 0)
            {
                int currentLength = mainTextBox.Text.Length;
                double progressPercentage = (double)currentLength / goalCount;

                // 進捗に応じて背景の高さを変更
                progressBackground.Height = progressPercentage * this.ActualHeight;

                // 目標達成時の背景色変更
                if (currentLength >= goalCount)
                {
                    progressBackground.Height = this.ActualHeight; // 完全に水色に
                }
            }
        }
    }
}