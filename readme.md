```
Reqs:
* azd cli
```

# Install AZD
```
brew tap azure/azd && brew install azd
```

# Add Dependencies
```
* cd voting-app.ColourService && dotnet restore 
* cd voting-app.VotesService && dotnet restore
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