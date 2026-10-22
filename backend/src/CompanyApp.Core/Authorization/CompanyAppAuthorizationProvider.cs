using Abp.Authorization;
using Abp.Localization;

namespace CompanyApp.Authorization
{
    public class CompanyAppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.GetPermissionOrNull("Pages") ?? context.CreatePermission("Pages", L("Pages"));

            // Company
            pages.CreateChildPermission(PermissionNames.Company_Read, L("Company.Read"));
            pages.CreateChildPermission(PermissionNames.Company_Create, L("Company.Create"));
            pages.CreateChildPermission(PermissionNames.Company_Update, L("Company.Update"));
            pages.CreateChildPermission(PermissionNames.Company_Delete, L("Company.Delete"));

            // Product
            pages.CreateChildPermission(PermissionNames.Product_Read, L("Product.Read"));
            pages.CreateChildPermission(PermissionNames.Product_Create, L("Product.Create"));
            pages.CreateChildPermission(PermissionNames.Product_Update, L("Product.Update"));
            pages.CreateChildPermission(PermissionNames.Product_Delete, L("Product.Delete"));

            // Order
            pages.CreateChildPermission(PermissionNames.Order_Read, L("Order.Read"));
            pages.CreateChildPermission(PermissionNames.Order_Create, L("Order.Create"));
            pages.CreateChildPermission(PermissionNames.Order_Update, L("Order.Update"));
            pages.CreateChildPermission(PermissionNames.Order_Delete, L("Order.Delete"));
            pages.CreateChildPermission(PermissionNames.Order_ChangeStatus, L("Order.ChangeStatus"));

            // OrderLine
            pages.CreateChildPermission(PermissionNames.OrderLine_Read, L("OrderLine.Read"));
            pages.CreateChildPermission(PermissionNames.OrderLine_Create, L("OrderLine.Create"));
            pages.CreateChildPermission(PermissionNames.OrderLine_Update, L("OrderLine.Update"));
            pages.CreateChildPermission(PermissionNames.OrderLine_Delete, L("OrderLine.Delete"));

            // RBAC
            pages.CreateChildPermission(PermissionNames.AppUser_Read, L("AppUser.Read"));
            pages.CreateChildPermission(PermissionNames.AppRole_Read, L("AppRole.Read"));
            pages.CreateChildPermission(PermissionNames.AppUser_Create, L("AppUser.Create"));
            pages.CreateChildPermission(PermissionNames.AppRole_Create, L("AppRole.Create"));
            pages.CreateChildPermission(PermissionNames.AppUser_Update, L("AppUser.Update"));
            pages.CreateChildPermission(PermissionNames.AppRole_Update, L("AppRole.Update"));
            pages.CreateChildPermission(PermissionNames.AppUser_Delete, L("AppUser.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_Delete, L("AppRole.Delete"));
            pages.CreateChildPermission(PermissionNames.AppRole_AssignPermissions, L("AppRole.AssignPermissions"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CompanyAppConsts.LocalizationSourceName);
        }
    }
}
