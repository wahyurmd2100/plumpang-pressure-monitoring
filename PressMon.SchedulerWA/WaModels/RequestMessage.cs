using System.Collections.Generic;

namespace TMS.Web.Models.sendWA
{
    public class RequestMessage
    {
        public string to { get; set; }
        public string type { get; set; }
        public Template template { get; set; }

    }
    public class Template
    {
        public string @namespace { get; set; }
        public string name { get; set; }
        public Language language { get; set; }
        public List<Component> components { get; set; }
    }

    public class Language
    {
        public string policy { get; set; }
        public string code { get; set; }
    }

    public class Component
    {
        public string type { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class Parameter
    {
        public string type { get; set; }
        public string text { get; set; }
    }
}
