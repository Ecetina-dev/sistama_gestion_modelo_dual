using System;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Excepcion personalizada para errores de conexion a base de datos.
    /// </summary>
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException() : base() { }
        public DatabaseConnectionException(string message) : base(message) { }
        public DatabaseConnectionException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    /// <summary>
    /// Excepcion personalizada para errores de operaciones CRUD.
    /// </summary>
    public class CrudOperationException : Exception
    {
        public string Operation { get; set; }
        public object Entity { get; set; }

        public CrudOperationException() : base() { }
        public CrudOperationException(string message) : base(message) { }
        public CrudOperationException(string message, Exception innerException) 
            : base(message, innerException) { }
        public CrudOperationException(string message, string operation, object entity) 
            : base(message)
        {
            Operation = operation;
            Entity = entity;
        }
    }

    /// <summary>
    /// Excepcion personalizada para errores de validacion.
    /// </summary>
    public class ValidationException : Exception
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerException) 
            : base(message, innerException) { }
        public ValidationException(string message, string fieldName, string fieldValue) 
            : base(message)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }
    }

    /// <summary>
    /// Excepcion personalizada para errores de registro duplicado.
    /// </summary>
    public class DuplicateRecordException : Exception
    {
        public string TableName { get; set; }
        public string DuplicateValue { get; set; }

        public DuplicateRecordException() : base() { }
        public DuplicateRecordException(string message) : base(message) { }
        public DuplicateRecordException(string message, Exception innerException) 
            : base(message, innerException) { }
        public DuplicateRecordException(string tableName, string duplicateValue) 
            : base($"El registro '{duplicateValue}' ya existe en la tabla '{tableName}'.")
        {
            TableName = tableName;
            DuplicateValue = duplicateValue;
        }
    }

    /// <summary>
    /// Excepcion personalizada para errores de referencia a entidad relacionada.
    /// </summary>
    public class ReferentialIntegrityException : Exception
    {
        public string PrimaryTable { get; set; }
        public string ForeignTable { get; set; }
        public int EntityId { get; set; }

        public ReferentialIntegrityException() : base() { }
        public ReferentialIntegrityException(string message) : base(message) { }
        public ReferentialIntegrityException(string message, Exception innerException) 
            : base(message, innerException) { }
        public ReferentialIntegrityException(string primaryTable, string foreignTable, int entityId) 
            : base($"No se puede eliminar el registro {entityId} de '{primaryTable}' porque tiene referencias en '{foreignTable}'.")
        {
            PrimaryTable = primaryTable;
            ForeignTable = foreignTable;
            EntityId = entityId;
        }
    }
}