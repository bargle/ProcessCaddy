# ProcessCaddy
A simple C# application for starting and monitoring other processes.

### Usage
ProcessCaddy is configured via a single JSON file which lists each application that is to be monitored. Each process can be maintained individually, or all applications can be started/stopped simultaneously using the Processes menu Start All and Stop All options. Once an application has been started via the menu options, it will restart automatically if it exits outside of the control of ProcessCaddy.

## Main Window
![image](ProcessCaddy/img/main_dialog.png)

## Configuration
ProcessCaddy requires a config.json file to live in the same folder as the EXE. It is a simple listing of the applications that are to be controlled and monitored. Each entry has 3 variables, name, path to exe, and command line arguments. The "args" variable is optional and can be omitted if not needed.

## Example config.json
```
[
  {
    "name" : "MyApp",
    "exec" : "C:\\MyFolder\\MyApp.exe",
    "args" : "-force_awesome 1"
  }
]
```

The double backslashes are required for the JSON file to be properly parsed.

## Discord Webhooks
Version 1.4 brings support for reporting start/stop/exit events to a Discord channel via Discord's Webhook API. 
The discord.json file controls this functionality.

To enable these events, create a webhook on the channel and grab the URL generated. Create a discord.json file next to the ProcessCaddy EXE in the following format:

## Example discord.json
```
[
  {
    "enabled" : true,
    "URI" : "https://discordapp.com/api/webhooks/<CHANNEL_ID>/<HOOK>"
  }
]
```
