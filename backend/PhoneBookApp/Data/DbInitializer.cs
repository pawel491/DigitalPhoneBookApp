using System;
using Microsoft.EntityFrameworkCore;
using PhoneBookApp.Model.Entities;

namespace PhoneBookApp.Data;

public static class DbInitializer
{
    public static void InitializeDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();

            SeedData(dbContext);
        } catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
        }
    }
    public static void SeedData(AppDbContext context)
    {
        if (context.PhoneContacts.Any())
        {
            return; // Database has some data
        }

        var contacts = new List<PhoneContact>
        {
            new PhoneContact { Name = "John Doe", PhoneNumber = "+48123123123" },
            new PhoneContact { Name = "Jane Smith", PhoneNumber = "+48987654321" },
            new PhoneContact { Name = "Alice Johnson", PhoneNumber = "+48555123456" }
        };

        context.PhoneContacts.AddRange(contacts);
        context.SaveChanges();
    }
}
