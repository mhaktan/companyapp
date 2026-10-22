using System;
using Xunit;
using FluentAssertions;
using CompanyApp.Entities;

namespace CompanyApp.Tests.OrderLines
{
    public class OrderLineEntityTests
    {
        [Fact]
        public void OrderLine_ShouldBeCreatable()
        {
            // Act
            var entity = new OrderLine();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void OrderLine_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new OrderLine();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void OrderLine_Description_ShouldAcceptValue()
        {
            var entity = new OrderLine { Description = "Test Value" };
            entity.Description.Should().Be("Test Value");
        }

    }
}
