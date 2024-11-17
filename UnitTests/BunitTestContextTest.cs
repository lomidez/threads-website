using Bunit;
using NUnit.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="BunitTestContext"/> class.
    /// </summary>
    public class BunitTestContextTests : BunitTestContext
    {
        /// <summary>
        /// Test to verify that the <see cref="BunitTestContext"/> initializes and disposes correctly
        /// and renders the <see cref="DummyComponent"/> with the expected markup.
        /// </summary>
        [Test]
        public void TestContext_Valid_Should_Initialize_And_Dispose_Correctly()
        {
            // Arrange: Create and render the DummyComponent using the TestContext
            var dummyComponent = TestContext.RenderComponent<DummyComponent>();

            // Act: Verify the rendered markup of the component
            dummyComponent.MarkupMatches("<p>Hello, Bunit!</p>");

            // Assert: Ensure that the TestContext is not null
            Assert.That(TestContext, Is.Not.Null);
        }

        /// <summary>
        /// A simple dummy component for rendering a static "Hello, Bunit!" message.
        /// </summary>
        public class DummyComponent : ComponentBase
        {
            /// <summary>
            /// Builds the render tree for the component.
            /// </summary>
            /// <param name="builder">The <see cref="RenderTreeBuilder"/> used to build the component's render tree.</param>
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                // Render a paragraph element with text content
                builder.OpenElement(0, "p");
                builder.AddContent(1, "Hello, Bunit!");
                builder.CloseElement();
            }
        }
    }
}
