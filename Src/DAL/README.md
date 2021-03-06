# Repository
## Technologies/Dependencies
[EntityFramework][0]

## Dependency injection
For each custom aggregate root the generic implementation of IRepository<> is auto registered and can be injected via IRepository<> contract. When custom repository for aggregate root is created then generic implementation of repository is overwritten. Lifestyle is Scoped.

## Overview
### Generic repository

Contract:
```csharp
public interface IRepository<T, in TKey> where T : class, IAggregateRootEntity<TKey>
{
    /// <summary>
    /// Sync CrudState in aggregate root and in the related entities. If graph contains links to other aggregate roots they will be skipped.
    /// </summary>
    /// <param name="aggregateRoot"></param>
    void PersistAggregateRoot(T aggregateRoot);

    /// <summary>
    /// Read aggregate root with all related entities but w/o other aggregate roots.
    /// </summary>
    /// <param name="aggregateRootKey"></param>
    /// <returns>Aggregate root graph</returns>
    Task<T> ReadAggregateRootAsync(TKey aggregateRootKey);

    /// <summary>
    /// Read aggregate root with all related entities but w/o other aggregate roots.
    /// </summary>
    /// <param name="aggregateRootKey"></param>
    /// <returns>Aggregate root graph</returns>
    T ReadAggregateRoot(TKey aggregateRootKey);
}
```
```csharp
public class Repository<T, TKey> : IRepository<T, TKey> where T : class, IAggregateRootEntity<TKey>
{    
    public virtual void PersistAggregateRoot(T entity) { ... }
    public virtual async Task<T> ReadAggregateRootAsync(TKey key) { ... }
    public virtual T ReadAggregateRoot(TKey key) { ... }
    
    protected readonly IDataContext DataContext;
    protected DbSet<T> GetDbSet() { ... }
}
```

To inject a generic repository use generic interface:

```csharp
public class CarsEntityService : ICarsEntityService
{
  public CarsEntityService(IRepository<Car, Guid> carsRepository) {}
}
```

### Custom repository

Example:
```csharp
public interface ICarsRepository : IRepository<Car, Guid>
{
  IEnumerable<Car> GetCarsWithColor(string color);
}
```
```csharp
public class CarsRepository : Repository<Car, Guid>, ICarsRepository
{
  IEnumerable<Car> GetCarsWithColor(string color) { ... }
}
```

And now CarsRepository implementation can be injected via ICarsRepository and IRepository<<Car, Guid>> contracts:

```csharp
public class CarsEntityService : ICarsEntityService
{
  public CarsEntityService(ICarsRepository carsRepository) {}
}
```

Note: Protected members and methods (DataContext, GetDbSet) in the generic implementation can be used in the custom implementations.

# Unit of Work

## Dependency injection
It's auto registered and can be injected via IUnitOfWork interface.

## Overview

Api:

```csharp
public interface IUnitOfWork
{
    /// <summary>
    /// Save changes to DataBase in single transaction async
    /// </summary>
    /// <returns></returns>
    Task SaveAsync();

    /// <summary>
    /// Save changes to DataBase in single transaction
    /// </summary>
    /// <returns></returns>
    void Save();
}
```

Inject UoW example:

```csharp
public class CarsWorkflowService : ICarsWorkflowService
{
  public CarsWorkflowService(IUnitOfWork unitOfWork) {}
}
```

# Query Repository

## Technologies/Dependencies
[Dapper][1]

## Dependency injection
Services marked as IQueryRepository are auto registered with Scoped lifestyle.

Query repository example:
```csharp
public class CarVmDto
{
    public string PublicKey { get; set; }
    public string Color { get; set; }
}
```
```csharp
public interface ICarsQueryRepository : IQueryRepository
{
    Task<IEnumerable<CarVmDto>> GetAllCarsAsync();
}
```
```csharp
public class CarsQueryRepository : QueryRepositoryBase, ICarsQueryRepository
{
    public CarsQueryRepository(IOptions<ConnectionStrings> connectionStrings) : base(connectionStrings)
    {
    }

    public async Task<IEnumerable<CarVmDto>> GetAllCarsAsync()
    {
        var sql = "SELECT * FROM [dbo].[Car];";

        var dtos = await GetFilteredListAsync<CarVmDto>(sql);
        return dtos;
    }
}
```
QueryRepositoryBase has helper methods that can be used to implement your custom methods. Or dapper can be used directly. To access SqlConnection use protected SqlConnection GetDbConnection().

# Entity Mapping

[EntityFramework fluent api][3] is used for mapping.
Framework automatically loads all intances of IMappingModule interface and passes EntityFramework ModelBuilder. Example:

Automaticcaly registered entity properties:
* Id - registered as .HasKey(x => x.Id)
* CrudState - ignored as .Ignore(x => x.CrudState)

Id property is auto registered as 

```csharp
public class CarsMappingModule : IMappingModule
{
    public void Install(IModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wheel>();

        var carEntityBuilder = modelBuilder.Entity<Car>();

        carEntityBuilder
            .HasMany(x => x.Wheels)
            .WithOne()
            .HasForeignKey(x => x.CarId);
    }
}
```

# Connection Strings
To add connection strings just put next section to appsetting.json file:

```javascript
"connectionStrings": {
    "Oltp": "Data Source=(local); Initial Catalog=DddCore.Tests.Integration.Database; Integrated Security=SSPI;",
    "ReadOnly": "Data Source=(local); Initial Catalog=DddCore.Tests.Integration.Database; Integrated Security=SSPI;"
}
```
And add configuration to container:
```csharp
services.Configure<ConnectionStrings>(Configuration.GetSection("connectionStrings"));
```
ConnectionStrings class will be injected to repository constructor via IOptions<>:

```csharp
public class ConnectionStrings
{
    /// <summary>
    /// Connection string to DataBase for write operations
    /// </summary>
    public string Oltp { get; set; }

    /// <summary>
    /// Connection string to readonly DataBase
    /// </summary>
    public string ReadOnly { get; set; }
}
```
```csharp
public DataContext(IOptions<ConnectionStrings> connectionStrings)
{
    this.connectionStrings = connectionStrings.Value;
}
```

[Return][2]

[0]: https://docs.microsoft.com/en-us/ef/core/
[1]: https://github.com/StackExchange/dapper-dot-net
[2]: https://github.com/Alexander-Shein/DddCore/blob/net-core/README.md
[3]: https://msdn.microsoft.com/en-us/library/jj591617(v=vs.113).aspx


