using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using System;

namespace UnitTests.Models
{
    /// <summary>
    /// Unit tests for the <see cref="CommentModel"/> class.
    /// </summary>
    [TestFixture]
    public class CommentModelTests
    {
        /// <summary>
        /// Tests if the <see cref="CommentModel.Id"/> property can be correctly set and retrieved.
        /// </summary>
        [Test]
        public void CommentModel_Valid_Id_Should_Be_Gettable_And_Settable()
        {
            // Arrange: Create a new instance of CommentModel.
            var comment = new CommentModel();

            // Act: Set the Id property.
            comment.Id = "comment123";

            // Assert: Verify the Id property was set correctly.
            Assert.That(comment.Id, Is.EqualTo("comment123"));
        }

        /// <summary>
        /// Tests if the <see cref="CommentModel.Comment"/> property can be correctly set and retrieved.
        /// </summary>
        [Test]
        public void CommentModel_Valid_Comment_Should_Be_Gettable_And_Settable()
        {
            // Arrange: Create a new instance of CommentModel.
            var comment = new CommentModel();

            // Act: Set the Comment property.
            comment.Comment = "This is a test comment.";

            // Assert: Verify the Comment property was set correctly.
            Assert.That(comment.Comment, Is.EqualTo("This is a test comment."));
        }

        /// <summary>
        /// Tests the default constructor of <see cref="CommentModel"/> to ensure properties are initialized correctly.
        /// </summary>
        [Test]
        public void CommentModel_Default_Constructor_Should_Initialize_Properties_To_Null()
        {
            // Arrange & Act: Create a new instance of CommentModel using the default constructor.
            var comment = new CommentModel();

            // Assert: Verify that Id is not null or empty and is a valid GUID.
            Assert.That(comment.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(Guid.TryParse(comment.Id, out _), Is.True, "Id should be a valid GUID.");

            // Assert: Verify that the Comment property is null by default.
            Assert.That(comment.Comment, Is.Null);
        }
    }
}
