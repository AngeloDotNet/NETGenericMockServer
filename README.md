# .NET 8 Minimal API Mock Server

## 🛠️ Dependencies

- .NET 8 SDK
- Microsoft.EntityFrameworkCore.Sqlite
- Bogus

## 📚 Endpoints (examples after startup)

| Method | Endpoint |
|--------|----------|
| GET | /api/products |
| GET | /api/products/{id} |
| POST | /api/products |
| PUT | /api/products/{id} |
| DELETE | /api/products/{id} |

## ⚙️ How to extend with a new entity

1. Add the class in Models (inherit EntityBase or add long Id property).
2. Register the entity in the AppDbContext (modelBuilder.Entity<YourEntity>();).
3. (Optional) Implement IEntityFaker<YourEntity> for the specific faker and register it in Services.
4. In Program.cs, add `app.MapEntityEndpoints<YourEntity>("yourentities");`

## 💡 Note

- The GenericRepository uses DbContext.Set<T>() and FindAsync(id) to search by ID; if your entity uses a different pk name, adjust the repository.
- The DefaultEntityFaker attempts to populate common properties (string, number, DateTime). You can override it by registering a type-specific faker.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⭐ Give a Star

Don't forget that if you find this project helpful, please give it a ⭐ on GitHub to show your support and help others discover it.