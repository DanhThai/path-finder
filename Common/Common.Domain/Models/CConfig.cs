namespace Common.Domain
{
    public class CConfig
    {
        public int EnsureDBPort { get; set; } = 5432;
        public CServiceConfig ServiceInfo { get; set; }
        public string ConnectionString { get; set; }
        public EndpointConfig Endpoints { get; set; }
        public AuthConfig Authentication { get; set; }
        public StorageConfig Storage { get; set; }
    }

    public class AuthConfig
    {
        public JwtConfig JWT { get; set; }
        public GoogleConfig Google { get; set; }
    }

    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }

    public class StorageConfig
    {
        public CloudinaryConfig Cloudinary { get; set; }
    }

    public class CServiceConfig
    {
        public string Version { get; set; } = "v1";
        public int DevelopmentPort { get; set; }
        public string APIUrl { get; set; }
    }

    public class EndpointConfig
    {
        public string Admin { get; set; }
        public string Learner { get; set; }
    }

    public class GoogleConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class CloudinaryConfig
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
