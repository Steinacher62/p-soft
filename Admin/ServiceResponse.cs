using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using Telerik.Web.UI;

namespace ch.appl.psoft.Admin
{

    public class ServiceResponse
    {

        public ServiceResponse()
        {

        }

        public string GetServiceResponse (Exception Error, string MessageShort)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<ErrorList> errorList = new List<ErrorList>();
            ErrorList ListItem = new ErrorList();
            ListItem.Type = "Error";
            ListItem.Title = "Fehler";
            ListItem.ErrorMessageShort = MessageShort;
            ListItem.ErrorMessage = Error.Message;
            errorList.Add(ListItem);
      
            return serializer.Serialize(errorList);
        }

        private class ErrorList
        {
            public string Type { get; set; }
            public string Title { get; set; }
            public string ErrorMessageShort { get; set; }

            public string ErrorMessage { get; set; }
        }

    }
}