using Microsoft.EntityFrameworkCore;
using NETMockServer.Data;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed;

public class FakeDataSeeder(IServiceProvider sp, AppDbContext db)
{

    // Ensure seed only if table empty
    public async Task EnsureSeedAsync<T>(int count = 20) where T : class
    {
        var set = db.Set<T>();
        var any = await set.AsNoTracking().AnyAsync();

        if (any)
        {
            return;
        }

        // Resolve faker (will use specific if registered, otherwise default generic)
        var fakerType = typeof(IEntityFaker<>).MakeGenericType(typeof(T));
        var faker = sp.GetService(fakerType) ?? throw new InvalidOperationException($"No faker registered for {typeof(T).Name}");

        var generateMethod = fakerType.GetMethod("Generate")!;
        var list = new List<T>();

        for (var i = 0; i < count; i++)
        {
            var item = (T)generateMethod.Invoke(faker, null)!;
            list.Add(item);
        }

        await set.AddRangeAsync(list);
        await db.SaveChangesAsync();
    }
}