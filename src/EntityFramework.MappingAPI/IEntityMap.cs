using System;

namespace EntityFramework.MappingAPI
{
    public interface IEntityMap
    {
        /// <summary>
        /// Entity type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Table name in database
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Table schema
        /// </summary>
        string Schema { get; }

        /// <summary>
        /// Is table-per-hierarchy mapping
        /// </summary>
        bool IsTph { get; }

        /// <summary>
        /// Is table-per-hierarchy base entity
        /// </summary>
        bool IsRoot { get; }

        /// <summary>
        /// Mapped properties
        /// </summary>
        IPropertyMap[] Properties { get; }

        /// <summary>
        /// Foreign key properties
        /// </summary>
        IPropertyMap[] Fks { get; }

        /// <summary>
        /// Primary key properties
        /// </summary>
        IPropertyMap[] Pks { get; }

        /// <summary>
        /// Tph entity discriminators
        /// </summary>
        IPropertyMap[] Discriminators { get; }

        /// <summary>
        /// Gets property map by property name
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IPropertyMap this[string property] { get; }

        /// <summary>
        /// Gets property map by property name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        IPropertyMap Prop(string propertyName);
    }
}
