using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using graduation_project_ideas_2.Models;

namespace graduation_project_ideas_2.Model1
{
    public class showTeamProj
    {
        public IEnumerable<users> user { get; set; }
        public IEnumerable<teamleaders> teamleaders { get; set; }
        public IEnumerable<projects> projects { get; set; }
    }
}