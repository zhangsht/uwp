using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos.ViewModels
{
    class TodoItemViewModel 
    {
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }
        

        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem {
            get { return selectedItem; }
            set { this.selectedItem = value; }
        }

        public TodoItemViewModel() {}

        public void AddTodoItem(Models.TodoItem todo)
        {
            this.allItems.Add(todo);
        }

        public void RemoveTodoItem(string id)
        {
            for (int i = 0; i < this.allItems.Count; i++)
            {
                if (this.allItems[i].id == id)
                {
                    this.allItems.Remove(this.allItems[i]);
                    break;
                }
            }
           
            this.selectedItem = null;
        }

        public void UpdateTodoItem(Models.TodoItem OriginTodo, Models.TodoItem UpdateInfo)
        {
            int index = this.allItems.IndexOf(OriginTodo);
            if (index >= 0 && index < this.allItems.Count)
            {
                this.allItems[index] = UpdateInfo;
            }
            this.selectedItem = null;
        }

    }
}
