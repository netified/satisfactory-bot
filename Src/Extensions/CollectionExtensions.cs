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

namespace SatisfactoryBot.Extensions
{
    using DSharpPlus.Entities;
    using DSharpPlus.Interactivity;
    using System;
    using System.Collections.Generic;

    internal static class CollectionExtensions
    {
        public static IEnumerable<Page> GeneratePagesInEmbeds(this ICollection<string?> input, string title = "")
        {
            if (input.Count == 0)
                throw new InvalidOperationException("You must provide a list of strings that is not null or empty!");

            var result = new List<Page>();
            var split = new List<string>();

            var row = 1;
            var msg = "";
            foreach (var s in input)
            {
                if (msg.Length + s.Length >= 2000)
                {
                    split.Add(msg);
                    msg = "";
                }
                msg += $"{s} \n";
                if (row >= input.Count)
                    split.Add(msg);
                row++;
            }

            var page = 1;
            foreach (var s in split)
            {
                result.Add(new Page
                {
                    Embed = new DiscordEmbedBuilder()
                    {
                        Title = title,
                        Description = s
                    }.WithFooter($"Page {page} / {split.Count}. ")
                });
                page++;
            }

            return result;
        }
    }
}