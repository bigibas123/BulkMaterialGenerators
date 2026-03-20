using System;
using System.Collections.Generic;
using cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Locales;
using nadena.dev.ndmf;
using nadena.dev.ndmf.localization;

namespace cc.dingemans.bigibas123.bulkmaterialgenerators.Editor.Utils
{
    public class BulkMaterialException : Exception
    {
        public BulkMaterialException(ErrorSeverity severity)
        {
            Severity = severity;
        }

        public BulkMaterialException(ErrorSeverity severity, string message, params string[] messagesubst) :
            base(message)
        {
            Severity = severity;
            _messageSubstitutions.AddRange(messagesubst);
        }

        public BulkMaterialException(ErrorSeverity severity, Exception innerException, string message,
            params string[] messagesubst) : base(message,
            innerException)
        {
            Severity = severity;
            _messageSubstitutions.AddRange(messagesubst);
        }


        public ErrorSeverity Severity { get; } = ErrorSeverity.Error;
        private List<ObjectReference> _objects = new();
        public ObjectReference[] References => _objects.ToArray();
        private List<string> _messageSubstitutions = new();
        public string[] MessageSubstitutions => _messageSubstitutions.ToArray();
        private BulkMaterialError _error;

        public void AddReference(ObjectReference obj)
        {
            _objects.Add(obj);
        }

        public void AddReference(UnityEngine.Object obj)
        {
            AddReference(ObjectRegistry.GetReference(obj));
        }

        public BulkMaterialError Error
        {
            get
            {
                if (_error == null)
                {
                    _error = new BulkMaterialError(this);
                }

                return _error;
            }
        }
    }

    public class BulkMaterialError : SimpleError
    {
        public BulkMaterialError(BulkMaterialException exception)
        {
            _exception = exception;
        }

        public override Localizer Localizer => BMGLocales.L;
        public override string TitleKey => $"BMG:{_exception.Message}";
        public override string[] TitleSubst => _exception.MessageSubstitutions;
        public override ErrorSeverity Severity => _exception.Severity;

        private BulkMaterialException _exception;
        public override ObjectReference[] References => _exception.References;
    }
}