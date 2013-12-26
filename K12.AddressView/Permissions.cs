using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.AddressView
{
    class Permissions
    {
        public static string 戶籍地址檢視 { get { return "K12.AddressView.AddressViewByPermanent.cs"; } }
        public static bool 戶籍地址檢視權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[戶籍地址檢視].Executable;
            }
        }

        public static string 聯絡地址檢視 { get { return "K12.AddressView.AddressViewByMailing.cs"; } }
        public static bool 聯絡地址檢視權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[聯絡地址檢視].Executable;
            }
        }
    }
}
