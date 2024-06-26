```
Reqs:
* azd cli
* az cli
* terraform
```

# Install AZD
```
brew tap azure/azd && brew install azd
```

# Add Dependencies
```
* cd voting-app.BeerService && dotnet restore 
* cd voting-app.VotesService && dotnet restore
```

# Build App (locally)
```
dotnet run --project voting-app.AppHost
```

# Deploy to Azure

```
azd auth login
azd up
```

# add api management

```
az login
az account set --subscription <SUBSCRIPTION_ID>
```

Setup terraform

```
terraform init
terraform apply
```

(wait 40 minutes) :)

# Test
```
curl -X 'GET' \
  'https://aspire-apim-test.azure-api.net/beers' \
  -H 'accept: */*'
```

```
curl -X 'GET' \
  'https://aspire-apim-test.azure-api.net/beer/votes?beer=Heineken' \
  -H 'accept: */*'
```

```
curl -X 'POST' \
  'https://aspire-apim-test.azure-api.net/beer/vote' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "name": "Heineken"
}'
```

```
curl -X 'GET' \
  'https://aspire-apim-test.azure-api.net/beers/votes?beer=Heineken' \
  -H 'accept: */*'
```

# Publish Portal
You need to log into the Azure Portal and publish the portal through API Management Service