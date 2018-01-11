# EntityFramework.BulkInsert
Updated port of EntityFramework.BulkInsert from the original version on the Codeplex site. This is not my original project, this is to keep it going and add minor updates.  The original is hosted on Codeplex.

# Purpose
The purpose of this library is for performing Bulk Inserts using EntityFramework 6 and your existing `DbContext` instance to perform faster inserts instead of generating multiple insert statements for a collection of strongly typed objects.

# Usage

```cs
IEnumerable<Car> cars = GenerateCars();

using (var context = GetDbContext())
{
    context.BulkInsert<Car>(cars);
}
```

async IO support is also built in:

```cs
IEnumerable<Car> cars = GenerateCars();

using (var context = GetDbContext())
{
    await context.BulkInsertAsync<Car>(cars);
}
```