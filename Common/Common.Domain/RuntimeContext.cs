namespace Common.Domain
{
    public static class RuntimeContext
    {
        private static readonly AsyncLocal<RuntimeContextInstance> _current = new AsyncLocal<RuntimeContextInstance>();
        public static RuntimeContextInstance Current
        {
            get
            {
                if (_current.Value == null)
                {
                    _current.Value = new RuntimeContextInstance();
                }
                return _current.Value;
            }
            set
            {
                _current.Value = value;
            }
        }

        public static CConfig Config { get; set; }
    }

    public class RuntimeContextInstance
    {
        public Guid UserId { get; set; }
        public CUser User { get; set; }
        public string ClientIP { get; set; }
    }
}
