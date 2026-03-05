using Bogus;
using Library.Domain;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Library.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }

    }

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (!context.Books.Any())
        {
            var bookFaker = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.IsAvailable, f => true)
                .RuleFor(b => b.ISBN, f => f.Random.Replace("###-#-##-######-#"));

            var books = bookFaker.Generate(20);
            context.Books.AddRange(books);
        }

        if (!context.Member.Any())
        {
            var memberFaker = new Faker<Member>()
                .RuleFor(m => m.Name, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber("##########"));

            var members = memberFaker.Generate(10);
            context.Member.AddRange(members);
        }

        context.SaveChanges();

        if (!context.Loans.Any())
        {
            var books = context.Books.ToList();
            var members = context.Member.ToList();
            var random = new Random();

            var loans = new List<Loan>();
            for (int i = 0; i < 15; i++)
            {
                var book = books[random.Next(books.Count)];
                if (!book.IsAvailable) continue; // skip already loaned

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
        }

        context.SaveChanges();
    }
}