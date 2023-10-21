using DebridLinkFrNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DebridLinkFrNET.Test
{
    public class DownloaderTest
    {
        [Fact]
        public async Task Add()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "url/0123"; //TODO: replace with a real url of a supported website

            var result = await client.Downloader.AddAsync(url);

            Assert.True(result != null && result.Count > 0);
        }

        [Fact]
        public async Task AddMultiFile()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "url/0123"; //TODO: replace with a real url of a supported website with a multi file

            var result = await client.Downloader.AddAsync(url);

            Assert.True(result != null && result.Count > 0);
        }

        [Fact]
        public async Task GetById()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String id = "id1234"; //TODO : Replace with a existing id on your account

            var result = await client.Downloader.GetByIdAsync(id); 

            Assert.Equal("filename", result.Name); //TODO : Replace with the name of the downloaded file.

            const String fakeId = "dsqqfsfqsd545fqs";

            var notFoundResult = await client.Downloader.GetByIdAsync(fakeId);

            Assert.Null(notFoundResult);
        }

        [Fact]
        public async Task Delete()
        {
            var client = new DebridLinkFrNETClient(Setup.ApiKey);

            const String url = "url/0123"; //TODO: replace with a real url of a supported website

            var addResult = await client.Downloader.AddAsync(url);
            var ids = string.Join(",", addResult.Select(link => link.Id));

            await client.Downloader.DeleteAsync(ids);


            var fileList = new List<HostedFile>();
            foreach (var id in addResult.Select(link => link.Id))
            {
                var currHostedFile = await client.Downloader.GetByIdAsync(id);
                if (currHostedFile != null)
                {
                    fileList.Add(currHostedFile);
                }
            }

            Assert.Empty(fileList);
        }
    }
}
