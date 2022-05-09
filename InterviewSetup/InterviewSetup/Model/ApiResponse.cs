using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewSetup.Model
{
    public class ApiResponse
    {
        public int Count { get; set; }
        public List<Wmi> Results { get; set; }
    }
}
