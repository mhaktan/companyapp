using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using CompanyApp.Entities;
using CompanyApp.Orders.Dto;
using CompanyApp.Analytics.Dto;
using CompanyApp.StateMachine.Dto;
using CompanyApp.OrderLines.Dto;
using CompanyApp.Authorization;
using CompanyApp.Flows;

namespace CompanyApp.Orders
{
    public class OrderAppService : AsyncCrudAppService<
        Order,
        OrderDto,
        long,
        PagedOrderResultRequestDto,
        CreateOrderDto,
        OrderDto>,
        IOrderAppService
    {
        private readonly IRepository<StatusChangeLog, long> _statusChangeLogRepo;
        private readonly IFlowEngine _flowEngine;

        public OrderAppService(IRepository<Order, long> repository, IFlowEngine flowEngine, IRepository<StatusChangeLog, long> statusChangeLogRepo)
            : base(repository)
        {
            _flowEngine = flowEngine;
            _statusChangeLogRepo = statusChangeLogRepo;
            // Claim-based authorization (JwtPermissionChecker reads JWT "permission" claims)
            GetPermissionName = PermissionNames.Order_Read;
            GetAllPermissionName = PermissionNames.Order_Read;
            CreatePermissionName = PermissionNames.Order_Create;
            UpdatePermissionName = PermissionNames.Order_Update;
            DeletePermissionName = PermissionNames.Order_Delete;
        }

        protected override IQueryable<Order> CreateFilteredQuery(PagedOrderResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Id.ToString().Contains(input.Keyword) ||
                    (x.OrderNo != null && x.OrderNo.Contains(input.Keyword)) ||
                    (x.Notes != null && x.Notes.Contains(input.Keyword)))
                .WhereIf(!input.OrderNo.IsNullOrWhiteSpace(), x => x.OrderNo != null && x.OrderNo.Contains(input.OrderNo))
                .WhereIf(!input.Notes.IsNullOrWhiteSpace(), x => x.Notes != null && x.Notes.Contains(input.Notes))
                .WhereIf(input.OrderDate.HasValue, x => x.OrderDate == input.OrderDate.Value)
                .WhereIf(input.DeliveryDate.HasValue, x => x.DeliveryDate == input.DeliveryDate.Value)
                .WhereIf(input.DiscountRate.HasValue, x => x.DiscountRate == input.DiscountRate.Value)
                .WhereIf(input.VatRate.HasValue, x => x.VatRate == input.VatRate.Value)
                .WhereIf(input.Subtotal.HasValue, x => x.Subtotal == input.Subtotal.Value)
                .WhereIf(input.DiscountAmount.HasValue, x => x.DiscountAmount == input.DiscountAmount.Value)
                .WhereIf(input.VatAmount.HasValue, x => x.VatAmount == input.VatAmount.Value)
                .WhereIf(input.GrandTotal.HasValue, x => x.GrandTotal == input.GrandTotal.Value)
                .WhereIf(input.Status.HasValue, x => x.Status == (OrderStatus)input.Status.Value)
                .WhereIf(input.OrderDateFrom.HasValue, x => x.OrderDate >= input.OrderDateFrom.Value)
                .WhereIf(input.OrderDateTo.HasValue, x => x.OrderDate <= input.OrderDateTo.Value)
                .WhereIf(input.DeliveryDateFrom.HasValue, x => x.DeliveryDate >= input.DeliveryDateFrom.Value)
                .WhereIf(input.DeliveryDateTo.HasValue, x => x.DeliveryDate <= input.DeliveryDateTo.Value)
                .WhereIf(input.DiscountRateFrom.HasValue, x => x.DiscountRate >= input.DiscountRateFrom.Value)
                .WhereIf(input.DiscountRateTo.HasValue, x => x.DiscountRate <= input.DiscountRateTo.Value)
                .WhereIf(input.VatRateFrom.HasValue, x => x.VatRate >= input.VatRateFrom.Value)
                .WhereIf(input.VatRateTo.HasValue, x => x.VatRate <= input.VatRateTo.Value)
                .WhereIf(input.SubtotalFrom.HasValue, x => x.Subtotal >= input.SubtotalFrom.Value)
                .WhereIf(input.SubtotalTo.HasValue, x => x.Subtotal <= input.SubtotalTo.Value)
                .WhereIf(input.DiscountAmountFrom.HasValue, x => x.DiscountAmount >= input.DiscountAmountFrom.Value)
                .WhereIf(input.DiscountAmountTo.HasValue, x => x.DiscountAmount <= input.DiscountAmountTo.Value)
                .WhereIf(input.VatAmountFrom.HasValue, x => x.VatAmount >= input.VatAmountFrom.Value)
                .WhereIf(input.VatAmountTo.HasValue, x => x.VatAmount <= input.VatAmountTo.Value)
                .WhereIf(input.GrandTotalFrom.HasValue, x => x.GrandTotal >= input.GrandTotalFrom.Value)
                .WhereIf(input.GrandTotalTo.HasValue, x => x.GrandTotal <= input.GrandTotalTo.Value)
                .WhereIf(!input.StatusIn.IsNullOrWhiteSpace(), x => input.StatusIn
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => (OrderStatus?)int.Parse(v.Trim()))
                    .Contains(x.Status))
                .WhereIf(input.StatusNot.HasValue, x => x.Status != (OrderStatus)input.StatusNot.Value)
                .WhereIf(input.CompanyId.HasValue, x => x.CompanyId == input.CompanyId.Value);
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            // Baslangic disi bir durumla olusturma yalniz tek bir gecerli gecisle olabilir
            // (onay adimlarinin "Approve" gecisleri haric) — aksi halde onay atlanirdi.
            CheckCreatePermission();
            if (input.Status != (int)OrderStatus.Draft)
                ValidateStatusTransition(OrderStatus.Draft, (OrderStatus)input.Status);
            var result = await base.CreateAsync(input);
            await _flowEngine.TriggerAsync("on-create", "Order", result);

            // Frontend creates records with status pre-set without going through ChangeStatusAsync,
            // so mirror on-field-change here whenever the initial status isn't the default. Otherwise
            // status-driven flows (e.g. approval) never fire on plain Create.
            if (result.Status != (int)OrderStatus.Draft)
                await _flowEngine.TriggerAsync("on-field-change", "Order", result);
            return result;
        }

        public override async Task<OrderDto> UpdateAsync(OrderDto input)
        {
            // State machine: validate status transition + log
            var existing = await Repository.GetAsync(input.Id);
            var statusChanged = (int)existing.Status != input.Status;
            if (statusChanged)
            {
                var fromStatus = existing.Status.ToString();
                var toStatus = ((OrderStatus)input.Status).ToString();
                ValidateStatusTransition(existing.Status, (OrderStatus)input.Status);

                // Log status change
                await _statusChangeLogRepo.InsertAsync(new StatusChangeLog
                {
                    EntityType = "Order",
                    EntityId = input.Id.ToString(),
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Action = "Update",
                    ChangedByUserId = AbpSession.UserId
                });
            }

            var result = await base.UpdateAsync(input);
            await _flowEngine.TriggerAsync("on-update", "Order", result);

            // Frontend updates status via plain UpdateAsync (not ChangeStatusAsync) — fire
            // on-field-change so status-driven flows pick up the transition.
            if (statusChanged)
                await _flowEngine.TriggerAsync("on-field-change", "Order", result);
            return result;
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            await base.DeleteAsync(input);
            await _flowEngine.TriggerAsync("on-delete", "Order", new { Id = input.Id });
        }

        // Onay adimini tamamlayan rol genelde Update degil ChangeStatus yetkisine sahip olur;
        // RequireAllPermissions=false ile ikisinden biri yeterli (geriye donuk uyumlu).
        [Abp.Authorization.AbpAuthorize(PermissionNames.Order_ChangeStatus, PermissionNames.Order_Update, RequireAllPermissions = false)]
        public async Task<OrderDto> ChangeStatusAsync(long id, ChangeStatusInput input)
        {
            return await ApplyStatusChangeAsync(id, input);
        }

        // ABP dinamik API'si attribute'u implementasyonda arar (arayuzdeki yetmez)
        [Abp.Application.Services.RemoteService(false)]
        public async Task<OrderDto> ApplyApprovalTransitionAsync(long id, ChangeStatusInput input)
            => await ApplyStatusChangeAsync(id, input);

        private async Task<OrderDto> ApplyStatusChangeAsync(long id, ChangeStatusInput input)
        {
            var entity = await Repository.GetAsync(id);
            var currentStatus = entity.Status.ToString();

            // Find valid transition
            var transitions = new (string From, string To, string Action, bool Readonly)[]
            {
            ("Draft", "Confirmed", "Confirm", false),
            ("*", "Cancelled", "Cancel", false)
            };

            var transition = transitions.FirstOrDefault(t =>
                (t.From == "*" || t.From == currentStatus) && t.Action == input.Action);

            if (transition == default)
                throw new Abp.UI.UserFriendlyException($"Invalid action '{input.Action}' from status '{currentStatus}'");

            // Validate required fields per transition
            // No required fields for any transition
            // Bu gecisler icin bagli kayit on kosulu yok

            var fromStatus = currentStatus;

            // Apply new status
            entity.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), transition.To);

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            // No approval records to cancel (approvals not enabled for this project)

            // Log status change
            await _statusChangeLogRepo.InsertAsync(new Entities.StatusChangeLog
            {
                EntityType = "Order",
                EntityId = id.ToString(),
                FromStatus = fromStatus,
                ToStatus = transition.To,
                Action = input.Action,
                Comment = input.ActionData != null && input.ActionData.ContainsKey("comment") ? input.ActionData["comment"] : null,
                ChangedByUserId = AbpSession.UserId
            });

            var result = MapToEntityDto(entity);

            // Trigger flow: on-status-change (always)
            await _flowEngine.TriggerAsync("on-field-change", "Order", result);

            return result;
        }

        private void ValidateStatusTransition(OrderStatus from, OrderStatus to)
        {
            var allowed = new (string From, string To)[]
            {
                ("Draft", "Confirmed"),
                ("*", "Cancelled")
            };

            var isValid = allowed.Any(t =>
                (t.From == "*" || t.From == from.ToString()) &&
                t.To == to.ToString());

            if (!isValid)
                throw new Abp.UI.UserFriendlyException($"Invalid status transition from {from} to {to}");
        }
        [Abp.Authorization.AbpAuthorize(PermissionNames.Order_Read)]
        public List<GroupCountDto> GetGroupedCount(OrderGroupedCountInput input)
        {
            // Whitelist — istemciden gelen alan adı doğrudan sorguya girmez.
            var allowed = new[] { "Status", "CompanyId" };
            if (input.GroupBy == null || !allowed.Contains(input.GroupBy))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Gruplanabilir alan degil: {input.GroupBy}. Izin verilenler: {string.Join(", ", allowed)}");
            }

            var query = CreateFilteredQuery(input);

            switch (input.GroupBy)
            {
                case "Status":
                    return query
                        .GroupBy(x => x.Status)
                        .Select(g => new GroupCountDto
                        {
                            Key = ((int)g.Key).ToString(),
                            Label = g.Key.ToString(),
                            Count = g.Count(),
                        })
                        .ToList();
                case "CompanyId":
                    return query
                        .GroupBy(x => new { Key = x.CompanyId, Label = x.Company == null ? null : x.Company.Name })
                        .Select(g => new GroupCountDto
                        {
                            Key = g.Key.Key.ToString(),
                            Label = g.Key.Label ?? "(bos)",
                            Count = g.Count(),
                        })
                        .ToList();
                default:
                    return new List<GroupCountDto>();
            }
        }

        [Abp.Authorization.AbpAuthorize(PermissionNames.Order_Read)]
        public decimal? GetStats(OrderStatsInput input)
        {
            var query = CreateFilteredQuery(input);

            if (input.Aggregate == "avgDayDiff")
            {
                var allowedDates = new[] { "OrderDate", "DeliveryDate" };
                if (!allowedDates.Contains(input.FromField) || !allowedDates.Contains(input.ToField))
                {
                    throw new Abp.UI.UserFriendlyException("avgDayDiff icin gecerli iki tarih alani gerekli.");
                }
                switch (input.FromField + "|" + input.ToField)
                {
                    case "OrderDate|DeliveryDate":
                    {
                        var pairsOrderDateDeliveryDate = query
                            .Where(x => x.DeliveryDate != null)
                            .Select(x => new { A = x.OrderDate, B = x.DeliveryDate.Value })
                            .ToList();
                        if (pairsOrderDateDeliveryDate.Count == 0) return null;
                        return (decimal)pairsOrderDateDeliveryDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    case "DeliveryDate|OrderDate":
                    {
                        var pairsDeliveryDateOrderDate = query
                            .Where(x => x.DeliveryDate != null)
                            .Select(x => new { A = x.DeliveryDate.Value, B = x.OrderDate })
                            .ToList();
                        if (pairsDeliveryDateOrderDate.Count == 0) return null;
                        return (decimal)pairsDeliveryDateOrderDate.Average(p => (p.B - p.A).TotalDays);
                    }
                    default: return null;
                }
            }

            var allowedNumeric = new[] { "DiscountRate", "VatRate", "Subtotal", "DiscountAmount", "VatAmount", "GrandTotal" };
            if (!allowedNumeric.Contains(input.Field))
            {
                throw new Abp.UI.UserFriendlyException(
                    $"Toplanabilir alan degil: {input.Field}. Izin verilenler: {string.Join(", ", allowedNumeric)}");
            }
            switch (input.Field)
            {
                        case "DiscountRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.DiscountRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.DiscountRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.DiscountRate)
                            : query.Average(x => (decimal?)x.DiscountRate);
                        case "VatRate": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.VatRate)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.VatRate)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.VatRate)
                            : query.Average(x => (decimal?)x.VatRate);
                        case "Subtotal": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.Subtotal)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.Subtotal)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.Subtotal)
                            : query.Average(x => (decimal?)x.Subtotal);
                        case "DiscountAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.DiscountAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.DiscountAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.DiscountAmount)
                            : query.Average(x => (decimal?)x.DiscountAmount);
                        case "VatAmount": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.VatAmount)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.VatAmount)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.VatAmount)
                            : query.Average(x => (decimal?)x.VatAmount);
                        case "GrandTotal": return input.Aggregate == "sum" ? query.Sum(x => (decimal?)x.GrandTotal)
                            : input.Aggregate == "min" ? query.Min(x => (decimal?)x.GrandTotal)
                            : input.Aggregate == "max" ? query.Max(x => (decimal?)x.GrandTotal)
                            : query.Average(x => (decimal?)x.GrandTotal);
                        default: return null;
            }
        }

        /// <summary>
        /// Rapor verisi — kok kayit ve alt koleksiyonlar TEK yanitta.
        /// PDF sablonu template basina tek apiBinding kullaniyor.
        /// </summary>
        [Abp.Authorization.AbpAuthorize(PermissionNames.Order_Read)]
        public async Task<OrderReportDto> GetReportData(long id)
        {
            var root = await Repository.GetAll()
                .Include(x => x.OrderLines)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (root == null)
                throw new Abp.UI.UserFriendlyException($"Kayit bulunamadi: {id}");

            return new OrderReportDto
            {
                Data = ObjectMapper.Map<OrderDto>(root),
                OrderLines = ObjectMapper.Map<List<OrderLineDto>>(
                    root.OrderLines == null ? new List<OrderLine>() : root.OrderLines.ToList()),
            };
        }

    }
}
