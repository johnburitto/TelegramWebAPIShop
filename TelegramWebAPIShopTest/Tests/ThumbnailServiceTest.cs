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
    public class ThumbnailServiceTest
    {
        private readonly Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<Mapper> _mapper = new Mock<Mapper>(new MapperConfiguration(confiration => confiration.AddProfile(new ThumbnailProfile())));
        private readonly ThumbnailService _underTest;

        public ThumbnailServiceTest()
        {
            _underTest = new ThumbnailService(_context.Object, _mapper.Object);
        }

        [Fact]
        public async void GetAllAsyncTest_NormalFlow()
        {
            // Given
            var thunbnails = new List<Thumbnail>()
            {
                new Thumbnail
                {
                    Id = 1,
                    URI = "D://",
                    ProductId = 1
                },
                new Thumbnail
                {
                    Id = 113,
                    URI = "D://Hello",
                    ProductId = 5
                }
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(thunbnails);

            // When
            var result = await _underTest.GetAllAsync();

            // Then
            Assert.NotNull(result);
            Assert.IsType<List<Thumbnail>>(result);

            foreach (var el in result)
            {
                Assert.NotNull(el);
                Assert.IsType<Thumbnail>(el);
                Assert.NotEqual(0, el.Id);
                Assert.NotNull(el.URI);
                Assert.NotEmpty(el.URI);
                Assert.NotEqual(0, el.ProductId);
            }

            Assert.Equal(thunbnails.Count, result.Count);
            Assert.Equal(thunbnails, result);
        }

        [Fact]
        public async void GetAllAsyncTest_EmptyDatabase()
        {
            // Given
            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { });

            // When
            var result = await _underTest.GetAllAsync();

            //Then
            Assert.NotNull(result);
            Assert.IsType<List<Thumbnail>>(result);
            Assert.Empty(result);
        }

        [Fact]
        public async void GetByIdAsyncTest_NormalFlow()
        {
            // Given
            var thumbnail = new Thumbnail
            {
                Id = 1,
                URI = "D://helloworld.png",
                ProductId = 12
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { thumbnail });

            // When
            var result = await _underTest.GetByIdAsync(1);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Thumbnail>(result);
            Assert.Equal(thumbnail, result);
            Assert.Equal(thumbnail.Id, result.Id);
            Assert.Equal(thumbnail.URI, result.URI);
            Assert.Equal(thumbnail.ProductId, result.ProductId);
        }

        [Fact]
        public async void GetByIdAsyncTest_TryGetObjectWithIdThatDoesNotExist()
        {
            // Given
            var thunbnails = new List<Thumbnail>()
            {
                new Thumbnail
                {
                    Id = 1,
                    URI = "D://",
                    ProductId = 1
                },
                new Thumbnail
                {
                    Id = 113,
                    URI = "D://Hello",
                    ProductId = 5
                }
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(thunbnails);

            // When
            var result = await _underTest.GetByIdAsync(41);

            // Then
            Assert.Null(result);
            Assert.DoesNotContain(result, thunbnails);
        }

        [Fact]
        public async void CreateAsyncTest_NormalFlow()
        {
            // Given
            var dto = new ThumbnailCreateDto
            {
                URI = "D:/helloworld.png",
                ProductId = 12
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.CreateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Thumbnail>(result);
            Assert.Equal(dto.URI, result.URI);
            Assert.Equal(dto.ProductId, result.ProductId);
            _context.Verify(_ => _.Thumbnails.AddAsync(It.IsAny<Thumbnail>(), It.IsAny<CancellationToken>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAsyncTest_NormalFlow()
        {
            // Given
            var thumbnail = new Thumbnail
            {
                Id = 52,
                URI = "D:/",
                ProductId = 1
            };
            var dto = new ThumbnailUpdateDto
            {
                Id = 52,
                URI = "D:/helloworld.png",
                ProductId = 1
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { thumbnail });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            var result = await _underTest.UpdateAsync(dto);

            // Then
            Assert.NotNull(result);
            Assert.IsType<Thumbnail>(result);
            Assert.Equal(dto.URI, result.URI);
            Assert.Equal(dto.ProductId, result.ProductId);
            _context.Verify(_ => _.Thumbnails.Update(It.IsAny<Thumbnail>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async void UpdateAsyncTest_TryUpdateObjectWithIdThatDoesNotExist()
        {
            // Given
            var dto = new ThumbnailUpdateDto
            {
                Id = 52,
                URI = "D:/helloworld.png",
                ProductId = 1
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { });

            // When
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _underTest.UpdateAsync(dto));

            // Then
            Assert.Equal($"Value cannot be null. (Parameter 'There is no thumbnail with Id {dto.Id}')", result.Message);
        }

        [Fact]
        public async void DeleteAsyncTest_NormalFlow()
        {
            // Given
            var thumbnail = new Thumbnail
            {
                Id = 74,
                URI = "D:/helloworld.png",
                ProductId = 1
            };

            _context.Setup(_ => _.Thumbnails).ReturnsDbSet(new List<Thumbnail> { thumbnail });
            _context.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // When
            await _underTest.DeleteAsync(thumbnail);

            // Then
            _context.Verify(_ => _.Thumbnails.Remove(It.IsAny<Thumbnail>()), Times.Once());
            _context.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
