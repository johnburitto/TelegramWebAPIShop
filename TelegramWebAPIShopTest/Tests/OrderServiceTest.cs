using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Impls;
using Moq;
using Moq.EntityFrameworkCore;
using TelegramWebAPIShopTest.Profiles;

namespace TelegramWebAPIShopTest.Tests
{
    public class OrderServiceTest
    {
        private readonly Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<Mapper> _orderMapper = new Mock<Mapper>(new MapperConfiguration(configuration => configuration.AddProfile(new OrderProfile())));
        private readonly Mock<Mapper> _productMapper = new Mock<Mapper>(new MapperConfiguration(configuration => configuration.AddProfile(new ProductProfile())));
        private readonly Mock<ProductService> _productService;
        private readonly OrderService _underTest;

        public OrderServiceTest()
        {
            _productService = new Mock<ProductService>(_context.Object, _productMapper.Object);
            _underTest = new OrderService(_context.Object, _productService.Object, _orderMapper.Object);
        }

        [Fact]
        public async void GetAllAsyncTest_NormalFlow()
        {
            // Given
            var orders = new List<Order>()
            {
                new Order
                {
                    Id = 1,
                    UserTelegramId = "80157223",
                    Name = "Some name",
                    Phone = "Some phone",
                    Address = "Some address"
                },
                new Order
                {
                    Id = 2,
                    UserTelegramId = "01522431",
                    Name = "Some other name",
                    Phone = "Some other phone",
                    Address = "Some other address"
                }
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(orders);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result);

            foreach (var el in result)
            {
                Assert.NotNull(el);
                Assert.IsType<Order>(el);
                Assert.NotEqual(0, el.Id);
                Assert.NotNull(el.UserTelegramId);
                Assert.NotNull(el.Name);
                Assert.NotNull(el.Phone);
                Assert.NotNull(el.Address);
            }

            Assert.Equal(orders.Count, result.Count);
            Assert.Equal(orders, result);
        }

        [Fact]
        public async void GetAllAsyncTest_EmptyDatabase()
        {
            // Given
            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { });

            // When
            var result = await _underTest.GetAllAsync();

            //Then
            Assert.NotNull(result);
            Assert.IsType<List<Order>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async void GetByIdAsyncTest_NormalFlow()
        {
            // Given
            var order = new Order
            {
                Id = 1,
                UserTelegramId = "43243548",
                Name = "Some name",
                Phone = "Some phone",
                Address = "Some address"
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { order });

            // When
            var result = await _underTest.GetByIdAsync(1);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Order>(result);
            Assert.Equal(order, result);
            Assert.Equal(order.Id, result.Id);
            Assert.Equal(order.UserTelegramId, result.UserTelegramId);
            Assert.Equal(order.Name, result.Name);
            Assert.Equal(order.Phone, result.Phone);
            Assert.Equal(order.Address, result.Address);
        }

        [Fact]
        public async void GetByIdAsyncTest_TryGetObjectWithIdThatDoesNotExist()
        {
            // Given
            var orders = new List<Order>()
            {
                new Order
                {
                    Id = 1,
                    UserTelegramId = "80157223",
                    Name = "Some name",
                    Phone = "Some phone",
                    Address = "Some address"
                },
                new Order
                {
                    Id = 2,
                    UserTelegramId = "01522431",
                    Name = "Some other name",
                    Phone = "Some other phone",
                    Address = "Some other address"
                }
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(orders);

            // When
            var result = await _underTest.GetByIdAsync(41);

            // Then
            Assert.Null(result);
            Assert.DoesNotContain(result, orders);
        }

        [Fact]
        public async void CreateAsyncTest_NormalFlow()
        {
            // Given
            var dto = new OrderCreateDto
            {
                UserTelegramId = "54022453",
                Name = "Some name",
                Phone = "Some phone",
                Address = "Some address",
                ProductsIds = new List<int> { 1, 90, 3, 567 }
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.CreateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Order>(result);
            Assert.Equal(dto.UserTelegramId, result.UserTelegramId);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Phone, result.Phone);
            Assert.Equal(dto.Address, result.Address);
            _context.Verify(_ => _.Orders.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_NormalFlow()
        {
            // Given
            var order = new Order
            {
                Id = 52,
                UserTelegramId = "54022453",
                Name = "Some name",
                Phone = "Some phone",
                Address = "Some address"
            };
            var dto = new OrderUpdateDto
            {
                Id = 52,
                UserTelegramId = "54022453",
                Name = "Some other name",
                Phone = "Some other phone",
                Address = "Some other address",
                ProductsIds = new List<int> { 1, 90, 3, 567 }
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { order });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.UpdateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Order>(result);
            Assert.Equal(dto.UserTelegramId, result.UserTelegramId);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Phone, result.Phone);
            Assert.Equal(dto.Address, result.Address);
            _context.Verify(_ => _.Orders.Update(It.IsAny<Order>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_TryUpdateObjectWithIdThatDoesNotExist()
        {
            // Given
            var dto = new OrderUpdateDto
            {
                Id = 52,
                UserTelegramId = "54022453",
                Name = "Some other name",
                Phone = "Some other phone",
                Address = "Some other address",
                ProductsIds = new List<int> { 1, 90, 3, 567 }
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { });

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.UpdateAsync(dto));

            // Then
            Assert.Equal($"Value cannot be null. (Parameter 'There is no order with Id {dto.Id}')", result.Message);
        }

        [Fact]
        public async void DeleteAsyncTest_NormalFlow()
        {
            // Given
            var order = new Order
            {
                Id = 52,
                UserTelegramId = "54022453",
                Name = "Some name",
                Phone = "Some phone",
                Address = "Some address"
            };

            _context.Setup(_ => _.Orders).ReturnsDbSet(new List<Order> { order });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            await _underTest.DeleteAsync(order);

            // Then
            _context.Verify(_ => _.Orders.Remove(It.IsAny<Order>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
