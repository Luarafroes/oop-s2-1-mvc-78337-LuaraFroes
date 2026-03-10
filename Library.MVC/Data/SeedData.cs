using Bogus;
using Library.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library.MVC.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            context.Database.Migrate();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // --------------------
            // Seed Roles
            // --------------------
            if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));

            if (!await roleManager.RoleExistsAsync(AppRoles.Member))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Member));

            // --------------------
            // Seed Admin User
            // --------------------
            var adminEmail = "admin@library.com";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Letmein01*");
                await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            }

            // Seed Books
            if (!context.Books.Any())
            {
                var bookFaker = new Faker<Book>()
                    .RuleFor(b => b.Id, f => 0) // Id will be set by the database
                    .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                    .RuleFor(b => b.Author, f => f.Name.FullName())
                    .RuleFor(b => b.Category, f => f.PickRandom(new[]
{
    "Fiction",
    "Science",
    "Technology",
    "History",
    "Biography"
}))
                    .RuleFor(b => b.IsAvailable, true)
                    .RuleFor(b => b.ISBN, f => f.Random.Replace("###-#-##-######-#"));

                context.Books.AddRange(bookFaker.Generate(20));
                context.SaveChanges();
            }

            // Seed Members
            if (!context.Member.Any())
            {
                var memberFaker = new Faker<Member>()
                    .RuleFor(m => m.Id, f => 0) // Id will be set by the database
                    .RuleFor(m => m.Name, f => f.Name.FullName())
                    .RuleFor(m => m.Email, f => f.Internet.Email())
                    .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("##########"));

                context.Member.AddRange(memberFaker.Generate(10));
                context.SaveChanges();
            }

            // Seed Loans
            if (!context.Loans.Any())
            {
                var books = context.Books.ToList();
                var members = context.Member.ToList();
                var random = new Random();
                var loans = new List<Loan>();

                for (int i = 0; i < 15; i++)
                {
                    var book = books[random.Next(books.Count)];
                    if (!book.IsAvailable) continue;

                    var member = members[random.Next(members.Count)];
                    var loanDate = DateTime.Now.AddDays(-random.Next(30));
                    var dueDate = loanDate.AddDays(14);
                    var returned = random.Next(0, 2) == 0 ? (DateTime?)null : loanDate.AddDays(random.Next(1, 14));

                    loans.Add(new Loan
                    {
                        BookId = book.Id,
                        MemberId = member.Id,
                        LoanDate = loanDate,
                        DueDate = dueDate,
                        ReturnedDate = returned
                    });

                    if (returned == null) book.IsAvailable = false;
                }

                context.Loans.AddRange(loans);
                context.SaveChanges();
            }
        }
    }


    }

