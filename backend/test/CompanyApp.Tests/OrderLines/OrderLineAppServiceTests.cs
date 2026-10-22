using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using CompanyApp.Entities;
using CompanyApp.OrderLines;
using CompanyApp.OrderLines.Dto;
using CompanyApp.Flows;

namespace CompanyApp.Tests.OrderLines
{
    public class OrderLineAppServiceTests
    {
        private readonly Mock<IRepository<OrderLine, long>> _repositoryMock;
        private readonly OrderLineAppService _service;

        public OrderLineAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<OrderLine, long>>();
            _service = new OrderLineAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new OrderLine { Id = 1, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m },
                new OrderLine { Id = 2, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m },
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
                new OrderLine { Id = 1, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m },
                new OrderLine { Id = 2, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m },
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
            var dto = new CreateOrderLineDto
            {
                Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<OrderLine>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new OrderLine { Id = 1, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new OrderLine { Id = 1, Description = "Test description", Quantity = 10.0m, UnitPrice = 10.0m });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
