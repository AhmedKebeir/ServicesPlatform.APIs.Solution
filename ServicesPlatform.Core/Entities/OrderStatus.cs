using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,       // العميل أنشأ الطلب ولم يتم استلامه بعد
        [EnumMember(Value = "Accepted")]
        Accepted,      // الفني قبل الطلب
        [EnumMember(Value = "OnTheWay")]
        OnTheWay,      // الفني في الطريق للعميل
        [EnumMember(Value = "InProgress")]
        InProgress,    // الفني بدأ العمل عند العميل
        [EnumMember(Value = "Finished")]
        Finished,      // الفني أنهى العمل
        [EnumMember(Value = "Cancelled")]
        Cancelled      // الطلب تم إلغاؤه (اختياري)
    }

}
