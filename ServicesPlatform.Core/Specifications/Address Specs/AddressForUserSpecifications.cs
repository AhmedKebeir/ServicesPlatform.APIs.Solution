using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Address_Spec
{
    public class AddressForUserSpecifications : BaseSpecifications<Address>
    {
        public AddressForUserSpecifications(int id, string userId)
            : base(a => a.UserId == userId && a.Id == id)
        {

        }
    }
}
