namespace XtraSpurt.XRepository
{
    public class XPagedQueryParam
    {
        /// <summary>
        ///  Current Page Number 
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        ///  Total Item in a page
        /// </summary>
        public int Size { get; set; } = 10;
    }
}