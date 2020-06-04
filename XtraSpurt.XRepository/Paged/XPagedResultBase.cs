using System;

namespace XtraSpurt.XRepository
{
    public class XPagedResultBase
    {
        public int Current { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public int RowCount { get; set; }
        public int FirstRow => (Current - 1) * Size + 1;
        public int LastRow => Math.Min(Current * Size, RowCount);
        public bool HasPrevious => (Current > 1);
        public bool HasNext => (Current < Count);
    }
}