using Microsoft.EntityFrameworkCore;

namespace FunctionalTests.Features.Todo.Commands;

public class DeleteTodoTestsShould(ApiServiceFixture serverFixture) : ApiTestBase(serverFixture)
{
    [Fact]
    public async Task response_ok_when_delete_one_todo()
    {
        // Arrange
        var todo = await Given.CreateDefaultTodo();

        // Act
        var response = await Given.Client.DeleteAsync(ApiDefinition.V1.Todo.DeleteTodo(todo.Id));

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await Given.ExecuteDbContextAsync(async context =>
        {
            var dbTodo = await context.Todos.FirstOrDefaultAsync();
            dbTodo.Should().BeNull();
        });
    }
}