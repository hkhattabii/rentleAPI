using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentleAPI.Models
{
    public class DocumentGen
    {
        string message { get; set; }
        bool success { get;  set; }

        public DocumentGen(string message, bool success)
        {
            this.message = message;
            this.success = success;
        }

    }
}
