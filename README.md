# Satisfactory Bot

This is a simple bot for starting, stopping & restarting a Satisfactory server through Discord.
This bot itself does not contain any of the files for a Satisfactory server.

The idea is you put it in the directory where the Satisfactory server already exists, make the bot startup at boot and it will also automatically launch the Satisfactory server for you.

This bot works only with Windows for the moment.

## Install windows service

Open Command Prompt as an administrator.

Use the `sc` command to create a new service. The syntax is as follows:

```bash
sc create [ServiceName] binPath=[PathToExecutable]
```

Replace `[ServiceName]` with a name you want to give to the service. It can be anything you like, but it's best to keep it short and descriptive. For example, "SatisfactoryBotService".

Replace `[PathToExecutable]` with the full path to the executable file. In this case, it would be `D:\Games\SatisfactoryBot\SatisfactoryBot.exe`.

Add the `start` parameter to the `sc create` command to start the service immediately after it's created. The complete command would be:

```bash
sc create SatisfactoryBotService binPath="D:\Games\SatisfactoryBot\SatisfactoryBot.exe" start=auto
```

The `auto` value for the `start` parameter means that the service will start automatically when Windows starts up.

Once the service is created, you can manage it like any other Windows service. Use the `sc start` command to start the service, `sc stop` command to stop it, and `sc delete` command to remove it.

## Configure

Your `application.json` file must contain the following parameters to work.

```
{
  "BotConfiguration": {
    "Token": "[BOTTOKEN]",
    "GuildId": [DISCORDID],
    "ServerDirectory": "D:\\Games\\Satisfactory"
  }
}
```