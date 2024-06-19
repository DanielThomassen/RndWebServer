namespace RndWebServer.Server.Infrastructure;

public interface IEndpoint
{
    Task ProcessPost();
    Task ProcessGet();
    Task ProcessDelete();
    Task ProcessPut();
    Task ProcessPatch();
}