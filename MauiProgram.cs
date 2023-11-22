using Microsoft.Extensions.Logging;
using Sentry;
using Sentry.Profiling;
using System.Net.Sockets;

namespace maui_sentry;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()

			.UseSentry(options =>
			{
				// The DSN is the only required option.
				options.Dsn = "FILL ME IN";

				// By default, we will send the last 100 breadcrumbs with each event.
				// If you want to see everything we can capture from MAUI, you may wish to use a larger value.
				options.MaxBreadcrumbs = 1000;

                options.Debug = true;
                // options.AutoSessionTracking = true;
                options.IsGlobalModeEnabled = true;
                options.EnableTracing = true;

                var profilingIntegration = new ProfilingIntegration();

                options.AddIntegration(profilingIntegration);
			})

			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var env = Environment.GetEnvironmentVariable("DOTNET_DiagnosticPorts");


		Socket listener = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
		listener.Bind(new UnixDomainSocketEndPoint(Path.Join(Path.GetTempPath(), $"dotnet-diagnostic-{Environment.ProcessId}-1-socket")));
        listener.Listen(100);

        Task.Run(async () =>
		{
			while (true)
			{
				var unixSocket = await listener.AcceptAsync();
				_ = Task.Run(async () =>
				{
					try
					{
						var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
						await tcpSocket.ConnectAsync("127.0.0.1", 9009);
						var buffer = new byte[65536];
						while (true)
						{
							var tcpRead = tcpSocket.ReceiveAsync(Array.Empty<byte>());
							var unixRead = unixSocket.ReceiveAsync(Array.Empty<byte>());
							var currentRead = await Task.WhenAny(tcpRead, unixRead);
							if (currentRead == tcpRead)
							{
								int bytesRead = await tcpSocket.ReceiveAsync(buffer);
								int bytesSent = await unixSocket.SendAsync(buffer.AsMemory(0, bytesRead));
							}
							else
							{
								int bytesRead = await unixSocket.ReceiveAsync(buffer);
								int bytesSent = await tcpSocket.SendAsync(buffer.AsMemory(0, bytesRead));
							}
						}
					}
					catch (Exception ex)
					{

					}
				});
			}
        });

        return builder.Build();
	}
}
