using Microsoft.EntityFrameworkCore;
using Todos.Features.Todo.Commands;

namespace AspireTest.Features.Todo.Commands;

public class UpdateTodoTestsShould(ApiServiceFixture given) : ApiTestBase(given)
{
    [Fact]
    public async Task response_ok_when_update_one_todo()
    {
        // Arrange
        var todo = await Given.CreateDefaultTodo();

        var command = new UpdateTodo.Command
        {
            Title = "new title"
        };

        // Act
        var response = await  Given.TodoClient.PutAsync(ApiDefinition.V1.Todo.UpdateTodo(todo.Id), command);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Given.ExecuteDbContextAsync(async context =>
        {
            var dbTodo = await context.Todos.FirstOrDefaultAsync();
            dbTodo.Should().NotBeNull();
            dbTodo!.Title.Should().Be(command.Title);
            dbTodo.Completed.Should().Be(false);
        });
    }
    
    [Fact]
    public async Task response_bad_request_when_title_invalid()
    {
        // Arrange
        var command = new UpdateTodo.Command()
        {
            Title = string.Empty
        };

        // Act
        var response = await Given.TodoClient.PutAsync(ApiDefinition.V1.Todo.UpdateTodo("test"), command);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}