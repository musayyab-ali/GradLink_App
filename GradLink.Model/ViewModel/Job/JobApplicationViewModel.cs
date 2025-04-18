using GradLink.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradLink.Model.ViewModel.Job
{
    public class JobApplicationViewModel
    {
        public List<GradLink.Repository.MSSQL.ORM.Entities.Job> JobPosts { get; set; }
        public List<int> AppliedJobIds { get; set; }
    }
}
