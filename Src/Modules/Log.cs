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

namespace SatisfactoryBot.Modules
{
    using DSharpPlus.Interactivity.Extensions;
    using DSharpPlus.SlashCommands;
    using Extensions;
    using System.Threading.Tasks;

    internal class Log : ApplicationCommandModule
    {
        [SlashCommand("Logs", "Retrieve he satisfactory server logs")]
        public static async Task Start(InteractionContext ctx)
        {
            var logFile = @$"{Worker.Configuration!.ServerDirectory}\FactoryGame\Saved\Logs\FactoryGame.log";
            var pages = WriteSafeReadAllLines(logFile)!.GeneratePagesInEmbeds("Server Logs");
            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages);
        }

        private static List<string?> WriteSafeReadAllLines(string path)
        {
            using var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(csv);

            var file = new List<string?>();
            while (!sr.EndOfStream)
                file.Add(sr.ReadLine());
            return file;
        }
    }
}