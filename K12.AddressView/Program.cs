using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Permission;

namespace K12.AddressView
{
    public class Program
    {
        [MainMethod()]
        static public void Main()
        {
            //依戶籍地址檢視
            FeatureAce UserPermission = FISCA.Permission.UserAcl.Current[Permissions.戶籍地址檢視];
            if (UserPermission.Executable)
                K12.Presentation.NLDPanels.Student.AddView(new AddressViewByPermanent());

            //依聯絡地址檢視
            UserPermission = FISCA.Permission.UserAcl.Current[Permissions.聯絡地址檢視];
            if (UserPermission.Executable)
                K12.Presentation.NLDPanels.Student.AddView(new AddressViewByMailing());

            Catalog detail1;
            detail1 = RoleAclSource.Instance["學生"]["檢視模式"];
            detail1.Add(new RibbonFeature(Permissions.戶籍地址檢視, "依戶籍地址檢視"));
            detail1.Add(new RibbonFeature(Permissions.聯絡地址檢視, "依聯絡地址檢視"));
        }
    }
}
