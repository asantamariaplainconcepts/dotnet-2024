using MediatR;

namespace ArchTests.Features;

public class SlicesTests
{
    [Fact]
    public void AllRequestsShouldBeUsedAsQueryOrCommand()
    {
        var result = AppTypes()
            .That()
            .ImplementInterface(typeof(IRequest))
            .And()
            .AreClasses()
            .Or()
            .ImplementInterface(typeof(IRequest<>))
            .And()
            .AreClasses()
            .Should()
            .ImplementInterface(typeof(IBaseQuery))
            .Or()
            .ImplementInterface(typeof(IBaseCommand))
            .GetResult();


        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void AllSlicesNotHaveDependenciesBetweenSlices()
    {
        var result = AppTypes()
            .Slice()
            .ByNamespacePrefix(nameof(Todos.Features))
            .Should()
            .NotHaveDependenciesBetweenSlices()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}


