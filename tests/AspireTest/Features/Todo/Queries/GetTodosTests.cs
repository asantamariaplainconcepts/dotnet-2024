using System.Net.Http.Json;
using Todos.Features.Todo.Queries;

namespace AspireTest.Features.Todo.Queries;

public class GetTodosTestsShould(ApiServiceFixture fixture) : ApiTestBase(fixture)
{
    [Fact]
    public async Task response_ok_()
    {
        // Arrange
        var todo = await Given.CreateDefaultTodo();

        // Act

        var response = await Given.TodoClient.GetFromJsonAsync<GetTodos.Response[]>(ApiDefinition.V1.Todo.GetTodos());

        // Assert
        response!.Should().HaveCount(1);
        
    }
}