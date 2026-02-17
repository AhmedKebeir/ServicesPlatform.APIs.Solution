using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public enum ReviewAuthorType
    {
        [EnumMember(Value = "User")]
        User,
        [EnumMember(Value = "Technician")]
        Technician 
    }
}
