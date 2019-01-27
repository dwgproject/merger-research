using System;
using System.Collections.Generic;

namespace HuntingHelperWebService.Model{
    public class Hunt{

        public int ID { get; set; }
        public DateTime Issued { get; set; }

        public ICollection<User> Users { get; set; }




    }
}