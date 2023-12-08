using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;



namespace IntSurvey.Models
{
    public class activationCodeID
    {
        public string id { get; set; }
        public string errorMessage { get; set; }
        public string errorName { get; set; }
        public int errorcode { get; set; }

    }
    public class Questionnaire
    {
        public int oid { get; set; }
        public string name { get; set; }
        public int companyOid { get; set; }
        public string company { get; set; }
    }

    public class Root
    {
        public string errorMessage { get; set; }
        public string errorName { get; set; }
        public int errorCode { get; set; }
        public List<Questionnaire> questionnaires { get; set; }
    }


public class keyVM
    {
        public string id;

        public List<activationCodeID> CodeID { get; set; }
    }

}
