using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;
using TN.TNM.DataAccess.Messages.Results.QuyTrinh;

namespace TN.TNM.Api.Controllers
{
    public class QuyTrinhController : Controller
    {
        private readonly IQuyTrinhDataAccess _iQuyTrinh;

        public QuyTrinhController(IQuyTrinhDataAccess iQuyTrinh)
        {
            _iQuyTrinh = iQuyTrinh;
        }

        [HttpPost]
        [Route("api/quytrinh/createQuyTrinh")]
        [Authorize(Policy = "Member")]
        public CreateQuyTrinhResult CreateQuyTrinh([FromBody] CreateQuyTrinhParameter request)
        {
            return this._iQuyTrinh.CreateQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/searchQuyTrinh")]
        [Authorize(Policy = "Member")]
        public SearchQuyTrinhResult SearchQuyTrinh([FromBody] SearchQuyTrinhParameter request)
        {
            return this._iQuyTrinh.SearchQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/getMasterDataSearchQuyTrinh")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchQuyTrinhResult GetMasterDataSearchQuyTrinh([FromBody] GetMasterDataSearchQuyTrinhParameter request)
        {
            return this._iQuyTrinh.GetMasterDataSearchQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/getDetailQuyTrinh")]
        [Authorize(Policy = "Member")]
        public GetDetailQuyTrinhResult GetDetailQuyTrinh([FromBody] GetDetailQuyTrinhParameter request)
        {
            return this._iQuyTrinh.GetDetailQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/updateQuyTrinh")]
        [Authorize(Policy = "Member")]
        public UpdateQuyTrinhResult UpdateQuyTrinh([FromBody] UpdateQuyTrinhParameter request)
        {
            return this._iQuyTrinh.UpdateQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/deleteQuyTrinh")]
        [Authorize(Policy = "Member")]
        public DeleteQuyTrinhResult DeleteQuyTrinh([FromBody] DeleteQuyTrinhParameter request)
        {
            return this._iQuyTrinh.DeleteQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/checkTrangThaiQuyTrinh")]
        [Authorize(Policy = "Member")]
        public CheckTrangThaiQuyTrinhResult CheckTrangThaiQuyTrinh([FromBody] CheckTrangThaiQuyTrinhParameter request)
        {
            return this._iQuyTrinh.CheckTrangThaiQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/guiPheDuyet")]
        [Authorize(Policy = "Member")]
        public GuiPheDuyetResult GuiPheDuyet([FromBody] GuiPheDuyetParameter request)
        {
            return this._iQuyTrinh.GuiPheDuyet(request);
        }

        [HttpPost]
        [Route("api/quytrinh/pheDuyet")]
        [Authorize(Policy = "Member")]
        public PheDuyetResult PheDuyet([FromBody] PheDuyetParameter request)
        {
            return this._iQuyTrinh.PheDuyet(request);
        }

        [HttpPost]
        [Route("api/quytrinh/huyYeuCauPheDuyet")]
        [Authorize(Policy = "Member")]
        public HuyYeuCauPheDuyetResult HuyYeuCauPheDuyet([FromBody] HuyYeuCauPheDuyetParameter request)
        {
            return this._iQuyTrinh.HuyYeuCauPheDuyet(request);
        }

        [HttpPost]
        [Route("api/quytrinh/tuChoi")]
        [Authorize(Policy = "Member")]
        public TuChoiResult TuChoi([FromBody] TuChoiParameter request)
        {
            return this._iQuyTrinh.TuChoi(request);
        }

        [HttpPost]
        [Route("api/quytrinh/getLichSuPheDuyet")]
        [Authorize(Policy = "Member")]
        public GetLichSuPheDuyetResult GetLichSuPheDuyet([FromBody] GetLichSuPheDuyetParameter request)
        {
            return this._iQuyTrinh.GetLichSuPheDuyet(request);
        }

        [HttpPost]
        [Route("api/quytrinh/getDuLieuQuyTrinh")]
        [Authorize(Policy = "Member")]
        public GetDuLieuQuyTrinhResult GetDuLieuQuyTrinh([FromBody] GetDuLieuQuyTrinhParameter request)
        {
            return this._iQuyTrinh.GetDuLieuQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/getMasterDataCreateQuyTrinh")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreateQuyTrinhResult GetMasterDataCreateQuyTrinh([FromBody] GetMasterDataCreateQuyTrinhParameter request)
        {
            return this._iQuyTrinh.GetMasterDataCreateQuyTrinh(request);
        }

        [HttpPost]
        [Route("api/quytrinh/checkUpdateQuyTrinh")]
        [Authorize(Policy = "Member")]
        public CheckUpdateQuyTrinhResult CheckUpdateQuyTrinh([FromBody] CheckUpdateQuyTrinhParameter request)
        {
            return this._iQuyTrinh.CheckUpdateQuyTrinh(request);
        }
    }
}
