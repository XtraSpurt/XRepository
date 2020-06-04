using System.Collections.Generic;

namespace XtraSpurt.XRepository
{
    public class XPagedResult<TM, TV> : XPagedResultBase where TM : class where TV : class
    {
        public List<TV> Results { get; set; }

        public XPagedResult()
        {
            Results = new List<TV>();
        }
    }

    public class XPagedResult<TM> : XPagedResultBase where TM : class
    {
        public List<TM> Results { get; set; }

        public XPagedResult()
        {
            Results = new List<TM>();
        }
    }
}