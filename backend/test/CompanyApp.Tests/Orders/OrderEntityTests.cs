using System;
using Xunit;
using FluentAssertions;
using CompanyApp.Entities;

namespace CompanyApp.Tests.Orders
{
    public class OrderEntityTests
    {
        [Fact]
        public void Order_ShouldBeCreatable()
        {
            // Act
            var entity = new Order();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Order_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Order();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Order_OrderNo_ShouldAcceptValue()
        {
            var entity = new Order { OrderNo = "Test Value" };
            entity.OrderNo.Should().Be("Test Value");
        }

    }
}
