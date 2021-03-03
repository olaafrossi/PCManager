using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCManager.DataAccess.Library.Models
{
    public class LogModel
    {
        public int iD { get; set; }

        public string Timestamp { get; set; }

        public string Level { get; set; }

        public string Exception { get; set; }

        public string RenderedMessage { get; set; }

        public string Properties { get; set; }
    }
}
