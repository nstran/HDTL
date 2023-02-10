using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateEmplyeeRequestParameter : BaseParameter
    {
        public DeXuatXinNghiModel DeXuatXinNghi { get; set; }
    }
}
