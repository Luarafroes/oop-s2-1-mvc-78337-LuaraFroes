using Library.Domain;
using Library.MVC;

namespace Library.Tests
{
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
        public void Loanshouldhavebook()
        {
            // Arrange
            var book = new Library.Domain.Book
            {
                Title = "The Great Gatsby"
            };
            var loan = new Loan
            {
                Book = book
            };
            // Act
            var loanBook = loan.Book;
            // Assert
            Assert.Equal(book, loanBook);
        }
        [Fact]
        public void Loanshoudhavemember()
        {
            // Arrange
            var member = new Member
            {
                Name = "John Doe"
            };
            var loan = new Loan
            {
                Member = member
            };
            // Act
            var loanMember = loan.Member;
            // Assert
            Assert.Equal(member, loanMember);
        }
        [Fact]
        public void Loan_Should_Have_LoanDate()
        {
            // Arrange
            var loanDate = DateTime.Now;
            var loan = new Loan
            {
                LoanDate = loanDate
            };
            // Act
            var actualLoanDate = loan.LoanDate;
            // Assert
            Assert.Equal(loanDate, actualLoanDate);
        }
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
}
