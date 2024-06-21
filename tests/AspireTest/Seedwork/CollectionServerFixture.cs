namespace AspireTest.Seedwork;

[CollectionDefinition(nameof(CollectionServerFixture))]
public class CollectionServerFixture
    :ICollectionFixture<ApiServiceFixture>
{
}