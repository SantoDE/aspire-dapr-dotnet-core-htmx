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

Check if Aspire is now listed with `dotnet workload list`:

```
$ dotnet workload list

Installed Workload Id      Manifest Version      Installation Source
--------------------------------------------------------------------
aspire                     8.0.2/8.0.100         SDK 8.0.300 
```

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

# Troubleshooting on Linux

The command `dotnet run --project voting-app.AppHost` might not run out-of-the-box.

If you get the following error ([raised by this code in Aspire](https://github.com/dotnet/aspire/blob/03e9633c8b79a344be60286b8fc774c2525d1444/src/Aspire.Hosting.Dapr/DaprDistributedApplicationLifecycleHook.cs#L297)):

```
Unhandled exception. System.AggregateException: One or more errors occurred. (Unable to locate the Dapr CLI.)
 ---> Aspire.Hosting.DistributedApplicationException: Unable to locate the Dapr CLI.
```

Your `dapr` runtime might not be located in a directory like `usr/local/bin/dapr`. In this case find your dapr runtime (on a Linux/Manjaro machine it was located at `/usr/bin/dapr`) and symlink it to `/usr/local/bin/dapr` like this:

```
sudo ln -s /your/dapr/runtime/location /usr/local/bin/dapr
```

If you're on Linux and got a warning regarding an untrusted ASP.NET Core developer certificate:

```
dotnet run --project voting-app.AppHost      
Building...
warn: Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServer[8]
      The ASP.NET Core developer certificate is not trusted. For information about trusting the ASP.NET Core developer certificate, see https://aka.ms/aspnet/https-trust-dev-cert.
```

you might get Exceptions inside the Aspire dashboard and you don't see any app etc.

Then you need to trust the certificate in Linux, which doesn't work via [the provided link's statement](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-8.0&tabs=visual-studio%2Clinux-ubuntu#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos) `dotnet dev-certs https --trust`! But [what works is to install linux-dev-certs](https://github.com/dotnet/aspnetcore/issues/32842#issuecomment-2069720475): 

```
# install linux-dev-certs as dotnet global tool
dotnet tool update -g linux-dev-certs

# Add the following to your ~/.bash_profile or ~/.zshrc (otherwise dotnet wont find the command)
# Add .NET Core SDK tools
export PATH="$PATH:/home/jonashackt/.dotnet/tools"

# install linux-dev-certs
dotnet linux-dev-certs install
```

The warning might persist, but the Aspire Dashboard should work now!
