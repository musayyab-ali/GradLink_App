using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradLink.Model.ViewModel.QuestionAnswer
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }
}
