// Copyright (c) 2023 Netified <contact@netified.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

namespace SatisfactoryBot
{
    using DSharpPlus;
    using DSharpPlus.CommandsNext;
    using DSharpPlus.Entities;
    using DSharpPlus.EventArgs;
    using DSharpPlus.SlashCommands;
    using Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Models;
    using Serilog;

    public class Worker : IHostedService
    {
        public static DiscordShardedClient? Client;
        public static BotConfiguration? Configuration;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public Worker(IConfiguration configuration, ILogger<Worker> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Configuration = new BotConfiguration();
            _configuration.GetSection(nameof(BotConfiguration)).Bind(Configuration);

            _logger.LogInformation("Initialize shared client");
            Client = await InitializeClientAsync(Configuration);

            _logger.LogInformation("Initialize slash commands");
            await AddSlashCommandAsync(Configuration);

            _logger.LogInformation("Initializes and connects all shards");
            Client.Ready += Ready;
            await Client.StartAsync();
        }

        /// <summary>
        /// Initializes the client.
        /// </summary>
        /// <param name="configuration">The bot configuration.</param>
        private async Task<DiscordShardedClient> InitializeClientAsync(BotConfiguration configuration)
        {
            var logFactory = new LoggerFactory().AddSerilog();
            var client = new DiscordShardedClient(new DiscordConfiguration()
            {
                Token = configuration.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                AutoReconnect = true,
                LoggerFactory = logFactory,
            });

            await client.UseCommandsNextAsync(new CommandsNextConfiguration()
            {
                EnableDefaultHelp = true,
                EnableMentionPrefix = true,
            });

            return client;
        }

        /// <summary>
        /// Adds the slash commands.
        /// </summary>
        /// <param name="configuration">The bot configuration.</param>
        private async Task AddSlashCommandAsync(BotConfiguration configuration)
        {
            var slashCommands = await Client.UseSlashCommandsAsync(new SlashCommandsConfiguration());
            var slashCommandModules = ReflectionExtensions.InitializeClasses<ApplicationCommandModule>();

            foreach (var module in slashCommandModules)
            {
                slashCommands.RegisterCommands(module.GetType(), Worker.Configuration!.GuildId);
            }
        }

        /// <summary>
        /// Fired when the client enters ready state
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ReadyEventArgs"/> instance containing the event data.</param>
        private static async Task Ready(DiscordClient sender, ReadyEventArgs e)
        {
            await sender.UpdateStatusAsync(new DiscordActivity()
            {
                Name = $"Satisfactory server",
                ActivityType = ActivityType.Playing
            });
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disconnects and disposes of all shards.");
            if (Client != null)
                await Client.StopAsync();
        }
    }
}