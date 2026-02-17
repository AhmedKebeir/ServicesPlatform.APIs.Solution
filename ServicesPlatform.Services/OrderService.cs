using ServicesPlatform.Core;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string title,string userId, int technicianId, int departmentId, string description,string phoneNumber, OrderAddress address, ICollection<string> imageUrls)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = new Order(title,userId, technicianId, departmentId, description,phoneNumber, address, imageUrls);

            await orderRepo.AddAsync(order);
            await _unitOfWork.CompleteAsync();
            return order;
        }
    }
}
