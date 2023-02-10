using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeRequestDataAccess
    {
        CreateEmployeeRequestResult CreateEmployeeRequest(CreateEmplyeeRequestParameter parameter);
        GetEmployeeRequestByIdResult GetEmployeeRequestById(GetEmployeeRequestByIdParameter parameter);
        EditEmployeeRequestByIdResult EditEmployeeRequestById(EditEmployeeRequestByIdParameter parameter);
        SearchEmployeeRequestResult SearchEmployeeRequest(SearchEmployeeRequestParameter parameter);
        GetDataSearchEmployeeRequestResult GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestParameter parameter);
        GetMasterCreateEmpRequestResult GetMasterCreateEmpRequest(GetMasterCreateEmpRequestParameter parameter);
        DeleteDeXuatXinNghiByIdResult DeleteDeXuatXinNghiById(DeleteDeXuatXinNghiByIdParameter parameter);
        DatVeMoiDeXuatXinNghiResult DatVeMoiDeXuatXinNghi(DatVeMoiDeXuatXinNghiParameter parameter);
    }
}
