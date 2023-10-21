using DebridLinkFrNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DebridLinkFrNET.Test
{
    public class SeedboxTest
    {
        [Fact]
        public async Task List()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            var result = await client.Seedbox.ListAsync();

            Assert.True(result.Count != 0);
        }

        [Fact]
        public async Task AddTorrentFile()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String filePath = @"big-buck-bunny.torrent";

            var file = await System.IO.File.ReadAllBytesAsync(filePath);

            var result = await client.Seedbox.AddTorrentByFileAsync(file);

            Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.HashString);
        }

        [Fact]
        public async Task AddTorrent()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent";

            var result = await client.Seedbox.AddTorrentAsync(url);

            HttpClient _httpClient;
            _httpClient = new HttpClient();
            //_httpClient.Timeout = TimeSpan.FromMilliseconds(_settings.Timeout);
            Uri _uri;
            _uri = new Uri(result.Files.First().DownloadUrl);
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, _uri);
            httpRequestMessage.Headers.Range = new RangeHeaderValue(0, result.Files.First().Size-1);
            using HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, default);

            Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.HashString);
        }

        [Fact]
        public async Task ListWithGivenId()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const string id = "53263ca8ae0eea13260";

            var result = await client.Seedbox.ListAsync(id);

            Assert.Equal("dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c", result.First().HashString);
        }

        [Fact]
        public async Task Cached()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const string hash = "dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c";

            var result = await client.Seedbox.CachedAsync(hash);

            Assert.True(result != null);
        }

        [Fact]
        public async Task Start()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent";

            var newTorrent = await client.Seedbox.AddTorrentAsync(url, true);

            var unwanted = new List<string>();
            unwanted.Add(newTorrent.Files.First().Id);

            var result = await client.Seedbox.StartAsync(newTorrent.Id, unwanted.ToArray());

            Assert.True(result != null);
        }

        [Fact]
        public async Task Delete()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "magnet:?xt=urn:btih:dd8255ecdc7ca55fb0bbf81323d87062db1f6d1c&dn=Big+Buck+Bunny&tr=udp%3A%2F%2Fexplodie.org%3A6969&tr=udp%3A%2F%2Ftracker.coppersurfer.tk%3A6969&tr=udp%3A%2F%2Ftracker.empire-js.us%3A1337&tr=udp%3A%2F%2Ftracker.leechers-paradise.org%3A6969&tr=udp%3A%2F%2Ftracker.opentrackr.org%3A1337&tr=wss%3A%2F%2Ftracker.btorrent.xyz&tr=wss%3A%2F%2Ftracker.fastcast.nz&tr=wss%3A%2F%2Ftracker.openwebtorrent.com&ws=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2F&xs=https%3A%2F%2Fwebtorrent.io%2Ftorrents%2Fbig-buck-bunny.torrent";

            var addResult = await client.Seedbox.AddTorrentAsync(url);
           
            var id = addResult.Id;

            await client.Seedbox.DeleteAsync(id);


            var currHostedFile = await client.Seedbox.ListAsync(id);

            Assert.Empty(currHostedFile);
        }
    }
}
