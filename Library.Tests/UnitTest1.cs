using Library.Domain;

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
    }
