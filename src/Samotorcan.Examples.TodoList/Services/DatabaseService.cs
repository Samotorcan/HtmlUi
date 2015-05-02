using Newtonsoft.Json;
using Samotorcan.HtmlUi.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samotorcan.Examples.TodoList.Controllers
{
    public class DatabaseService : Controller
    {
        private const string _todosFileName = "todos.json";

        public void SaveTodos(IEnumerable<TodoItem> todos)
        {
            if (todos == null)
                todos = new List<TodoItem>();

            File.WriteAllText(_todosFileName, JsonConvert.SerializeObject(todos), Encoding.UTF8);
        }

        public List<TodoItem> LoadTodos()
        {
            if (File.Exists(_todosFileName))
                return JsonConvert.DeserializeObject<List<TodoItem>>(File.ReadAllText(_todosFileName, Encoding.UTF8));

            return new List<TodoItem>();
        }
    }
}
