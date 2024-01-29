using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntSurvey.QuestionModels
{
    public class ResponseVariant
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public string response { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public int questionnaireId { get; set; }
        public string question { get; set; }
        public int gradingType { get; set; }
        public string comentary { get; set; }
        public List<ResponseVariant> responseVariants { get; set; }
    }

    public class Questionnaire
    {
        public int oid { get; set; }
        public string name { get; set; }
        public List<Question> questions { get; set; }
        public int companyOid { get; set; }
        public string company { get; set; }
    }

    public class Response
    {
        public int id { get; set; }  // Assuming this is the response ID
        public int questionId { get; set; }
        public int responseVariantId { get; set; }
        public string alternativeResponse { get; set; }
        public string comentary { get; set; }
    }

    public class ResponseData
    {
        public int oid { get; set; }
        public int questionnaireId { get; set; }
        public int companyOid { get; set; }
        public string licenseId { get; set; }
        public List<Response> responses { get; set; }
    }

    public class RootObject
    {
        public string errorMessage { get; set; }
        public string errorName { get; set; }
        public int errorCode { get; set; }
        public Questionnaire questionnaire { get; set; }
    }

    public class EncodedObj
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Settings { get; set; }
        public string URI { get; set; }
    }

    public class ServiceInfo
    {
        public string ServiceUri { get; set; }
        public string Port { get; set; }
        public bool Authorization { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    public class AppCredentials
    {
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static string Uri { get; set; }
        public static string CacID { get; set; }
    }

}
