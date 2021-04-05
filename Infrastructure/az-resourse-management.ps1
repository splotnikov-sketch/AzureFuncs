#az login

$resourceGroupName = "FuncsResourceGroup"
$location = "eastus"
$storageAccountName = "funcsstorageaccount"
$azureFunctionsPremiumPlanName = "FuncsPremiumPlan"
$appName = "SimpleFuncs"
#$appInsightsName = $appName + "Insight";

az group create `
    -l $location `
    -n $resourceGroupName

az storage account create `
    -g $resourceGroupName `
    -n $storageAccountName `
    -l $location `
    --sku Standard_LRS

az functionapp plan create `
    --resource-group $resourceGroupName `
    --name $azureFunctionsPremiumPlanName `
    --location $location `
    --number-of-workers 1 `
    --sku EP1 `
    --is-linux

az functionapp create --name $appName `
    --storage-account $storageAccountName `
    --resource-group $resourceGroupName `
    --plan $azureFunctionsPremiumPlanName `
    --runtime dotnet `
    --functions-version 3

az functionapp deployment slot create `
    --resource-group $resourceGroupName `
    --name $appName `
    -slot staging    

az cosmosdb create `
    --name $storageAccountName `
    --resource $resourceGroupName `
    --default-consistency-level Eventual `
    --locations regionName=$location failoverPriority=0 isZoneRedundant=False 


$appInsightsInstrumentationKey="$(az resource show -g $resourceGroupName -n $appName --resource-type 'Microsoft.Insights/components' --query properties.InstrumentationKey)"
# this instrumentation key comes with quotes, which we need to remove
$appInsightsInstrumentationKey = $appInsightsInstrumentationKey -replace '"', ""

#storage connection string
$storageConnectionString = "$(az storage account show-connection-string `
    --resource-group $($resourceGroupName) `
    --name $($storageAccountName) `
    --query connectionString `
    --output tsv)"

$cosmosConnectionString = "$(az cosmosdb keys list `
   --type connection-strings `
   --name $storageAccountName `
   --resource-group $resourceGroupName `
   --query connectionStrings[0].connectionString)"


$settings = @(
  "APPINSIGHTS_INSTRUMENTATIONKEY=$appInsightsInstrumentationKey",
  "AzureWebJobsStorage=$storageConnectionString",
  "WEBSITE_RUN_FROM_PACKAGE=1",
  "Cosmos:ConnectionString=$cosmosConnectionString",
  "Cosmos:DatabaseId=FunctionsData",
  "Cosmos:ContainerId=Primes"
)

az webapp config appsettings set `
    --resource-group $resourceGroupName `
    --name $appName `
    --settings $settings `

//----------

// CI-CD

az functionapp deployment container config --enable-cd --query CI_CD_URL `
    --output tsv `
    --name $appName `
    --resource-group $resourceGroupName

az functionapp deployment container show-cd-url --name $appName `
    --resource-group $resourceGroupName

func azure functionapp fetch-app-settings $appName    