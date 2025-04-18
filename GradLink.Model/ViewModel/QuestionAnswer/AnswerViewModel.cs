using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradLink.Model.ViewModel.QuestionAnswer
{

    public class AnswerViewModel
    {
        public int Id { get; set; }
        public string AnsweredBy { get; set; }
        public string AnswerText { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
