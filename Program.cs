using TestFecthOptimizer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IRepository, Repository>()
    .AddSingleton<ITypesOnlyParentIdService, TypesOnlyParentIdService>()
    .AddGraphQLServer()
    .AddTypes();

var app = builder.Build();
app.MapGraphQL();
app.Run();
