﻿using Microsoft.Extensions.DependencyInjection;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Business.Repositories;
using NaturalAndNutritious.Business.Services;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Services.AdminPanelServices;

namespace NaturalAndNutritious.Business.Extensions
{
    public static class BusinessServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IShipperRepository, ShipperRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IOrderService, OrderService>();
        }
    }
}
