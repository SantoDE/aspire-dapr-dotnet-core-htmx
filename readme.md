# Reqs:
* Azure Developer CLI (AZD)
* dotnet Core runtime + SDK
* Dapr CLI


# Install AZD, dotnet Core runtime + SDK and Dapr CLI
```
# On a Mac
brew tap azure/azd && brew install azd
brew install dotnet
brew install --cask dotnet-sdk
brew install dapr/tap/dapr-cli

# Arch/Manjaro Linux
pamac install dotnet-runtime-bin dotnet-sdk-bin aspnet-runtime-bin dapr-cli-bin
```

To check if everything is installed correctly, run `dotnet --list-runtimes`. It should look like this:

```
dotnet --list-runtimes     
Microsoft.AspNetCore.App 8.0.6 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
Microsoft.NETCore.App 8.0.6 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```

Also check

```
$ azd version
$ dapr -h
```

# Add Dependencies
```
cd voting-app.ColourService && dotnet restore && cd ..
cd voting-app.VotesService && dotnet restore && cd ..
```

# Install Aspire.NET
Since Aspire isn't a Nuget package, we need to install it ourselves:
```
dotnet workload install aspire
```

If that gives a `Inadequate permissions. Run the command with elevated privileges.` run the command with a prefixed `sudo`.

# Build App (locally)
```
dotnet run --project voting-app.AppHost
```

# Deploy to Azure

```
azd auth login
azd init
azd up
```
