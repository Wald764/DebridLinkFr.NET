# DebridLinkFr.NET

DebridLinkFr .NET wrapper library written in C#

Greatly inspired by the following repositories which do the same thing but for other sites:
- https://github.com/rogerfar/Alldebrid.NET/
- https://github.com/rogerfar/RD.NET
- https://github.com/rogerfar/Premiumize.NET

## Usage

Create an instance of `DebridLinkFrNETClient` for each user you want to authenticate. If you need to support multiple users you will need to create a new instance every time you switch users.

```csharp
var client = new DebridLinkFrNETClient("api key");
```

Pass in the Agent name you wish to use for your application. This can be any meaningfull name.

Pass in the Api Key for the user. If you don't have an API key yet, you can leave this blank and use Pin authentication.

The method naming  must follows the API documentation when possible:
```csharp
var client = new DebridLinkFrNETClient("api key");

var result = await client.Torrent.AddAsync(magnet);
```

The following API calls are available:
```csharp
var client = new DebridLinkFrNETClient("api key");

client.Account.
client.Seedbox.
client.Downloader.
```

## Authentication

Each user has its own API key, which can be found here: <https://debrid-link.fr/webapp/apikey>.

## Unit tests

The unit tests are not designed to be ran all at once, they are used to act as a test client.

Create a file `setup.txt` and put your API token in there.

Some functions will need replacement ID's to work properly.