using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Parameters.Asset;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using NLog.Fluent;
using NLog;
using System;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration;
using System.Net;

namespace TN.TNM.Api.Controllers
{
    public class AssetController : Controller
    {
        private readonly IAssetDataAccess _iAssetDataAccess;

        public AssetController(IAssetDataAccess iAssetDataAccess)
        {
            _iAssetDataAccess = iAssetDataAccess;
        }

        [HttpGet]
        [Route("iclock/cdata")]
        public string getCdata(GetDataMCCParameter parameter)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info($"==================== API Get iclock/cdata =================");
                string a = $"GET OPTION FROM: 2145224260046\\r\\nStamp=9999\\r\\nOpStamp=9999\\r\\nErrorDelay=60\\r\\nDelay=30\\r\\nTransTimes=00:00;14:05\\r\\nTransInterval=1\\r\\nTransFlag=111111111111\\r\\nRealtime=1\\r\\nTimeZone=7\\r\\nADMSSyncTime=1\\r\\n\"";
                logger.Info($"{a}");
                return "GET OPTION FROM: 2145224260046\r\nStamp=9999\r\nOpStamp=9999\r\nErrorDelay=60\r\nDelay=30\r\nTransTimes=00:00;14:05\r\nTransInterval=1\r\nTransFlag=111111111111\r\nRealtime=1\r\nTimeZone=7\r\nADMSSyncTime=1\r\n";
                
            }
            catch (System.Exception ex)
            {
                logger.Info($"Call iclock/cdata{ex}");
                //return new GetDataMCCResult
                //{
                //};
                return "";
            }
        }

        [HttpPost]
        [Route("swipe/dataSwipe")]
        public TakeDataSwipeResult checkData(TakeDataSwipeParameter parameter)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info($"+++++++++++++++++++++ API Post dataSwipe +++++++++++++++++++++++++++");
                logger.Info($"+++++++++++++++++++++ CardID{parameter.CardId} +++++++++++++++++++++++++++");
                logger.Info($"+++++++++++++++++++++ MId{parameter.MId} +++++++++++++++++++++++++++");
                logger.Info($"+++++++++++++++++++++ Time{parameter.Time} +++++++++++++++++++++++++++");

                return new TakeDataSwipeResult
                {
                    CardId = parameter.CardId,
                    MId = parameter.MId,
                    Time = parameter.Time,
                    StatusCode = HttpStatusCode.OK,
                    Message = "Success"
                };
            }
            catch (System.Exception ex)
            {
                return new TakeDataSwipeResult
                {
                    MessageCode = ex.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
        }

        [HttpPost]
        [Route("api/asset/getMasterDataAssetForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataAssetFormResult GetMasterDataAssetForm([FromBody] GetMasterDataAssetFormParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataAssetForm(request);
        }

        [HttpPost]
        [Route("api/asset/createOrUpdateAsset")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateAssetResult CreateOrUpdateAsset([FromBody] CreateOrUpdateAssetParameter request)
        {
            return this._iAssetDataAccess.CreateOrUpdateAsset(request);
        }

        [HttpPost]
        [Route("api/asset/getAllAssetList")]
        [Authorize(Policy = "Member")]
        public GetAllAssetListResult GetAllAssetList([FromBody] GetAllAssetListParameter parameter)
        {
            return this._iAssetDataAccess.GetAllAssetList(parameter);
        }

        [HttpPost]
        [Route("api/asset/getMasterDataPhanBoTSForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPhanBoTSFormResult GetMasterDataPhanBoTSForm([FromBody] GetMasterDataAssetFormParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataPhanBoTSForm(request);
        }

        [HttpPost]
        [Route("api/asset/taoPhanBoTaiSan")]
        [Authorize(Policy = "Member")]
        public TaoPhanBoTaiSanResult TaoPhanBoTaiSan([FromBody] TaoPhanBoTaiSanParameter request)
        {
            return this._iAssetDataAccess.TaoPhanBoTaiSan(request);
        }

        [HttpPost]
        [Route("api/asset/getMasterDataThuHoiTSForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPhanBoTSFormResult GetMasterDataThuHoiTSForm([FromBody] GetMasterDataAssetFormParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataThuHoiTSForm(request);
        }

        [HttpPost]
        [Route("api/asset/taoThuHoiTaiSan")]
        [Authorize(Policy = "Member")]
        public TaoPhanBoTaiSanResult TaoThuHoiTaiSan([FromBody] TaoPhanBoTaiSanParameter request)
        {
            return this._iAssetDataAccess.TaoThuHoiTaiSan(request);
        }

        [HttpPost]
        [Route("api/asset/createOrUpdateBaoDuong")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateBaoDuongResult CreateOrUpdateBaoDuong([FromBody] CreateOrUpdateBaoDuongParameter request)
        {
            return this._iAssetDataAccess.CreateOrUpdateBaoDuong(request);
        }

        [HttpPost]
        [Route("api/asset/deleteBaoDuong")]
        [Authorize(Policy = "Member")]
        public DeleteBaoDuongResult DeleteBaoDuong([FromBody] DeleteBaoDuongParameter request)
        {
            return this._iAssetDataAccess.DeleteBaoDuong(request);
        }

        [HttpPost]
        [Route("api/asset/getDataAssetDetail")]
        [Authorize(Policy = "Member")]
        public GetDataAssetDetailResult GetDataAssetDetail([FromBody] GetDataAssetDetailParameter request)
        {
            return this._iAssetDataAccess.GetDataAssetDetail(request);
        }


        [HttpPost]
        [Route("api/asset/uploadFile")]
        [AllowAnonymous]
        public UploadFileVacanciesResult UploadFile([FromForm] UploadFileAssetParameter request)
        {
            return this._iAssetDataAccess.UploadFile(request);
        }
        

       [HttpPost]
        [Route("api/asset/downloadTemplateAsset")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateAssetResult DownloadTemplateAsset([FromBody] DownloadTemplateAssetParameter request)
        {
            return this._iAssetDataAccess.DownloadTemplateAsset(request);
        }

        [HttpPost]
        [Route("api/asset/getMasterDataYeuCauCapPhatForm")]
        [Authorize(Policy = "Member")]
        public GetMasterDataPhanBoTSFormResult GetMasterDataYeuCauCapPhatForm([FromBody] GetMasterDataAssetFormParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataYeuCauCapPhatForm(request);
        }

        [HttpPost]
        [Route("api/asset/createOrYeuCauCapPhat")]
        [Authorize(Policy = "Member")]
        public CreateOrYeuCauCapPhatResult CreateOrYeuCauCapPhat([FromForm] CreateOrYeuCauCapPhatParameter request)
        {
            return this._iAssetDataAccess.CreateOrYeuCauCapPhat(request);
        }

        [HttpPost]
        [Route("api/asset/getAllYeuCauCapPhatTSList")]
        [Authorize(Policy = "Member")]
        public GetAllYeuCauCapPhatTSListResult GetAllYeuCauCapPhatTSList([FromBody] GetAllYeuCauCapPhatTSListParameter request)
        {
            return this._iAssetDataAccess.GetAllYeuCauCapPhatTSList(request);
        }

        [HttpPost]
        [Route("api/asset/xoaYeuCauCapPhat")]
        [Authorize(Policy = "Member")]
        public XoaYeuCauCapPhatResult XoaYeuCauCapPhat([FromBody] XoaYeuCauCapPhatParameter request)
        {
            return this._iAssetDataAccess.XoaYeuCauCapPhat(request);
        }

        [HttpPost]
        [Route("api/asset/getDataYeuCauCapPhatDetail")]
        [Authorize(Policy = "Member")]
        public GetDataYeuCauCapPhatDetailResult GetDataYeuCauCapPhatDetail([FromBody] GetDataYeuCauCapPhatDetailParameter request)
        {
            return this._iAssetDataAccess.GetDataYeuCauCapPhatDetail(request);
        }
        
        [HttpPost]
        [Route("api/asset/deleteChiTietYeuCauCapPhat")]
        [Authorize(Policy = "Member")]
        public DeleteChiTietYeuCauCapPhatResult DeleteChiTietYeuCauCapPhat([FromBody] DeleteChiTietYeuCauCapPhatParameter request)
        {
            return this._iAssetDataAccess.DeleteChiTietYeuCauCapPhat(request);
        }

        [HttpPost]
        [Route("api/asset/createOrUpdateChiTietYeuCauCapPhat")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateChiTietYeuCauCapPhatResult CreateOrUpdateChiTietYeuCauCapPhat([FromBody] CreateOrUpdateChiTietYeuCauCapPhatParameter request)
        {
            return this._iAssetDataAccess.CreateOrUpdateChiTietYeuCauCapPhat(request);
        }

        [HttpPost]
        [Route("api/asset/datVeMoiYeuCauCapPhatTS")]
        [Authorize(Policy = "Member")]
        public DatVeMoiYeuCauCapPhatTSResult DatVeMoiYeuCauCapPhatTS([FromBody] DatVeMoiYeuCauCapPhatTSParameter request)
        {
            return this._iAssetDataAccess.DatVeMoiYeuCauCapPhatTS(request);
        }

        [HttpPost]
        [Route("api/asset/capNhapTrangThaiYeuCauCapPhat")]
        [Authorize(Policy = "Member")]
        public CapNhapTrangThaiYeuCauCapPhatResult CapNhapTrangThaiYeuCauCapPhat([FromBody] CapNhapTrangThaiYeuCauCapPhatParameter request)
        {
            return this._iAssetDataAccess.CapNhapTrangThaiYeuCauCapPhat(request);
        }
        [HttpPost]
        [Route("api/asset/baoCaoPhanBo")]
        [Authorize(Policy = "Member")]
        public BaoCaoPhanBoResult BaoCaoPhanBo([FromBody] BaoCaoPhanBoParameter request)
        {
            return this._iAssetDataAccess.BaoCaoPhanBo(request);
        }
        [HttpPost]
        [Route("api/asset/baoCaoKhauHao")]
        [Authorize(Policy = "Member")]
        public BaoCaoKhauHaoResult BaoCaoKhauHao( [FromBody] BaoCaoKhauHaoParameter request)
        {
            return this._iAssetDataAccess.BaoCaoKhauHao(request);
        }

        [HttpPost]
        [Route("api/asset/getMasterDataBaoCaoPhanBo")]
        [Authorize(Policy = "Member")]
        public BaoCaoPhanBoResult GetMasterDataBaoCaoPhanBo([FromBody] BaoCaoPhanBoParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataBaoCaoPhanBo(request);
        }

        [HttpPost]
        [Route("api/asset/importAsset")]
        [Authorize(Policy = "Member")]
        public ImportAssetResult ImportAsset([FromBody] ImportAssetParameter request)
        {
            return this._iAssetDataAccess.ImportAsset(request);
        }

        [HttpPost]
        [Route("api/asset/downloadTemplateImportAsset")]
        [Authorize(Policy = "Member")]
        public DownloadTemplateImportResult DownloadTemplateImportAsset([FromBody] DownloadTemplateImportParameter request)
        {
            return this._iAssetDataAccess.DownloadTemplateImportAsset(request);
        }


        [HttpPost]
        [Route("api/asset/dotKiemKeSearch")]
        [Authorize(Policy = "Member")]
        public DotKiemKeSearchResult DotKiemKeSearch([FromBody] DotKiemKeSearchParameter request)
        {
            return this._iAssetDataAccess.DotKiemKeSearch(request);
        }


        [HttpPost]
        [Route("api/asset/taoDotKiemKe")]
        [Authorize(Policy = "Member")]
        public TaoDotKiemKeResult TaoDotKiemKe([FromBody] TaoDotKiemKeParameter request)
        {
            return this._iAssetDataAccess.TaoDotKiemKe(request);
        }

        [HttpPost]
        [Route("api/asset/deleteDotKiemKe")]
        [Authorize(Policy = "Member")]
        public DeleteDotKiemKeResult DeleteDotKiemKe([FromBody] DeleteDotKiemKeParameter request)
        {
            return this._iAssetDataAccess.DeleteDotKiemKe(request);
        }

        [HttpPost]
        [Route("api/asset/dotKiemKeDetail")]
        [Authorize(Policy = "Member")]
        public DotKiemKeDetailResult DotKiemKeDetail([FromBody] DotKiemKeDetailParameter request)
        {
            return this._iAssetDataAccess.DotKiemKeDetail(request);
        }

        [HttpPost]
        [Route("api/asset/updateKhauHaoMobile")]
        [Authorize(Policy = "Member")]
        public UpdateKhauHaoMobileResult UpdateKhauHaoMobile([FromBody] UpdateKhauHaoMobileParameter request)
        {
            return this._iAssetDataAccess.UpdateKhauHaoMobile(request);
        }

        [HttpPost]
        [Route("api/asset/addTaiSanToDotKiemKe")]
        [Authorize(Policy = "Member")]
        public AddTaiSanToDotKiemKeResult AddTaiSanToDotKiemKe([FromBody] AddTaiSanToDotKiemKeParameter request)
        {
            return this._iAssetDataAccess.AddTaiSanToDotKiemKe(request);
        }

        [HttpPost]
        [Route("api/asset/getMasterDataAddTaiSanVaoDotKiemKe")]
        [Authorize(Policy = "Member")]
        public GetMasterDataAddTaiSanVaoDotKiemKeResult GetMasterDataAddTaiSanVaoDotKiemKe([FromBody] GetMasterDataAddTaiSanVaoDotKiemKeParameter request)
        {
            return this._iAssetDataAccess.GetMasterDataAddTaiSanVaoDotKiemKe(request);
        }

    }
}
