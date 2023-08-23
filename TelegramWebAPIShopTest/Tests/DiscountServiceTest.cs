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
    public class DiscountServiceTest
    {
        private readonly Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<Mapper> _discountMapper = new Mock<Mapper>(new MapperConfiguration(configuration => configuration.AddProfile(new DiscountProfile())));
        private readonly Mock<Mapper> _productMapper = new Mock<Mapper>(new MapperConfiguration(configuration => configuration.AddProfile(new ProductProfile())));
        private readonly Mock<ProductService> _productService;
        private readonly DiscountService _underTest;

        public DiscountServiceTest()
        {
            _productService = new Mock<ProductService>(_context.Object, _productMapper.Object);
            _underTest = new DiscountService(_context.Object, _productService.Object, _discountMapper.Object);
        }

        [Fact]
        public async void GetAllAsyncTest_NormalFlow()
        {
            // Given
            var discounts = new List<Discount>()
            {
                new Discount
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    DiscoutInPercent = 50,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                },
                new Discount
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    DiscoutInPercent = 90,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                }
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(discounts);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<Discount>>(result);

            foreach (var el in result)
            {
                Assert.NotNull(el);
                Assert.IsType<Discount>(el);
                Assert.NotEqual(0, el.Id);
                Assert.NotNull(el.Name);
                Assert.NotNull(el.Description);
                Assert.True(el.DiscoutInPercent > 0, "Expected DiscoutInPercent to be greater than 0.");
                Assert.True(el.EndDate > el.StartDate, "Expected EndDate to be greater than StartDate.");
            }

            Assert.Equal(discounts.Count, result.Count);
            Assert.Equal(discounts, result);
        }

        [Fact]
        public async void GetAllAsyncTest_EmptyDatabase()
        {
            // Given
            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { });

            // When
            var result = await _underTest.GetAllAsync();

            //Then
            Assert.NotNull(result);
            Assert.IsType<List<Discount>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async void GetByIdAsyncTest_NormalFlow()
        {
            // Given
            var discount = new Discount
            {
                Id = 1,
                Name = "Some name",
                Description = "Some description",
                DiscoutInPercent = 100,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { discount });

            // When
            var result = await _underTest.GetByIdAsync(1);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Discount>(result);
            Assert.Equal(discount, result);
            Assert.Equal(discount.Id, result.Id);
            Assert.Equal(discount.Name, result.Name);
            Assert.Equal(discount.Description, result.Description);
            Assert.Equal(discount.DiscoutInPercent, result.DiscoutInPercent);
            Assert.Equal(discount.StartDate, result.StartDate);
            Assert.Equal(discount.EndDate, result.EndDate);
        }

        [Fact]
        public async void GetByIdAsyncTest_TryGetObjectWithIdThatDoesNotExist()
        {
            // Given
            var discounts = new List<Discount>()
            {
                new Discount
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    DiscoutInPercent = 50,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                },
                new Discount
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    DiscoutInPercent = 90,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                }
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(discounts);

            // When
            var result = await _underTest.GetByIdAsync(41);

            // Then
            Assert.Null(result);
            Assert.DoesNotContain(result, discounts);
        }

        [Fact]
        public async void CreateAsyncTest_NormalFlow()
        {
            // Given
            var dto = new DiscountCreateDto
            {
                Name = "Some name",
                Description = "Some description",
                DiscoutInPercent = 34.5f,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ProductsIds = new List<int> { 1, 90, 3, 567 }
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.CreateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Discount>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.DiscoutInPercent, result.DiscoutInPercent);
            Assert.Equal(dto.StartDate, result.StartDate);
            Assert.Equal(dto.EndDate, result.EndDate);
            Assert.True(dto.DiscoutInPercent > 0, "Expected DiscoutInPercent to be greater than 0.");
            Assert.True(dto.EndDate > dto.StartDate, "Expected EndDate to be greater than StartDate.");
            _context.Verify(_ => _.Discounts.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_NormalFlow()
        {
            // Given
            var discount = new Discount
            {
                Id = 52,
                Name = "Some name",
                Description = "Somde description",
                DiscoutInPercent = 50,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };
            var dto = new DiscountUpdateDto
            {
                Id = 52,
                Name = "Some other name",
                Description = "Somde other description",
                DiscoutInPercent = 50,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                ProductsIds = new List<int> { 1, 90, 3, 567 }
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { discount });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.UpdateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Discount>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.DiscoutInPercent, result.DiscoutInPercent);
            Assert.Equal(dto.StartDate, result.StartDate);
            Assert.Equal(dto.EndDate, result.EndDate);
            Assert.True(dto.DiscoutInPercent > 0, "Expected DiscoutInPercent to be greater than 0.");
            Assert.True(dto.EndDate > dto.StartDate, "Expected EndDate to be greater than StartDate.");
            _context.Verify(_ => _.Discounts.Update(It.IsAny<Discount>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_TryUpdateObjectWithIdThatDoesNotExist()
        {
            // Given
            var dto = new DiscountUpdateDto
            {
                Id = 52,
                Name = "Some other name",
                Description = "Somde other description",
                DiscoutInPercent = 50,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { });

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.UpdateAsync(dto));

            // Then
            Assert.Equal($"Value cannot be null. (Parameter 'There is no discount with Id {dto.Id}')", result.Message);
        }

        [Fact]
        public async void DeleteAsyncTest_NormalFlow()
        {
            // Given
            var discount = new Discount
            {
                Id = 52,
                Name = "Some name",
                Description = "Somde description",
                DiscoutInPercent = 50,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };

            _context.Setup(_ => _.Discounts).ReturnsDbSet(new List<Discount> { discount });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            await _underTest.DeleteAsync(discount);

            // Then
            _context.Verify(_ => _.Discounts.Remove(It.IsAny<Discount>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
