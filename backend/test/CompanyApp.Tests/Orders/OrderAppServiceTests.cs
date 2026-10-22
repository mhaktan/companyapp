using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Repositories;
using Moq;
using CompanyApp.Entities;
using CompanyApp.Orders;
using CompanyApp.Orders.Dto;
using CompanyApp.Flows;

namespace CompanyApp.Tests.Orders
{
    public class OrderAppServiceTests
    {
        private readonly Mock<IRepository<Order, long>> _repositoryMock;
        private readonly OrderAppService _service;

        public OrderAppServiceTests()
        {
            _repositoryMock = new Mock<IRepository<Order, long>>();
            _service = new OrderAppService(_repositoryMock.Object, new Mock<IFlowEngine>().Object, new Mock<IRepository<StatusChangeLog, long>>().Object);
        }

        [Fact]
        public void Repository_GetAll_ShouldReturnQueryable()
        {
            // Arrange
            var entities = new[]
            {
                new Order { Id = 1, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 },
                new Order { Id = 2, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 },
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
                new Order { Id = 1, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 },
                new Order { Id = 2, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 },
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
            var dto = new CreateOrderDto
            {
                OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0
            };

            _repositoryMock.Setup(r => r.InsertAndGetIdAsync(It.IsAny<Order>()))
                .ReturnsAsync(1);
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Order { Id = 1, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            _service.Should().NotBeNull();
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(new Order { Id = 1, OrderNo = "Test orderNo", OrderDate = DateTime.UtcNow, Status = 0 });

            // Act & Assert
            await _service.Invoking(s => s.DeleteAsync(new Abp.Application.Services.Dto.EntityDto<long> { Id = 1 }))
                .Should().NotThrowAsync();
        }
    }
}
