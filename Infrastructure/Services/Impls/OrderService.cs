using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Impls
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public OrderService(AppDbContext context, IProductService productService, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Order> CreateAsync(OrderCreateDto dto)
        {
            var order = _mapper.Map<Order>(dto);

            for (int i = 0; i < dto.ProductsIds?.Count; i++)
            {
                order.Products?.Add(await _productService.GetByIdAsync(dto.ProductsIds[i]) ?? throw new ArgumentNullException($"There is no product with Id {dto.ProductsIds[i]}"));
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task DeleteAsync(Order entity)
        {
            _context.Orders.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Order>> GetAllAsync()
        {
            return _context.Orders.Include(order => order.Products).ToListAsync();
        }

        public Task<Order?> GetByIdAsync(int id)
        {
            return _context.Orders
                .Where(order => order.Id == id)
                .Include(order => order.Products)
                .FirstOrDefaultAsync();
        }

        public Task<List<Order>> GetUserOrdersAsync(string id)
        {
            return _context.Orders.Where(order => order.UserTelegramId == id).Include(order => order.Products).ToListAsync();
        }

        public async Task<Order> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await GetByIdAsync(dto.Id) ?? throw new ArgumentNullException($"There is no order with Id {dto.Id}");

            _mapper.Map(dto, order);

            for (int i = 0; i < dto.ProductsIds?.Count; i++)
            {
                order.Products?.Add(await _productService.GetByIdAsync(dto.ProductsIds[i]) ?? throw new ArgumentNullException($"There is no product with Id {dto.ProductsIds[i]}"));
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
