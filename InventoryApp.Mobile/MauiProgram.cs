using InventoryApp.Mobile.Services;
using Microsoft.Extensions.Logging;

namespace InventoryApp.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		var baseUrl = DeviceInfo.Current.Platform == DevicePlatform.Android
			? "https://10.0.2.2:7023/"
			: "https://localhost:7023/";

		var inventoryClient = builder.Services.AddHttpClient<IInventoryService, InventoryService>(c =>
		{
			c.BaseAddress = new Uri(baseUrl);
			c.Timeout = TimeSpan.FromSeconds(15);
		});

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();

#if ANDROID
		inventoryClient.ConfigurePrimaryHttpMessageHandler(() => new Xamarin.Android.Net.AndroidMessageHandler
		{
			ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
				errors == System.Net.Security.SslPolicyErrors.None || cert?.Issuer == "CN=localhost"
		});
#elif IOS
		inventoryClient.ConfigurePrimaryHttpMessageHandler(() => new NSUrlSessionHandler
		{
			TrustOverrideForUrl = (sender, url, trust) => url.StartsWith("https://localhost", StringComparison.Ordinal)
		});
#endif
#endif

		return builder.Build();
	}
}
