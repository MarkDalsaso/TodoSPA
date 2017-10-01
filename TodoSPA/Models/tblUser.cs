using System;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public partial class tblUser
    {
        public int RecId { get; set; }
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
    }
}