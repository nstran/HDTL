using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class EditEmployeeRequestByIdParameter : BaseParameter
    {
        public DeXuatXinNghiModel DeXuatXinNghi { get; set; }
    }
}
