command: `dotnet ef migrations add initialMigration`

By default, EF Core looks for a class that inherits from DbContext within your project. This class represents your database context.
It also scans your project for classes that represent entities (tables) in your database. These entity classes are typically defined as DbSet<TEntity> properties within your DbContext.
