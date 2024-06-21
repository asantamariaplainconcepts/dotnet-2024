using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Todos.Features.Todo.Commands;

namespace FunctionalTests.Features.Todo.Commands;

public class CreateTodoTestsShould(ApiServiceFixture fixture) :  ApiTestBase(fixture)
{

    [Fact]
    public async Task response_ok_when_create_one_todo()
    {
        // Arrange
        var command = new CreateTodo.Command
        {
            Title = "test"
        };

        // Act
        var response = await Given.Client.PostAsJsonAsync(ApiDefinition.V1.Todo.CreateTodo(), command);

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
        var command = new CreateTodo.Command
        {
            Title = string.Empty
        };

        // Act
        var response = await Given.Client.PostAsync(ApiDefinition.V1.Todo.CreateTodo(), command);

        // Assert
        response.Should().NotBeNull();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}