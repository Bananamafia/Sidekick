using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sidekick.Application.Clipboard;
using Sidekick.Application.Game.Items.Parser.Patterns;
using Sidekick.Application.Game.Languages;
using Sidekick.Application.Keybinds;
using Sidekick.Application.Settings;
using Sidekick.Domain.Clipboard;
using Sidekick.Domain.Game.Languages;
using Sidekick.Domain.Keybinds;
using Sidekick.Domain.Settings;

namespace Sidekick.Application
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSidekickApplication(this IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SaveSettingsHandler.FileName, optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var sidekickConfiguration = new SidekickSettings();
            configuration.Bind(sidekickConfiguration);
            services.AddSingleton(sidekickConfiguration);

            services.AddSingleton<IGameLanguageProvider, GameLanguageProvider>();
            services.AddSingleton<ISidekickSettings>(sp => sp.GetRequiredService<SidekickSettings>());
            services.AddSingleton<IKeybindsExecutor, KeybindsExecutor>();
            services.AddSingleton<IClipboardProvider, ClipboardProvider>();
            services.AddSingleton<IParserPatterns, ParserPatterns>();

            return services;
        }
    }
}
