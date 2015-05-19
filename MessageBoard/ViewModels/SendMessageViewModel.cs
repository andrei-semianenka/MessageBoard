using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MessageBoard.ViewModels
{
    public class SendMessageViewModel : BaseViewModel
    {
        public string MessageText { get; set; }
        public string SortFieldName { get; set; }
        public bool SortAscending { get; set; }
    }
}