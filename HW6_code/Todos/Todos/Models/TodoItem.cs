using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Todos.Models
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility Vis = (Visibility)value;
            return Vis == Visibility.Collapsed ? false : true;
        }
    }
    class TodoItem
    {
        public string id;
        public string title { get; set; }
        public string description { get; set; }
        public bool? completed { get; set; }
        public DateTime DueDate { get; set; }
        public ImageSource ImagePath { get; set; }
        public StorageFile shareFile { get; set; }

        public TodoItem(string title, string description, DateTimeOffset duedate, ImageSource imagepath, StorageFile shareFile)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.title = title;
            this.description = description;
            this.completed = false; //默认为未完成
            this.DueDate = duedate.DateTime;
            this.ImagePath = imagepath;
            this.shareFile = shareFile;
        }
       
        public bool TodoInfoValidator()
        {
            string MessageToNotify = string.Empty;

            if (this.title.Length == 0)
            {
                MessageToNotify += "Title is empty!\n";
            }

            if (this.description.Length == 0)
            {
                MessageToNotify += "Details are empty!\n";
            }

            if (this.DueDate.Subtract(DateTime.Now).Days < 0)
            {
                MessageToNotify += "Due date should be later than now!\n";
            }

            if (MessageToNotify.Length != 0)
            {
                var Notify = new MessageDialog(MessageToNotify).ShowAsync();
                return false;
            }
            else {
                return true;
            }
        }

        public void Update(ref TodoItem UpdateInfo)
        {
            this.title = UpdateInfo.title;
            this.description = UpdateInfo.description;
            this.DueDate = UpdateInfo.DueDate;
            this.ImagePath = UpdateInfo.ImagePath;
            this.shareFile = UpdateInfo.shareFile;
        }
    }
}
