using Bunit;
using NUnit.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace UnitTests
{
    public class BunitTestContextTests : BunitTestContext
    {
        [Test]
        public void TestContext_ShouldInitialize_AndDisposeCorrectly()
        {
            // Arrange
            var dummyComponent = TestContext.RenderComponent<DummyComponent>();

            // Act
            dummyComponent.MarkupMatches("<p>Hello, Bunit!</p>");

            // Assert
            Assert.That(TestContext, Is.Not.Null);
        }

        public class DummyComponent : ComponentBase
        {
            protected override void BuildRenderTree(RenderTreeBuilder builder)
            {
                builder.OpenElement(0, "p");
                builder.AddContent(1, "Hello, Bunit!");
                builder.CloseElement();
            }
        }
    }
}

