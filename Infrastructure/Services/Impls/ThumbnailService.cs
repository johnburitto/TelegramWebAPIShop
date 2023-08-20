using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Impls
{
    public class ThumbnailService : IThumbnailService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ThumbnailService(AppDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Thumbnail> CreateAsync(ThumbnailCreateDto dto)
        {
            var thumbnail = _mapper.Map<Thumbnail>(dto);

            await _context.Thumbnails.AddAsync(thumbnail);
            await _context.SaveChangesAsync();

            return thumbnail;
        }

        public async Task DeleteAsync(Thumbnail entity)
        {
            _context.Thumbnails.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Thumbnail>> GetAllAsync()
        {
            return _context.Thumbnails.Include(thumbnail => thumbnail.Product).ToListAsync();
        }

        public Task<Thumbnail?> GetByIdAsync(int id)
        {
            return _context.Thumbnails.Where(thumbnail => thumbnail.Id == id)
                .Include(thumbnail => thumbnail.Product)
                .FirstOrDefaultAsync();

        }

        public async Task<Thumbnail> UpdateAsync(ThumbnailUpdateDto dto)
        {
            var thumbnail = await GetByIdAsync(dto.Id) ?? throw new ArgumentNullException($"There is no thumbnail with Id {dto.Id}");

            _mapper.Map(dto, thumbnail);
            _context.Thumbnails.Update(thumbnail);
            await _context.SaveChangesAsync();

            return thumbnail;
        }
    }
}
