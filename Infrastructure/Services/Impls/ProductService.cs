﻿using AutoMapper;
using Core.Dtos.Create;
using Core.Dtos.Update;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Impls
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Product> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteAsync(Product entity)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<Product>> GetAllAsync()
        {
            return _context.Products
                .Include(product => product.Thumbnails)
                .Include(product => product.Orders)
                .Include(product => product.Discounts.Where(discount => discount.Status == DiscountStatus.Active))
                .ToListAsync();
        }

        public virtual Task<Product?> GetByIdAsync(int id)
        {
            return _context.Products
                .Where(product => product.Id == id)
                .Include(product => product.Thumbnails)
                .Include(product => product.Orders)
                .Include(product => product.Discounts.Where(discount => discount.Status == DiscountStatus.Active))
                .FirstOrDefaultAsync();
        }

        public async Task<Product> TryGetNextAsync(int id)
        {
            var products = await GetAllAsync();
            var index = products.IndexOf(products.Find(product => product.Id == id) ?? throw new Exception($"There isn't product with id {id}"));

            return index >= products.Count - 1 ? products[products.Count - 1] : products[index + 1];
        }

        public async Task<Product> TryGetPreviousAsync(int id)
        {
            var products = await GetAllAsync();
            var index = products.IndexOf(products.Find(product => product.Id == id) ?? throw new Exception($"There isn't product with id {id}"));

            return index <= 0 ? products[0] : products[index - 1];
        }

        public async Task<Product> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await GetByIdAsync(dto.Id) ?? throw new ArgumentNullException($"There is no product with Id {dto.Id}");

            _mapper.Map(dto, product);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}
