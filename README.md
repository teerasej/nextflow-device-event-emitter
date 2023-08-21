# Nextflow Event Emitter sample

This console application is build to emitted a series of JSON data to Azure Event Hubs.

## appsettings.json

We have to create `/appsettings.json` with following keys:

```json
{
  "EventHubSettings": {
    "ConnectionString": "YOUR_EVENTHUB_CONNECTION_STRING",
    "Name": "YOUR_EVENTHUB_NAME"
  }
}
```