using System;

namespace Tolitech.CodeGenerator.Auditing
{
    public class AuditScope : IDisposable
    {
        private readonly AuditableEntity _entity;
        private readonly EventTypeEnum _eventType;

        public AuditScope(AuditableEntity entity, EventTypeEnum eventType)
        {
            _entity = entity;
            _eventType = eventType;

            Start();
        }

        private void Start()
        {
            _entity.StartAudit(_eventType);
        }

        private void Finish()
        {
            _entity.FinishAudit();
        }

        public void Dispose()
        {
            Finish();
            GC.SuppressFinalize(this);
        }
    }
}