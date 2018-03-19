using System;

namespace EntityFramework.BulkInsert
{
    public class RowsCopiedEventArgs : EventArgs
    {
        public RowsCopiedEventArgs(long rowsCopied)
        {
            this.RowsCopied = rowsCopied;
        }

        //public bool Abort { get; set; }
        public long RowsCopied { get; set; }
    }
}
