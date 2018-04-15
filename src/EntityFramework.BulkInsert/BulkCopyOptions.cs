using System;

namespace EntityFramework.BulkInsert
{
    [Flags]
    public enum BulkCopyOptions
    {
        //
        // Summary:
        //     Use the default values for all options.
        Default = 0,
        //
        // Summary:
        //     Preserve source identity values. When not specified, identity values are assigned
        //     by the destination.
        KeepIdentity = 1,
        //
        // Summary:
        //     Check constraints while data is being inserted. By default, constraints are not
        //     checked.
        CheckConstraints = 2,
        //
        // Summary:
        //     Obtain a bulk update lock for the duration of the bulk copy operation. When not
        //     specified, row locks are used.
        TableLock = 4,
        //
        // Summary:
        //     Preserve null values in the destination table regardless of the settings for
        //     default values. When not specified, null values are replaced by default values
        //     where applicable.
        KeepNulls = 8,
        //
        // Summary:
        //     When specified, cause the server to fire the insert triggers for the rows being
        //     inserted into the database.
        FireTriggers = 16,
        //
        // Summary:
        //     When specified, each batch of the bulk-copy operation will occur within a transaction.
        //     If you indicate this option and also provide a System.Data.SqlClient.SqlTransaction
        //     object to the constructor, an System.ArgumentException occurs.
        UseInternalTransaction = 32
    }
}
