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
    public class ProductServiceTest
    {
        private readonly Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<Mapper> _mapper = new Mock<Mapper>(new MapperConfiguration(configuration => configuration.AddProfile(new ProductProfile())));
        private readonly ProductService _underTest;

        public ProductServiceTest()
        {
            _underTest = new ProductService(_context.Object, _mapper.Object);
        }

        [Fact]
        public async void GetAllAsyncTest_NormalFlow()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<Product>>(result);

            foreach (var el in result)
            {
                Assert.NotNull(el);
                Assert.IsType<Product>(el);
                Assert.NotEqual(0, el.Id);
                Assert.NotNull(el.Name);
                Assert.NotNull(el.Description);
                Assert.True(el.Price >= 0, "Expected Price to be greater or equal 0.");
            }

            Assert.Equal(products.Count, result.Count);
            Assert.Equal(products, result);
        }

        [Fact]
        public async void GetAllAsyncTest_EmptyDatabase()
        {
            // Given
            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { });

            // When
            var result = await _underTest.GetAllAsync();

            //Then
            Assert.NotNull(result);
            Assert.IsType<List<Product>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async void GetByIdAsyncTest_NormalFlow()
        {
            // Given
            var product = new Product
            {
                Id = 1,
                Name = "Some name",
                Description = "Some description",
                Price = 100
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { product });

            // When
            var result = await _underTest.GetByIdAsync(1);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Equal(product, result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Price, result.Price);
        }

        [Fact]
        public async void GetByIdAsyncTest_TryGetObjectWithIdThatDoesNotExist()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.GetByIdAsync(41);

            // Then
            Assert.Null(result);
            Assert.DoesNotContain(result, products);
        }

        [Fact]
        public async void CreateAsyncTest_NormalFlow()
        {
            // Given
            var dto = new ProductCreateDto
            {
                Name = "Some name",
                Description = "Some description",
                Price = 100
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.CreateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Price, result.Price);
            Assert.True(result.Price >= 0, "Expected Price to be greater or equal 0.");
            _context.Verify(_ => _.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_NormalFlow()
        {
            // Given
            var product = new Product
            {
                Id = 52,
                Name = "Some name",
                Description = "Somde description",
                Price = 100
            };
            var dto = new ProductUpdateDto
            {
                Id = 52,
                Name = "Some other name",
                Description = "Somde other description",
                Price = 100
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { product });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.UpdateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Description, result.Description);
            Assert.Equal(dto.Price, result.Price);
            Assert.True(result.Price >= 0, "Expected Price to be greater or equal 0.");
            _context.Verify(_ => _.Products.Update(It.IsAny<Product>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void UpdateAsyncTest_TryUpdateObjectWithIdThatDoesNotExist()
        {
            // Given
            var dto = new ProductUpdateDto
            {
                Id = 52,
                Name = "Some other name",
                Description = "Somde other description",
                Price = 100
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { });

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.UpdateAsync(dto));

            // Then
            Assert.Equal($"Value cannot be null. (Parameter 'There is no product with Id {dto.Id}')", result.Message);
        }

        [Fact]
        public async void DeleteAsyncTest_NormalFlow()
        {
            // Given
            var product  = new Product
            {
                Id = 154,
                Name = "Some name",
                Description = "Some description",
                Price = 100
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(new List<Product> { product });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            await _underTest.DeleteAsync(product);

            // Then
            _context.Verify(_ => _.Products.Remove(It.IsAny<Product>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void TryGetNextAsyncTest_NormalFlow()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.TryGetNextAsync(products[0].Id);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Contains(result, products);
            Assert.NotEqual(products[0], result);
            Assert.Equal(products[1], result);
        }

        [Fact]
        public async void TryGetNextAsyncTest_GetNextFromProductThatDoesNotExist()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };
            var id = 3;

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await Assert.ThrowsAsync<Exception>(async () => await _underTest.TryGetNextAsync(id));

            // Then
            Assert.Equal($"There isn't product with id {id}", result.Message);
        }

        [Fact]
        public async void TryGetNextAsyncTest_GetNextWhenCurrentProductIsLast()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 3,
                    Name = "Some third name",
                    Description = "Some third desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.TryGetNextAsync(products[2].Id);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Contains(result, products);
            Assert.NotEqual(products[0], result);
            Assert.NotEqual(products[1], result);
            Assert.Equal(products[2], result);
        }

        [Fact]
        public async void TryGetPreviousAsyncTest_NormalFlow()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.TryGetPreviousAsync(products[1].Id);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Contains(result, products);
            Assert.NotEqual(products[1], result);
            Assert.Equal(products[0], result);
        }

        [Fact]
        public async void TryGetPreviousAsyncTest_GetPreviousFromProductThatDoesNotExist()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some other name",
                    Description = "Some other desc",
                    Price = 100
                }
            };
            var id = 3;

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await Assert.ThrowsAsync<Exception>(async () => await _underTest.TryGetPreviousAsync(id));

            // Then
            Assert.Equal($"There isn't product with id {id}", result.Message);
        }

        [Fact]
        public async void TryGetPrevioustAsyncTest_GetPreviousWhenCurrentProductIsFirst()
        {
            // Given
            var products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 2,
                    Name = "Some name",
                    Description = "Some desc",
                    Price = 100
                },
                new Product
                {
                    Id = 3,
                    Name = "Some third name",
                    Description = "Some third desc",
                    Price = 100
                }
            };

            _context.Setup(_ => _.Products).ReturnsDbSet(products);

            // When
            var result = await _underTest.TryGetPreviousAsync(products[0].Id);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Product>(result);
            Assert.Contains(result, products);
            Assert.NotEqual(products[1], result);
            Assert.NotEqual(products[2], result);
            Assert.Equal(products[0], result);
        }
    }
}
