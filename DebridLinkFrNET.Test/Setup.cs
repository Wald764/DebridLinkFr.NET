using System.Net;

namespace DebridLinkFrNET.Test;

public static class Setup
{
    public const string FakeApiKey = "test-api-key-12345";

    public static (DebridLinkFrNETClient Client, MockHttpMessageHandler Handler) CreateMockClient()
    {
        var handler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(handler);
        var client = new DebridLinkFrNETClient(FakeApiKey, httpClient);
        return (client, handler);
    }

    public static class Responses
    {
        public const string AccountInfos = @"{
            ""success"": true,
            ""value"": {
                ""email"": ""test@example.com"",
                ""emailVerified"": true,
                ""username"": ""TestUser"",
                ""accountType"": 1,
                ""premiumLeft"": 2592000,
                ""pts"": 100,
                ""registerDate"": ""2021-01-01T00:00:00Z""
            }
        }";

        public const string TorrentList = @"{
            ""success"": true,
            ""value"": [
                {
                    ""id"": ""53263ca8ae0eea13260"",
                    ""name"": ""Big Buck Bunny"",
                    ""hashString"": ""dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c"",
                    ""status"": 6,
                    ""totalSize"": 276445467,
                    ""downloadPercent"": 100,
                    ""downloadSpeed"": 0,
                    ""uploadSpeed"": 0,
                    ""uploadRatio"": 0.5,
                    ""peersConnected"": 0,
                    ""created"": 1609459200,
                    ""serverId"": ""1"",
                    ""wait"": false,
                    ""files"": [
                        {
                            ""id"": ""file1"",
                            ""name"": ""big-buck-bunny.mp4"",
                            ""size"": 276445467,
                            ""downloadUrl"": ""https://example.com/download/big-buck-bunny.mp4"",
                            ""downloadPercent"": 100
                        }
                    ],
                    ""trackers"": [
                        { ""announce"": ""udp://tracker.opentrackr.org:1337"" }
                    ]
                }
            ]
        }";

        public const string TorrentSingle = @"{
            ""success"": true,
            ""value"": {
                ""id"": ""53263ca8ae0eea13260"",
                ""name"": ""Big Buck Bunny"",
                ""hashString"": ""dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c"",
                ""status"": 6,
                ""totalSize"": 276445467,
                ""downloadPercent"": 100,
                ""downloadSpeed"": 0,
                ""uploadSpeed"": 0,
                ""uploadRatio"": 0.5,
                ""peersConnected"": 0,
                ""created"": 1609459200,
                ""serverId"": ""1"",
                ""wait"": false,
                ""files"": [
                    {
                        ""id"": ""file1"",
                        ""name"": ""big-buck-bunny.mp4"",
                        ""size"": 276445467,
                        ""downloadUrl"": ""https://example.com/download/big-buck-bunny.mp4"",
                        ""downloadPercent"": 100
                    }
                ],
                ""trackers"": [
                    { ""announce"": ""udp://tracker.opentrackr.org:1337"" }
                ]
            }
        }";

        public const string CachedResult = @"{
            ""success"": true,
            ""value"": {
                ""dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c"": {
                    ""id"": ""cached1"",
                    ""name"": ""Big Buck Bunny"",
                    ""hashString"": ""dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c"",
                    ""status"": 6,
                    ""totalSize"": 276445467,
                    ""downloadPercent"": 100,
                    ""created"": 1609459200,
                    ""serverId"": ""1"",
                    ""wait"": false,
                    ""files"": [],
                    ""trackers"": []
                }
            }
        }";

        public const string DownloaderAddResult = @"{
            ""success"": true,
            ""value"": [
                {
                    ""id"": ""hosted123"",
                    ""name"": ""test-file.zip"",
                    ""size"": 1048576,
                    ""url"": ""https://example.com/file"",
                    ""downloadUrl"": ""https://example.com/download/test-file.zip"",
                    ""host"": ""example.com"",
                    ""created"": 1609459200,
                    ""expired"": false
                }
            ]
        }";

        public const string DownloaderGetById = @"{
            ""success"": true,
            ""value"": {
                ""id"": ""hosted123"",
                ""name"": ""test-file.zip"",
                ""size"": 1048576,
                ""url"": ""https://example.com/file"",
                ""downloadUrl"": ""https://example.com/download/test-file.zip"",
                ""host"": ""example.com"",
                ""created"": 1609459200,
                ""expired"": false
            }
        }";

        public const string EmptyList = @"{
            ""success"": true,
            ""value"": []
        }";

        public const string DeleteSuccess = @"{
            ""success"": true,
            ""value"": {}
        }";

        public const string StartResult = @"{
            ""success"": true,
            ""value"": [""file1""]
        }";

        public const string NotFound = @"{
            ""success"": false,
            ""error"": ""not_found""
        }";
    }
}
