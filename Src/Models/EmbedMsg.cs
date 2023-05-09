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

namespace SatisfactoryBot.Models
{
    using DSharpPlus.Entities;
    using DSharpPlus.SlashCommands;

    internal class EmbedMsg
    {
        public class Success
        {
            public DiscordEmbedBuilder Embed { get; set; }

            public Success(InteractionContext ctx, string? title = null, string? description = null)
            {
                Embed = new Standard(ctx, DiscordColor.Green, title, description).Embed;
            }
        }

        public class Error
        {
            public DiscordEmbedBuilder Embed { get; set; }

            public Error(InteractionContext ctx, string? title = null, string? description = null)
            {
                Embed = new Standard(ctx, DiscordColor.Red, title, description).Embed;
            }
        }

        public class Standard
        {
            public DiscordEmbedBuilder Embed { get; set; }

            public Standard(InteractionContext ctx, DiscordColor? color = null, string? title = null, string? description = null)
            {
                Embed = new()
                {
                    Footer = new()
                    {
                        Text = $"Executed by {ctx.User.Username}#{ctx.User.Discriminator}",
                        IconUrl = ctx.User.AvatarUrl
                    },
                };

                if (color == null)
                    Embed.Color = DiscordColor.DarkButNotBlack;
                else
                    Embed.Color = (DiscordColor)color;

                if (!string.IsNullOrWhiteSpace(title))
                    Embed.Title = title;

                if (!string.IsNullOrWhiteSpace(description))
                    Embed.Description = description;
            }
        }
    }
}