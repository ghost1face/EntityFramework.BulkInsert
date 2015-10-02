using System;

namespace EntityFramework.BulkInsert.Exceptions
{
    public class EntityTypeNotFoundException : Exception
    {
        private readonly Type _type;
        public EntityTypeNotFoundException(Type type)
        {
            _type = type;
        }

        public override string Message
        {
            get { return string.Format("Type '{0}' was not found in context", _type); }
        }
    }
}
