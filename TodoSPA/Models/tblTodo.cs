using System;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public partial class tblTodo
    {
        public int RecId { get; set; }
        public int fkUserId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public bool Complete { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
    }
}
