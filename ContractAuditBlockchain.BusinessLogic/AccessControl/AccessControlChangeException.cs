using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ContractAuditBlockchain.BusinessLogic.AccessControl
{
    [Serializable]
    public class AccessControlChangeException : Exception
    {
        private IList<string> errors;

        public IEnumerable<string> Errors
        {
            get { return new ReadOnlyCollection<string>(errors); }
        }

        public AccessControlChangeException(string message) : base(message)
        {
            errors = new List<string>();
        }
        public AccessControlChangeException(string message, Exception inner) : base(message, inner)
        {
            errors = new List<string>();
        }
        public AccessControlChangeException(string message, IEnumerable<string> errors) : base(message)
        {
            this.errors = errors.ToList();
        }
        public AccessControlChangeException(string message, IEnumerable<string> errors, Exception inner) : base(message, inner)
        {
            this.errors = errors.ToList();
        }

        protected AccessControlChangeException(SerializationInfo info, StreamingContext context)
            :base(info, context)
        {
            errors = info.GetValue(nameof(errors), typeof(List<string>)) as IList<string>;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(errors), errors);
        }

    }
}
