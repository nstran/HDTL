using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.BillSale;
using TN.TNM.BusinessLogic.Messages.Requests.BillSale;
using TN.TNM.BusinessLogic.Messages.Responses.BillSale;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.BillSale;
using TN.TNM.DataAccess.Messages.Results.BillSale;

namespace TN.TNM.Api.Controllers
{
    public class BillSaleController : ControllerBase
    {
        private readonly IBillSale _iBillSale;
        private readonly IBillSaleDataAccess _iBillSaleDataAccess;
        public BillSaleController(IBillSale iBillSale, IBillSaleDataAccess iBillSaleDataAccess)
        {
            this._iBillSale = iBillSale;
            _iBillSaleDataAccess = iBillSaleDataAccess;
        }

        [HttpPost]
        [Route("api/billSale/getMasterDataBillSaleCreateEdit")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBillSaleCreateEditResult GetMasterDataBillSaleCreateEdit([FromBody]GetMasterDataBillSaleCreateEditParameter request)
        {
            return _iBillSaleDataAccess.GetMasterDataBillSaleCreateEdit(request);
        }

        [HttpPost]
        [Route("api/billSale/addOrEditBillSale")]
        [Authorize(Policy = "Member")]
        public AddOrEditBillSaleResult AddOrEditBillSale([FromBody]AddOrEditBillSaleParameter request)
        {
            return _iBillSaleDataAccess.AddOrEditBillSale(request);
        }

        [HttpPost]
        [Route("api/billSale/searchBillSale")]
        [Authorize(Policy = "Member")]
        public SearchBillOfSaleResult SearchBillOfSale([FromBody]SearchBillOfSaleParameter request)
        {
            return _iBillSaleDataAccess.SearchBillOfSale(request);
        }

        [HttpPost]
        [Route("api/billSale/getMasterDataSearchBillSale")]
        [Authorize(Policy = "Member")]
        public GetMasterBillOfSaleResult GetMasterBillOfSale([FromBody]GetMasterBillOfSaleParameter request)
        {
            return _iBillSaleDataAccess.GetMasterBillOfSale(request);
        }

        [HttpPost]
        [Route("api/billSale/getOrderByOrderId")]
        [Authorize(Policy = "Member")]
        public GetOrderByOrderIdResult GetOrderByOrderId([FromBody]GetOrderByOrderIdParameter request)
        {
            return _iBillSaleDataAccess.GetOrderByOrderId(request);
        }

        [HttpPost]
        [Route("api/billSale/updateStatus")]
        [Authorize(Policy = "Member")]
        public UpdateStatusResult UpdateStatus([FromBody]UpdateStatusParameter request)
        {
            return _iBillSaleDataAccess.UpdateStatus(request);
        }

        [HttpPost]
        [Route("api/billSale/deleteBillSale")]
        [Authorize(Policy = "Member")]
        public DeleteBillSaleResult DeleteBillSale([FromBody]DeleteBillSaleParameter request)
        {
            return _iBillSaleDataAccess.DeleteBillSale(request);
        }
    }
}