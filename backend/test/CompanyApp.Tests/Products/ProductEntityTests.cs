using System;
using Xunit;
using FluentAssertions;
using CompanyApp.Entities;

namespace CompanyApp.Tests.Products
{
    public class ProductEntityTests
    {
        [Fact]
        public void Product_ShouldBeCreatable()
        {
            // Act
            var entity = new Product();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Product_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Product();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Product_Name_ShouldAcceptValue()
        {
            var entity = new Product { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
