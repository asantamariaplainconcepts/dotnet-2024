using Todos.Domain;

namespace UnitTests.Domain.Entities;

public class TodoTests
{
    [Fact]
    public void creates_new_todo()
    {
        // Act
        var todo = new Todo("test");

        // Assert
        todo.Id.Should().NotBeNull();
        todo.Title.Should().Be("test");
        todo.Completed.Should().BeFalse();
    }
    
    [Fact]
    public void throws_exception_if__title_empty()
    {
        // Act
        var act = () => new Todo("");

        // Assert
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void throws_exception_if_title_update_empty()
    {
        //Arrange
       var todo =  new Todo("test");
        
        // Act
        var act = () => todo.UpdateTitle(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}