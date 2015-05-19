using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MessageBoard.Models;

namespace MessageBoard.ViewModels
{
    public class BaseViewModel
    {
        public string ResultMessage { get; set; }
        public ActionResultStatus ResultStatus { get; set; }
    }
}