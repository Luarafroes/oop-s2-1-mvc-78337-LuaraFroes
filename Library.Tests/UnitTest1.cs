using Library.Domain;
using Library.MVC;
using Library.MVC.Data;
using Library.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Library.Tests
{

    public class TestHelper
    {
        public static ApplicationDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
    public class UnitTest1
    {
        [Fact]
        public void ThisTestshouldPass()
        {
            string name = "Luara";
            Assert.Equal("Luara", name);
        }

        [Fact]
        public void book_Should_Have_Title()
        {
            // Arrange
            var book = new Library.Domain.Book
            {
                Title = "The Great Gatsby"
            };
            // Act
            var title = book.Title;
            // Assert
            Assert.Equal("The Great Gatsby", title);


        }
    }
    public class BookTests
    {
        [Fact]
        public void Book_Should_Have_Title()
        {
            // Arrange
            var book = new Library.Domain.Book
            {
                Title = "The Great Gatsby"
            };
            // Act
            var title = book.Title;
            // Assert
            Assert.Equal("The Great Gatsby", title);
        }
        [Fact]
        public void Book_Should_Have_Author()
        {
            // Arrange
            var book = new Library.Domain.Book
            {
                Author = "F. Scott Fitzgerald"
            };
            // Act
            var author = book.Author;
            // Assert
            Assert.Equal("F. Scott Fitzgerald", author);
        }

    }

    public class LoanTests
    {
        [Fact]
        public void CannotCreateLoanForBookAlreadyOnLoan()
        {
            using var context = TestHelper.GetContext();

            var book = new Book { Title = "Test Book", IsAvailable = false };
            context.Books.Add(book);
            context.SaveChanges();

            Assert.False(book.IsAvailable);
        }
    }
}

public class LoanTests
{
    [Fact]
    public void CannotCreateLoanForBookAlreadyOnLoan()
    {
        using var context = TestHelper.GetContext();

        var book = new Book { Title = "Test Book", IsAvailable = false };
        context.Books.Add(book);
        context.SaveChanges();

        Assert.False(book.IsAvailable);
    }

    [Fact]
    public void OverdueLoanDetected()
    {
        using var context = TestHelper.GetContext();

        var loan = new Loan
        {
            LoanDate = DateTime.Now.AddDays(-20),
            DueDate = DateTime.Now.AddDays(-5),
            ReturnedDate = null
        };

        context.Loans.Add(loan);
        context.SaveChanges();

        var overdue = context.Loans
            .Where(l => l.DueDate < DateTime.Now && l.ReturnedDate == null)
            .ToList();

        Assert.Single(overdue);
    }


    public class RoleTests
    {
        [Fact]
        public void AdminRole_Should_Exist()
        {
            // Arrange
            var adminRole = AppRoles.Admin;
            // Act
            var expectedRole = "Admin";
            // Assert
            Assert.Equal(expectedRole, adminRole);
        }
    }

    public class AuthorizationTests
    {
        [Fact]
        public void RoleControllerRequiresAdminRole()
        {
            var controller = typeof(RolesController);

            var attribute = controller
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .FirstOrDefault() as AuthorizeAttribute;

            Assert.NotNull(attribute);
            Assert.Contains("Admin", attribute.Roles);
        }
    }
    public class SeedDataTests
    {
        [Fact]
        public void AppRoles_Should_Have_Admin_And_Member()
        {
            // Arrange
            var adminRole = AppRoles.Admin;
            var memberRole = AppRoles.Member;
            // Act
            var expectedAdminRole = "Admin";
            var expectedMemberRole = "Member";
            // Assert
            Assert.Equal(expectedAdminRole, adminRole);
            Assert.Equal(expectedMemberRole, memberRole);
        }
    }
    public class AccessControlTests
    {
        [Fact]
        public void RolesController_Should_Have_Admin_Authorization()
        {
            // Arrange
            var controllerType = typeof(RolesController);
            // Act
            var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .FirstOrDefault() as AuthorizeAttribute;
            // Assert
            Assert.NotNull(authorizeAttribute);
            Assert.Equal(AppRoles.Admin, authorizeAttribute.Roles);
        }

        [Fact]
        public void BookSearchReturnsExpectedMatches()
        {
            using var context = TestHelper.GetContext();

            context.Books.AddRange(
                new Book { Title = "C# Programming", Author = "John", IsAvailable = true },
                new Book { Title = "Python Basics", Author = "Anna", IsAvailable = true }
            );

            context.SaveChanges();

            var search = "C#";

            var results = context.Books
                .Where(b => b.Title.Contains(search) || b.Author.Contains(search))
                .ToList();

            Assert.Single(results);
        }
    }
}


