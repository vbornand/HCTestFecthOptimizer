using TestFecthOptimizer.Infrastructure;

namespace TestFecthOptimizer.Types;

public class BookType : ObjectType<Book>
{
    protected override void Configure(IObjectTypeDescriptor<Book> descriptor)
    {
        base.Configure(descriptor);

        //Just to check if it works when the GraphQL type does not match with the class name.
        descriptor.Name("TheBook");

        //descriptor.Field(a => a.Id).UseOnlyParentId();
    }
}