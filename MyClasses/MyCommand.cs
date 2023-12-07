using System.Net.Http;

namespace MyClasses
{
    [Serializable]
    public class MyCommand
    {
        public HttpMethods HttpMethod { get; set; }

        public Car Car { get; set; }
    }
}
