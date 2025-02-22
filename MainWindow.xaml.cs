// MainWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PostIt
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<PostItTab> postItTabs;
        private DateTime lastClickTime = DateTime.MinValue;
        private int clickCount = 0;
        private Point dragStartPoint;

        public MainWindow()
        {
            InitializeComponent();
            postItTabs = new ObservableCollection<PostItTab>();
            PostItTabControl.ItemsSource = postItTabs;
            AddNewTab();

            // ランダムな位置に表示
            Random rand = new Random();
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = rand.NextDouble() * (screenWidth - this.Width);
            this.Top = rand.NextDouble() * (screenHeight - this.Height);
        }

        private void AddNewTab()
        {
            var newTab = new PostItTab
            {
                Title = $"メモ {postItTabs.Count + 1}",
                Content = "",
                GoalCount = 140
            };
            postItTabs.Add(newTab);
            PostItTabControl.SelectedIndex = postItTabs.Count - 1;
        }

        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            if ((currentTime - lastClickTime).TotalMilliseconds < 500)
            {
                clickCount++;
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

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);
                if (Math.Abs(currentPosition.X - dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPosition.Y - dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    TabItem tabItem = sender as TabItem;
                    if (tabItem != null)
                    {
                        PostItTab postItTab = tabItem.DataContext as PostItTab;
                        if (postItTab != null)
                        {
                            // 新しいウィンドウを作成
                            var newWindow = new MainWindow();
                            newWindow.postItTabs.Clear();
                            newWindow.postItTabs.Add(postItTab);
                            newWindow.Left = this.Left + currentPosition.X;
                            newWindow.Top = this.Top + currentPosition.Y;
                            newWindow.Show();

                            // 元のタブを削除
                            postItTabs.Remove(postItTab);
                            if (postItTabs.Count == 0)
                            {
                                this.Close();
                            }
                        }
                    }
                }
            }
        }

        private void TabItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(this);
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PostItTab)))
            {
                PostItTab droppedTab = e.Data.GetData(typeof(PostItTab)) as PostItTab;
                TabItem targetTab = sender as TabItem;
                if (targetTab != null && droppedTab != null)
                {
                    PostItTab targetPostItTab = targetTab.DataContext as PostItTab;
                    int targetIndex = postItTabs.IndexOf(targetPostItTab);
                    postItTabs.Insert(targetIndex, droppedTab);
                }
            }
        }
    }

    public class PostItTab : INotifyPropertyChanged
    {
        private string title;
        private string content;
        private int goalCount;
        private double progressHeight;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged(nameof(Content));
                UpdateProgress();
            }
        }

        public int GoalCount
        {
            get => goalCount;
            set
            {
                goalCount = value;
                OnPropertyChanged(nameof(GoalCount));
                UpdateProgress();
            }
        }

        public double ProgressHeight
        {
            get => progressHeight;
            set
            {
                progressHeight = value;
                OnPropertyChanged(nameof(ProgressHeight));
            }
        }

        private void UpdateProgress()
        {
            if (GoalCount > 0)
            {
                double progressPercentage = (double)Content.Length / GoalCount;
                ProgressHeight = progressPercentage * 400; // Window height
                if (Content.Length >= GoalCount)
                {
                    ProgressHeight = 400;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}