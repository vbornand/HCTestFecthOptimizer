using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

public class AuthorType : ObjectType<Author>
{
    protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
    {
        base.Configure(descriptor);

        //Just to check if it works when the GraphQL type does not match with the class name.
        descriptor.Name("TheAuthor");

        descriptor.Field(a => a.Id).UseOnlyParentId();
    }
}