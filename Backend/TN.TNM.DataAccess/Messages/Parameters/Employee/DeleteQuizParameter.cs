using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteQuizParameter : BaseParameter
    {
        public Guid QuizId { get; set; }
    }

}
