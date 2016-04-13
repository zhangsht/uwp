using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage.Streams;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            ViewModel = new ViewModels.TodoItemViewModel();

        }

        private ViewModels.TodoItemViewModel ViewModel;
        private StorageFile ImageFile;

        protected  override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
                if (ViewModel.SelectedItem == null)
                {
                    createButton.Content = "Create";
                }
                else
                {
                    createButton.Content = "Update";
                    title.Text = ViewModel.SelectedItem.title;
                    details.Text = ViewModel.SelectedItem.description;
                    dueDate.Date = ViewModel.SelectedItem.DueDate;
                    image.Source = ViewModel.SelectedItem.ImagePath;
                }
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
            //string textSource = "title and description from Todo item";
            data.Properties.Title = ViewModel.SelectedItem.title;
            data.Properties.Description = ViewModel.SelectedItem.description;
            //data.SetText(textSource);
            DataRequestDeferral GetFiles = args.Request.GetDeferral();
            try
            {
                if (ImageFile != null)
                {
                    data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(ImageFile);
                    data.SetBitmap(RandomAccessStreamReference.CreateFromFile(ImageFile));
                } else
                {
                    data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(ViewModel.SelectedItem.shareFile);
                    data.SetBitmap(RandomAccessStreamReference.CreateFromFile(ViewModel.SelectedItem.shareFile));
                }
            }
            finally
            {
                GetFiles.Complete();
            }

        }
        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (createButton.Content.ToString() != "Update")
            {
                Models.TodoItem TodoToCreate = new Models.TodoItem(title.Text, details.Text, dueDate.Date, image.Source, ImageFile);
                if (TodoToCreate.TodoInfoValidator())
                {
                    ViewModel.AddTodoItem(TodoToCreate);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
            }
            else
            {
                UpdateButton_Clicked(sender, e);
            }
            
        }
        private  void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }


        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                StorageFile strFile = ViewModel.SelectedItem.shareFile;
                Models.TodoItem TodoToUpdate = new Models.TodoItem(title.Text, details.Text, dueDate.Date, image.Source, strFile);
                TodoToUpdate.id = ViewModel.SelectedItem.id;
                if (TodoToUpdate.TodoInfoValidator())
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem, TodoToUpdate);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
            }
        }

        private void click_back(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), ViewModel);
        }

        private async void Click_SelectPicture(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            ImageFile = await openPicker.PickSingleFileAsync();
            if (ImageFile != null)
            {
                using (Windows.Storage.Streams.IRandomAccessStream fileStream = await ImageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    Windows.UI.Xaml.Media.Imaging.BitmapImage bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    image.Source = bitmapImage;
                }
            }
        }

        private void OnShare(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
