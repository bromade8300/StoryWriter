using Microsoft.Extensions.Logging;
using StoryWriter.BDD;
namespace StoryWriterClient;

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
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		try
		{
        } catch (Exception e)
        {

        }

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
