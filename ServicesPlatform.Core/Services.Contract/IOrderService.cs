using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string title,string userId, int technicianId, int departmentId, string description,string phoneNumber, OrderAddress address, ICollection<string> imageUrls);
    }
}
