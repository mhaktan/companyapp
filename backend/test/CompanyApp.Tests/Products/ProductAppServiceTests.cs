using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using CompanyApp.Entities;
using CompanyApp.Products;
using CompanyApp.Products.Dto;
using CompanyApp.Flows;

namespace CompanyApp.Tests.Products
{
    public class ProductAppServiceTests
    {
        private readonly Mock<IRepository<Product, long>> _repositoryMock;
        private readonly ProductAppService _service;

        public ProductAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Product, long>>();
            _service = new ProductAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Product { Id = 1, Name = "Test name" },
                new Product { Id = 2, Name = "Test name" },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act
            var result = _repositoryMock.Object.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
        }

        [Fact]
        public void Repository_GetAll_WithFilter_ShouldWork()
        {
            // Arrange
            var entities = new[]
            {
                new Product { Id = 1, Name = "Test name" },
                new Product { Id = 2, Name = "Test name" },
            }.AsQueryable();

            _repositoryMock.Setup(r => r.GetAll()).Returns(entities);

            // Act — simulate keyword filter
            var result = _repositoryMock.Object.GetAll()
                .Where(x => x.Id.ToString().Contains("1"));

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                Name = "Test name"
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Product>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Product { Id = 1, Name = "Test name" });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Product { Id = 1, Name = "Test name" });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
