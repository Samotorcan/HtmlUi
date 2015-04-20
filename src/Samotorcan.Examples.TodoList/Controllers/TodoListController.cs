using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Examples.TodoList.Controllers
{
    public class TodoListController : ObservableController
    {
        private const string _todosFileName = "todos.json";

        private ObservableCollection<TodoItem> _todos;
        public ObservableCollection<TodoItem> Todos
        {
            get { return _todos; }
            set { SetField(ref _todos, value); }
        }

        public TodoListController(int id)
            : base(id)
        {
            if (File.Exists(_todosFileName))
                Todos = new ObservableCollection<TodoItem>(JsonConvert.DeserializeObject<List<TodoItem>>(File.ReadAllText(_todosFileName, Encoding.UTF8)));
            else
                Todos = new ObservableCollection<TodoItem>();

            Todos.CollectionChanged += Todos_CollectionChanged;
        }

        private void Todos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("[" + string.Join(", ", Todos.Select(t => "'" + t.Text + "'")) + "]");
        }

        protected override void Dispose(bool disposing)
        {
            File.WriteAllText(_todosFileName, JsonConvert.SerializeObject(Todos), Encoding.UTF8);

            base.Dispose(disposing);
        }
    }
}
