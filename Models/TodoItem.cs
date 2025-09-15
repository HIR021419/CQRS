namespace TodoList.Server.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public bool IsDone { get; set; } = false;

        public TodoItem() { }

        public TodoItem(string name)
        {
            Name = name;
        }
    }
}
