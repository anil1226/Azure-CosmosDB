
using CosmosDB;
using Microsoft.Azure.Cosmos;

string cosmosDBEndpointUri = "https://vayucos.documents.azure.com:443/";
string cosmosDBKey = "99WgFQb8Q6ankZrgwf5QNhM0BN9IF4SgB6ARopow93OF8R1U3fTA3bj0tRXkLXxBUP8FvvmoIt4QACDbzBHxBw==";
string databaseName = "appdb";
string containerName = "Orders";


await ReplaceItems();

async Task ReplaceItems()
{
    CosmosClient cosmosClient;
    cosmosClient = new CosmosClient(cosmosDBEndpointUri, cosmosDBKey);

    Database database = cosmosClient.GetDatabase(databaseName);
    Container container = database.GetContainer(containerName);

    string orderId = "O1";
    string sqlQuery = $"SELECT o.id,o.category FROM Orders o WHERE o.orderId='{orderId}'";

    string id="";
    string category = "";

    QueryDefinition queryDefinition = new QueryDefinition(sqlQuery);
    using FeedIterator<Order> feedIterator = container.GetItemQueryIterator<Order>(queryDefinition);

    while(feedIterator.HasMoreResults)
    {
        FeedResponse<Order> respose = await feedIterator.ReadNextAsync();
        foreach(Order order in respose)
        {
            id = order.id;
            category = order.category;
        }
    }

    
    // Lets just delete the item
    await container.DeleteItemAsync<Order>(id, new PartitionKey(category));
    Console.WriteLine("Item is deleted");
}
