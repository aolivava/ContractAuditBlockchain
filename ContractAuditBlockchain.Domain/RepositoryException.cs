using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Runtime.Serialization;

namespace ContractAuditBlockchain.Domain
{
    public class ValidationError
    {
        public ValidationError() { this.FieldErrors = new Dictionary<string, List<string>>(); }

        public string EntityName { get; set; }
        public bool IsValid { get; set; }
        public Dictionary<string, List<string>> FieldErrors { get; set; }
    }

    public class RepositoryException : Exception, ISerializable
    {
        private List<ValidationError> validationErrors = new List<ValidationError>();

        public RepositoryException() : base() { }
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }
        public RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public List<ValidationError> ValidationErrors
        {
            get
            {
                validationErrors.Clear();
                if (this.InnerException != null && this.InnerException.GetType() == typeof(DbEntityValidationException))
                {
                    foreach (var error in ((DbEntityValidationException)InnerException).EntityValidationErrors)
                    {
                        ValidationError ve = new ValidationError()
                        {
                            EntityName = error.Entry.Entity.ToString(),
                            IsValid = error.IsValid
                        };
                        foreach (var validationError in error.ValidationErrors)
                        {
                            if (!ve.FieldErrors.ContainsKey(validationError.PropertyName))
                            {
                                ve.FieldErrors.Add(validationError.PropertyName, new List<string>());
                            }
                            ve.FieldErrors[validationError.PropertyName].Add(validationError.ErrorMessage);
                        }
                        validationErrors.Add(ve);
                    }
                }
                return validationErrors;
            }
        }
    }
}
