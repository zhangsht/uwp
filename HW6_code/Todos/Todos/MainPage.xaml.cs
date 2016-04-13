using NotificationsExtensions.Tiles;
using SQLitePCL;
using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            this.ViewModel = new ViewModels.TodoItemViewModel();
        }

        ViewModels.TodoItemViewModel ViewModel { get; set; }
        Models.TodoItem item;
        StorageFile file;

        protected  override  void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }
        private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataPackage data = args.Request.Data;
            string textSource = "title and description from Todo item";
            data.Properties.Title = item.title;
            data.Properties.Description = item.description;
            data.SetText(textSource);
            DataRequestDeferral GetFiles = args.Request.GetDeferral();
            try
            {
                data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(item.shareFile);
                data.SetBitmap(RandomAccessStreamReference.CreateFromFile(item.shareFile));
            }
            finally
            {
                GetFiles.Complete();
            }
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            Frame.Navigate(typeof(NewPage), ViewModel);
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (InlineToDoItemViewGrid.Visibility != ToDoListView.Visibility)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
        }

        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            Models.TodoItem TodoToCreate = new Models.TodoItem(title2.Text,
        details2.Text, dueDate2.Date, image2.Source, file);

            if (TodoToCreate.TodoInfoValidator())
            {
                ViewModel.AddTodoItem(TodoToCreate);
            }
            var db = App.conn;
            try
            {
                using (var TodoItem = db.Prepare("INSERT INTO TodoItems(Title, Description, DueDate) VALUES(?, ?, ?)"))
                {
                    TodoItem.Bind(1, title2.Text);
                    TodoItem.Bind(2, details2.Text);
                    TodoItem.Bind(3, dueDate2.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    TodoItem.Step();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void click_back(object sender, RoutedEventArgs e)
        {
            title2.Text = "";
            details2.Text = "";

        }
        private async void Click_SelectPicture(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (Windows.Storage.Streams.IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    image2.Source = bitmapImage;
                }
            }
        }

        private void onClick(object sender, RoutedEventArgs e)
        {
            var len = ViewModel.AllItems.Count;
            if (len < 1) return;
            string title = ViewModel.AllItems[len - 1].title;
            string details = ViewModel.AllItems[len - 1].description;
            bool? finished = ViewModel.AllItems[len - 1].completed;
            //var Notify = new MessageDialog(myTile.title).ShowAsync();
            if (ViewModel.SelectedItem != null)
            {
                title = ViewModel.SelectedItem.title;
                details = ViewModel.SelectedItem.description;
                finished = ViewModel.SelectedItem.completed;
                ViewModel.SelectedItem = null;
            }

            string is_finished = string.Empty;
            if (finished == true)
            {
                is_finished += "This item has been done.";
            } else
            {
                is_finished += "This item hasn't been done.";
            }
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    DisplayName = "Todos",
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = title,
                                    Style = TileTextStyle.Subtitle
                                }
                            }
                        }
                    },
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = title,
                                    Style = TileTextStyle.Subtitle
                                },
                                new TileText()
                                {
                                    Text = details,
                                    Style = TileTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = title,
                                    Style = TileTextStyle.Subtitle
                                },
                                new TileText()
                                {
                                    Text = details,
                                    Style = TileTextStyle.CaptionSubtle
                                },
                                new TileText()
                                {
                                    Text = is_finished,
                                    Style = TileTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private void onShare(object sender, RoutedEventArgs e)
        {
            item = (Models.TodoItem)(((MenuFlyoutItem)sender).DataContext);
            DataTransferManager.ShowShareUI();
        }

        private void BtnGetAll_Click(object sender, RoutedEventArgs e)
        {
            string result = string.Empty;
            StringBuilder DataQuery = new StringBuilder("%%");
            DataQuery.Insert(1, Query.Text);
            var db = App.conn;
            using (var statement = db.Prepare("SELECT Title, Description, DueDate FROM TodoItems WHERE Title LIKE ? OR Description LIKE ? OR DueDate LIKE ?"))
            {
                statement.Bind(1, DataQuery.ToString());
                statement.Bind(2, DataQuery.ToString());
                statement.Bind(3, DataQuery.ToString());

                while (SQLiteResult.ROW == statement.Step())
                {

                    result += statement[0].ToString();
                    result += " ";
                    result += statement[1].ToString();
                    result += " ";
                    result += statement[2].ToString();
                    result += "\n";
                }

            }

            var Notify = new MessageDialog(result).ShowAsync();

        }
       
    }
}
