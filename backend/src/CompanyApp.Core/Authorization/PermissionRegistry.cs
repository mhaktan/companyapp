using System.Collections.Generic;
using Abp.Dependency;

namespace CompanyApp.Authorization
{
    /// <summary>Single permission descriptor — name, group (entity), description.</summary>
    public class PermissionInfo
    {
        public string Name { get; }
        public string Group { get; }
        public string Description { get; }
        public bool IsRbac { get; }

        public PermissionInfo(string name, string group, string description, bool isRbac)
        {
            Name = name; Group = group; Description = description; IsRbac = isRbac;
        }
    }

    public interface IPermissionRegistry
    {
        IReadOnlyList<PermissionInfo> All { get; }
    }

    public class PermissionRegistry : IPermissionRegistry, ISingletonDependency
    {
        public IReadOnlyList<PermissionInfo> All { get; } = new List<PermissionInfo>
        {
            new PermissionInfo("Company.Read", "Company", "Read Company", false),
            new PermissionInfo("Company.Create", "Company", "Create Company", false),
            new PermissionInfo("Company.Update", "Company", "Update Company", false),
            new PermissionInfo("Company.Delete", "Company", "Delete Company", false),
            new PermissionInfo("Product.Read", "Product", "Read Product", false),
            new PermissionInfo("Product.Create", "Product", "Create Product", false),
            new PermissionInfo("Product.Update", "Product", "Update Product", false),
            new PermissionInfo("Product.Delete", "Product", "Delete Product", false),
            new PermissionInfo("Order.Read", "Order", "Read Order", false),
            new PermissionInfo("Order.Create", "Order", "Create Order", false),
            new PermissionInfo("Order.Update", "Order", "Update Order", false),
            new PermissionInfo("Order.Delete", "Order", "Delete Order", false),
            new PermissionInfo("Order.ChangeStatus", "Order", "Change Order status", false),
            new PermissionInfo("OrderLine.Read", "OrderLine", "Read OrderLine", false),
            new PermissionInfo("OrderLine.Create", "OrderLine", "Create OrderLine", false),
            new PermissionInfo("OrderLine.Update", "OrderLine", "Update OrderLine", false),
            new PermissionInfo("OrderLine.Delete", "OrderLine", "Delete OrderLine", false),
            new PermissionInfo("AppUser.Read", "AppUser", "Read users", true),
            new PermissionInfo("AppRole.Read", "AppRole", "Read roles", true),
            new PermissionInfo("AppUser.Create", "AppUser", "Create users", true),
            new PermissionInfo("AppRole.Create", "AppRole", "Create roles", true),
            new PermissionInfo("AppUser.Update", "AppUser", "Update users", true),
            new PermissionInfo("AppRole.Update", "AppRole", "Update roles", true),
            new PermissionInfo("AppUser.Delete", "AppUser", "Delete users", true),
            new PermissionInfo("AppRole.Delete", "AppRole", "Delete roles", true),
            new PermissionInfo("AppRole.AssignPermissions", "AppRole", "Assign permissions to roles", true),
        };
    }
}
