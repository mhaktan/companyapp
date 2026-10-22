namespace CompanyApp.Authorization
{
    public static class PermissionNames
    {
        public const string Pages = "Pages";

        // Company
        public const string Company_Read = "Company.Read";
        public const string Company_Create = "Company.Create";
        public const string Company_Update = "Company.Update";
        public const string Company_Delete = "Company.Delete";

        // Product
        public const string Product_Read = "Product.Read";
        public const string Product_Create = "Product.Create";
        public const string Product_Update = "Product.Update";
        public const string Product_Delete = "Product.Delete";

        // Order
        public const string Order_Read = "Order.Read";
        public const string Order_Create = "Order.Create";
        public const string Order_Update = "Order.Update";
        public const string Order_Delete = "Order.Delete";
        public const string Order_ChangeStatus = "Order.ChangeStatus";

        // OrderLine
        public const string OrderLine_Read = "OrderLine.Read";
        public const string OrderLine_Create = "OrderLine.Create";
        public const string OrderLine_Update = "OrderLine.Update";
        public const string OrderLine_Delete = "OrderLine.Delete";

        // RBAC management
        public const string AppUser_Read = "AppUser.Read";
        public const string AppRole_Read = "AppRole.Read";
        public const string AppUser_Create = "AppUser.Create";
        public const string AppRole_Create = "AppRole.Create";
        public const string AppUser_Update = "AppUser.Update";
        public const string AppRole_Update = "AppRole.Update";
        public const string AppUser_Delete = "AppUser.Delete";
        public const string AppRole_Delete = "AppRole.Delete";
        public const string AppRole_AssignPermissions = "AppRole.AssignPermissions";

    }
}
