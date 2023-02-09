using TestFecthOptimizer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IRepository, Repository>()
    .AddSingleton<OnlyParentIdTypeInterceptor>()
    .AddSingleton<ITypesOnlyParentIdService, TypesOnlyParentIdService>()
    .AddGraphQLServer().TryAddTypeInterceptor<OnlyParentIdTypeInterceptor>()
    .AddTypes();

var app = builder.Build();
app.MapGraphQL();
app.Run();
