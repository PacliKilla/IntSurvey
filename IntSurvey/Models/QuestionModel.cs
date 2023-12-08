using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntSurvey.QuestionModels
{
    public class Questionnaire
    {
        public int oid { get; set; }
        public string name { get; set; }
        public List<Question> questions { get; set; }
        public int companyOid { get; set; }
        public string company { get; set; }
    }

    public class Question
    {
        public string question { get; set; }
        public int gradingType { get; set; }
        public string comentary { get; set; }
        public List<string> answerVariants { get; set; }
    }

    public class RootObject
    {
        public Questionnaire questionnaire { get; set; }
        public string errorMessage { get; set; }
        public string errorName { get; set; }
        public int errorCode { get; set; }
    }

    public class Response
    {
        public string Question { get; set; }
        public List<string> response { get; set; }
    }
    public class QuestionResponse
    {
        public string question { get; set; }
        public int gradingType { get; set; }
        public string comentary { get; set; }
        public List<string> answerVariants { get; set; }
    }

    public class ResponseData
    {
        public int oid { get; set; } 
        public int questionnaireId { get; set; }
        public int companyOid { get; set; }
        public string licenseId { get; set; }
        public List<Response> responses { get; internal set; }
    }





}
