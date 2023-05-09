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

using SatisfactoryBot.Core;
using SatisfactoryBot.Extensions;
using SatisfactoryBot.Models;

namespace SatisfactoryBot.Modules
{
    using DSharpPlus.Entities;
    using DSharpPlus.SlashCommands;
    using DSharpPlus.SlashCommands.Attributes;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;

    [SlashRequireGuild]
    [SpecificGuildOnly]
    internal class Server : ApplicationCommandModule
    {
        internal static ServerInfo ServerInfo { get; set; } = new();

        internal class Messages
        {
            internal static string Success = "The server has been successfully {0}.";
            internal static string Error = "The server could not be {0}.";

            internal static string NotFound = "I could not find any process with the ID that was assigned to the server (PowerShell) window.";
            internal static string AlreadyRunning = "The server is already running.";
            internal static string NotRunning = "There is no server running";
        }

        [SlashCommand("Start", "Start the Satisfactory server")]
        public static async Task Start(InteractionContext ctx)
        {
            var title = nameof(Start);
            var type = "started";
            DiscordEmbedBuilder embed;

            Process? process = ServerInfo.Id.GetProcess();

            if (ServerInfo.Id == 0 || process == null)
            {
                await ProcessFunctions.StartServer();

                if (ServerInfo.Id == 0)
                    embed = new EmbedMsg.Error(ctx, title, string.Format(Messages.Error, type)).Embed;
                else
                    embed = new EmbedMsg.Success(ctx, title, string.Format(Messages.Success, type)).Embed;
            }
            else
                embed = new EmbedMsg.Error(ctx, title, Messages.AlreadyRunning).Embed;

            await Response.SendEmbed(ctx, embed);
        }

        [SlashCommand("Stop", "Stop the Satisfactory server")]
        public static async Task Stop(InteractionContext ctx)
        {
            var title = nameof(Stop);
            var type = "stopped";
            DiscordEmbedBuilder embed;

            if (ServerInfo.Id == 0)
                embed = new EmbedMsg.Error(ctx, title, Messages.NotRunning).Embed;
            else
            {
                Process? process = ServerInfo.Id.GetProcess();

                if (process == null)
                    embed = new EmbedMsg.Error(ctx, title, Messages.NotFound).Embed;
                else
                {
                    await ProcessFunctions.StopServer(process);
                    embed = new EmbedMsg.Success(ctx, title, string.Format(Messages.Success, type)).Embed;
                }
            }

            await Response.SendEmbed(ctx, embed);
        }

        [SlashCommand("Restart", "Restart the Satisfactory server")]
        public static async Task Restart(InteractionContext ctx)
        {
            var title = nameof(Restart);
            var type = "restarted";
            DiscordEmbedBuilder embed;

            if (ServerInfo.Id == 0)
                embed = new EmbedMsg.Error(ctx, title, Messages.NotRunning).Embed;
            else
            {
                Process? process = ServerInfo.Id.GetProcess();

                if (process == null)
                    embed = new EmbedMsg.Error(ctx, title, Messages.NotFound).Embed;
                else
                {
                    await ProcessFunctions.StopServer(process);
                    await ProcessFunctions.StartServer();

                    if (ServerInfo.Id == 0)
                        embed = new EmbedMsg.Error(ctx, title, $"{string.Format(Messages.Success, "stopped")} But it could not be restarted.").Embed;
                    else
                        embed = new EmbedMsg.Success(ctx, title, string.Format(Messages.Success, type)).Embed;
                }
            }

            await Response.SendEmbed(ctx, embed);
        }

        [SlashCommand("Status", "The status of the Satisfactory server")]
        public static async Task Status(InteractionContext ctx)
        {
            DiscordEmbedBuilder embed;
            var process = ServerInfo.Id.GetProcess();

            if (ServerInfo.Id == 0 || process == null)
                embed = new EmbedMsg.Error(ctx, nameof(Status), Messages.NotRunning).Embed;
            else
            {
                var difference = DateTime.UtcNow.Subtract(ServerInfo.StartTime);

                embed = new EmbedMsg.Success(ctx).Embed;
                embed.AddField("Status", ServerInfo.Status.ToString());
                embed.AddField("ID", process.Id.ToString());
                embed.AddField("Running for", $"{difference.Hours} hour(s), {difference.Minutes} minute(s), {difference.Seconds} second(s)");
                embed.AddField("Server Address", getExternalIP());
            }

            await Response.SendEmbed(ctx, embed);
        }

        private static string getExternalIP()
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString("https://api.ipify.org/");
            }
        }
    }
}