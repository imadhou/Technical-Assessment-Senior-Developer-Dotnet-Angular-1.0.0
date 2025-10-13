using N2J.WebApi.Extensions;
using N2J.WebApi.Middleware;
using N2J.WebApi.Startup;

var builder = WebApplication.CreateBuilder(args);
var startup = new Startup(builder);
await startup.RunAsync();

public partial class Program { }