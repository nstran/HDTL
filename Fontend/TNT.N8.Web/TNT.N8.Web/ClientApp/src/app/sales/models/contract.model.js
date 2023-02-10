"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ContractCostDetail = exports.ContractProductDetailProductAttributeValue = exports.ContractDetail = exports.Contract = void 0;
var Contract = /** @class */ (function () {
    function Contract() {
    }
    return Contract;
}());
exports.Contract = Contract;
var ContractDetail = /** @class */ (function () {
    function ContractDetail() {
        this.ContractDetailId = '00000000-0000-0000-0000-000000000000',
            this.VendorId = '',
            this.ContractId = '00000000-0000-0000-0000-000000000000',
            this.ProductId = '',
            this.Quantity = 0,
            this.UnitPrice = 0,
            this.CurrencyUnit = '',
            this.ExchangeRate = 1,
            this.Vat = 0,
            this.DiscountType = true,
            this.DiscountValue = 0,
            this.Description = '',
            this.OrderDetailType = 0,
            this.UnitId = '',
            this.IncurredUnit = 'CCCCC',
            this.Active = true,
            this.CreatedById = '00000000-0000-0000-0000-000000000000',
            this.CreatedDate = new Date(),
            this.UpdatedById = null,
            this.UpdatedDate = null,
            this.ContractProductDetailProductAttributeValue = [],
            this.ExplainStr = '',
            this.NameVendor = '',
            this.ProductNameUnit = '',
            this.NameMoneyUnit = '',
            this.SumAmount = 0,
            this.GuaranteeTime = 0;
        this.AmountDiscount = 0;
        this.ProductName = '';
        this.OrderNumber = 0;
        this.PriceInitial = null;
        this.IsPriceInitial = false;
    }
    return ContractDetail;
}());
exports.ContractDetail = ContractDetail;
var ContractProductDetailProductAttributeValue = /** @class */ (function () {
    function ContractProductDetailProductAttributeValue() {
        this.ContractDetailId = '00000000-0000-0000-0000-000000000000',
            this.ProductId = '',
            this.ProductAttributeCategoryId = '',
            this.ProductAttributeCategoryValueId = '',
            this.ContractProductDetailProductAttributeValueId = '00000000-0000-0000-0000-000000000000',
            this.NameProductAttributeCategoryValue = '',
            this.NameProductAttributeCategory = '';
    }
    return ContractProductDetailProductAttributeValue;
}());
exports.ContractProductDetailProductAttributeValue = ContractProductDetailProductAttributeValue;
var ContractCostDetail = /** @class */ (function () {
    function ContractCostDetail() {
        this.ContractCostDetailId = '00000000-0000-0000-0000-000000000000',
            this.CostId = '',
            this.ContractId = '00000000-0000-0000-0000-000000000000',
            this.Quantity = 0,
            this.UnitPrice = 0,
            this.CostName = '',
            this.Active = true,
            this.CreatedById = '00000000-0000-0000-0000-000000000000',
            this.CreatedDate = new Date(),
            this.UpdatedById = null,
            this.UpdatedDate = null;
    }
    return ContractCostDetail;
}());
exports.ContractCostDetail = ContractCostDetail;
//# sourceMappingURL=contract.model.js.map