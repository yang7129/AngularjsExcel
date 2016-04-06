using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MyTestWebBack.Models
{
    public class index
    {
        //[Required(ErrorMessage = "*")] 
        public string Sn { get; set; }
         
        public string TagName { get; set; }
         
        public string MallSn { get; set; }
         
        public string State { get; set; }
         
        public string ImageURL { get; set; }

        public string Priority { get; set; } 
    }
}
 
