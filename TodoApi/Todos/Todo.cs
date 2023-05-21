using System.ComponentModel.DataAnnotations;

namespace TodoApi.Todos;

public sealed class Todo
{
    public int Id { get; set; }

    [Required] public string Title { get; set; } = default!;

    public bool IsComplete { get; set; }

    [Required] public string OwnerId { get; set; } = default!;
}

public sealed class TodoItem
{
    public int Id { get; set; }

    [Required] public string Title { get; set; } = default!;

    public bool IsComplete { get; set; }
}

public static class TodoMappingExtensions
{
    public static TodoItem AsTodoItem(this Todo todo)
    {
        return new TodoItem
        {
            Id = todo.Id,
            Title = todo.Title,
            IsComplete = todo.IsComplete
        };
    }
}