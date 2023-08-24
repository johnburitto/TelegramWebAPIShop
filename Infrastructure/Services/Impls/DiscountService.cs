using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Impls
{
    public class DiscountService : IDiscountService
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public DiscountService(AppDbContext context, IProductService productService, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Discount> CreateAsync(DiscountCreateDto dto)
        {
            var discount = _mapper.Map<Discount>(dto);

            for (int i = 0; i < dto.ProductsIds?.Count; i++)
            {
                discount.Products?.Add(await _productService.GetByIdAsync(dto.ProductsIds[i]) ?? throw new ArgumentNullException($"There is no product with Id {dto.ProductsIds[i]}"));
            }

            await _context.Discounts.AddAsync(discount);
            await _context.SaveChangesAsync();

            return discount;
        }

        public async Task DeleteAsync(Discount entity)
        {
            _context.Discounts.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Discount>> GetAllAsync()
        {
            return _context.Discounts.Include(discount => discount.Products).ToListAsync();
        }

        public Task<Discount?> GetByIdAsync(int id)
        {
            return _context.Discounts
                .Where(discount => discount.Id == id)
                .Include(discount => discount.Products)
                .FirstOrDefaultAsync();
        }

        public async Task<Discount> UpdateAsync(DiscountUpdateDto dto)
        {
            var discount = await GetByIdAsync(dto.Id) ?? throw new ArgumentNullException($"There is no discount with Id {dto.Id}");

            _mapper.Map(dto, discount);

            for (int i = 0; i < dto.ProductsIds?.Count; i++)
            {
                discount.Products?.Add(await _productService.GetByIdAsync(dto.ProductsIds[i]) ?? throw new ArgumentNullException($"There is no product with Id {dto.ProductsIds[i]}"));
            }

            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();

            return discount;
        }
    }
}
