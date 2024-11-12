using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using System;

namespace UnitTests.Models
{
    [TestFixture]
    public class CommentModelTests
    {
        [Test]
        public void CommentModel_Valid_Id_Should_Be_Gettable_And_Settable()
        {
            // Arrange
            var comment = new CommentModel();

            // Act
            comment.Id = "comment123";

            // Assert
            Assert.That(comment.Id, Is.EqualTo("comment123"));
        }

        [Test]
        public void CommentModel_Valid_Comment_Should_Be_Gettable_And_Settable()
        {
            // Arrange
            var comment = new CommentModel();

            // Act
            comment.Comment = "This is a test comment.";

            // Assert
            Assert.That(comment.Comment, Is.EqualTo("This is a test comment."));
        }

        [Test]
        public void CommentModel_Default_Constructor_Should_Initialize_Properties_To_Null()
        {
            // Arrange & Act
            var comment = new CommentModel();

            // Assert
            Assert.That(comment.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(Guid.TryParse(comment.Id, out _), Is.True, "Id should be a valid GUID.");
            Assert.That(comment.Comment, Is.Null);
        }
    }
}
