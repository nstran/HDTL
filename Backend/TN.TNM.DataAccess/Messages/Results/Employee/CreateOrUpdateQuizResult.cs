using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class CreateOrUpdateQuizResult : BaseResult
    {
        public Guid QuizId { get; set; }
    }

}
