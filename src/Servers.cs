namespace IntegrationTests;

public class Servers
{
    public static readonly string PostgresConnectionString =
        "Host=localhost;Port=5433;Database=postgres;Username=postgres;password=postgres";

    public static readonly string SqlServerConnectionString =
        "Server=localhost;User Id=sa;Password=Str0ngPa$$w0rd;Database=master;Encrypt=False;TrustServerCertificate=true";
}