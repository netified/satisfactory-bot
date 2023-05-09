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

namespace SatisfactoryBot.Core
{
    using Extensions;
    using Models;
    using Modules;
    using System.Diagnostics;

    internal class ProcessFunctions
    {
        internal static async Task StartServer()
        {
            new Thread(StartProcess).Start();
            await Task.Delay(1000);
        }

        /// <summary>
        /// Stop the Satisfactory server.
        /// </summary>
        internal static Task StopServer(Process process)
        {
            process.Kill(true);
            Server.ServerInfo = new()
            {
                Id = 0,
                Status = ServerInfo.ServerStatus.Offline,
                Stopped = true
            };

            return Task.CompletedTask;
        }

        /// <summary>
        /// Start the Satisfactory server in a new CMD window.
        /// </summary>
        private static void StartProcess(object? obj)
        {
            Process cmd = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = @"c:\windows\system32\",
                    FileName = $"{Worker.Configuration!.ServerDirectory}\\FactoryServer.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = GetArgument()
                },
                EnableRaisingEvents = true
            };
            cmd.Exited += new EventHandler(ProcessExited);
            cmd.Start();

            Server.ServerInfo = new()
            {
                Id = cmd.Id,
                Status = ServerInfo.ServerStatus.Online,
            };

            cmd.WaitForExit();
        }

        private static string GetArgument()
        {
            if (string.IsNullOrEmpty(Worker.Configuration!.ServerIp))
                return "-log -unattended";
            return $"-multihome={Worker.Configuration!.ServerIp} -log -unattended";
        }

        /// <summary>
        /// Automatically restart the Satisfactory server after 5 seconds if it crashed.
        /// </summary>
        private static async void ProcessExited(object? sender, EventArgs e)
        {
            await Task.Delay(5000);
            var serverInfo = Server.ServerInfo;

            if (!serverInfo.Stopped)
            {
                Process? process = serverInfo.Id.GetProcess();
                if (process == null)
                    await StartServer();
            }
        }
    }
}