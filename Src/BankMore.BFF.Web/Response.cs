namespace BankMore.BFF.Web
{
    public class Response<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
