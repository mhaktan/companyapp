using System;
using Xunit;
using FluentAssertions;
using CompanyApp.Entities;

namespace CompanyApp.Tests.Companys
{
    public class CompanyEntityTests
    {
        [Fact]
        public void Company_ShouldBeCreatable()
        {
            // Act
            var entity = new Company();

            // Assert
            entity.Should().NotBeNull();
        }

        [Fact]
        public void Company_ShouldHaveDefaultValues()
        {
            // Act
            var entity = new Company();

            // Assert
            entity.Id.Should().Be(default(long));

        }

        [Fact]
        public void Company_Name_ShouldAcceptValue()
        {
            var entity = new Company { Name = "Test Value" };
            entity.Name.Should().Be("Test Value");
        }

    }
}
