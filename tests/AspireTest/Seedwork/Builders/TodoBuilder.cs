using Todos.Domain;

namespace AspireTest.Seedwork.Builders
{
    internal class TodoBuilder
    {
        private string _title = "todo-title";

        public TodoBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }
   
        public Todo Build()
        {
            return new Todo(_title);
        }
    }
}
