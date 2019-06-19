using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace graduation_project_ideas_2.Models
{
    public class DeptProf
    {
        public int id { get; set; }

        public IEnumerable<Department> departments { get; set; }

        public int? dept_id { get; set; }

        public int pro_user { get; set; }

    }
}