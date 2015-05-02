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
using System.Threading;
using System.Threading.Tasks;

namespace Samotorcan.Examples.TodoList.Controllers
{
    public class TodoListController : ObservableController
    {
        private const string _todosFileName = "todos.json";

        private ObservableCollection<TodoItem> _todos;
        public ObservableCollection<TodoItem> Todos
        {
            get
            {
                return _todos;
            }
            set
            {
                var oldTodos = _todos;

                if (SetField(ref _todos, value))
                {
                    TodosChanged();

                    if (oldTodos != null)
                        oldTodos.CollectionChanged -= Todos_CollectionChanged;

                    if (_todos != null)
                        _todos.CollectionChanged += Todos_CollectionChanged;
                }
            }
        }

        public TodoListController()
        {
            if (File.Exists(_todosFileName))
                Todos = new ObservableCollection<TodoItem>(JsonConvert.DeserializeObject<List<TodoItem>>(File.ReadAllText(_todosFileName, Encoding.UTF8)));
            else
                Todos = new ObservableCollection<TodoItem>();
        }

        public void ClearTodos()
        {
            Todos = new ObservableCollection<TodoItem>();
        }

        private void TodosChanged()
        {
            Debug.WriteLine("[" + string.Join(", ", Todos.Select(t => "'" + t.Text + "'")) + "]");
        }

        private void Todos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TodosChanged();
        }

        protected override void Dispose(bool disposing)
        {
            File.WriteAllText(_todosFileName, JsonConvert.SerializeObject(Todos), Encoding.UTF8);

            base.Dispose(disposing);
        }
    }
}
