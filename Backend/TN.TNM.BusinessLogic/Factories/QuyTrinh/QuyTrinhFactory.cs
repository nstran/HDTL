using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TN.TNM.BusinessLogic.Interfaces.QuyTrinh;
using TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh;
using TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.BusinessLogic.Factories.QuyTrinh
{
    public class QuyTrinhFactory : BaseFactory//, IQuyTrinh
    {
        //private IQuyTrinhDataAccess iQuyTrinhDataAccess;

        //public QuyTrinhFactory(IQuyTrinhDataAccess _iQuyTrinhDataAccess)
        //{
        //    this.iQuyTrinhDataAccess = _iQuyTrinhDataAccess;
        //}

        //public CreateQuyTrinhResponse CreateQuyTrinh(CreateQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.CreateQuyTrinh(parameter);
        //        var response = new CreateQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            Id = result.Id
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new CreateQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public SearchQuyTrinhResponse SearchQuyTrinh(SearchQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.SearchQuyTrinh(parameter);
        //        var response = new SearchQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListQuyTrinh = result.ListQuyTrinh
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new SearchQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public GetMasterDataSearchQuyTrinhResponse GetMasterDataSearchQuyTrinh(GetMasterDataSearchQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.GetMasterDataSearchQuyTrinh(parameter);
        //        var response = new GetMasterDataSearchQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListEmployee = result.ListEmployee
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GetMasterDataSearchQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public GetDetailQuyTrinhResponse GetDetailQuyTrinh(GetDetailQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.GetDetailQuyTrinh(parameter);
        //        var response = new GetDetailQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            QuyTrinh = result.QuyTrinh,
        //            ListCauHinhQuyTrinh = result.ListCauHinhQuyTrinh
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GetDetailQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public UpdateQuyTrinhResponse UpdateQuyTrinh(UpdateQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.UpdateQuyTrinh(parameter);
        //        var response = new UpdateQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new UpdateQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public DeleteQuyTrinhResponse DeleteQuyTrinh(DeleteQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.DeleteQuyTrinh(parameter);
        //        var response = new DeleteQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new DeleteQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public CheckTrangThaiQuyTrinhResponse CheckTrangThaiQuyTrinh(CheckTrangThaiQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.CheckTrangThaiQuyTrinh(parameter);
        //        var response = new CheckTrangThaiQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            Exists = result.Exists
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new CheckTrangThaiQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public GuiPheDuyetResponse GuiPheDuyet(GuiPheDuyetRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.GuiPheDuyet(parameter);
        //        var response = new GuiPheDuyetResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GuiPheDuyetResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public PheDuyetResponse PheDuyet(PheDuyetRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.PheDuyet(parameter);
        //        var response = new PheDuyetResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new PheDuyetResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public HuyYeuCauPheDuyetResponse HuyYeuCauPheDuyet(HuyYeuCauPheDuyetRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.HuyYeuCauPheDuyet(parameter);
        //        var response = new HuyYeuCauPheDuyetResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new HuyYeuCauPheDuyetResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public TuChoiResponse TuChoi(TuChoiRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.TuChoi(parameter);
        //        var response = new TuChoiResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new TuChoiResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public GetLichSuPheDuyetResponse GetLichSuPheDuyet(GetLichSuPheDuyetRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.GetLichSuPheDuyet(parameter);
        //        var response = new GetLichSuPheDuyetResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListLichSuPheDuyet = result.ListLichSuPheDuyet
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GetLichSuPheDuyetResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}

        //public GetDuLieuQuyTrinhResponse GetDuLieuQuyTrinh(GetDuLieuQuyTrinhRequest request)
        //{
        //    try
        //    {
        //        var parameter = request.ToParameter();
        //        var result = iQuyTrinhDataAccess.GetDuLieuQuyTrinh(parameter);
        //        var response = new GetDuLieuQuyTrinhResponse()
        //        {
        //            StatusCode = result.Status ? HttpStatusCode.OK : HttpStatusCode.ExpectationFailed,
        //            MessageCode = result.Message,
        //            ListDuLieuQuyTrinh = result.ListDuLieuQuyTrinh
        //        };
        //        return response;
        //    }
        //    catch (Exception e)
        //    {
        //        return new GetDuLieuQuyTrinhResponse()
        //        {
        //            StatusCode = HttpStatusCode.ExpectationFailed,
        //            MessageCode = e.Message
        //        };
        //    }
        //}
    }
}
