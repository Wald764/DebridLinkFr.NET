using System;
using System.Collections.Generic;
using System.Text;

namespace DebridLinkFrNET.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class BaseFile
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }

    public class TorrentFile : BaseFile
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("downloadPercent")]
        public int DownloadPercent { get; set; }
    }

    public class Torrent
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("hashString")]
        public string? HashString { get; set; }

        [JsonProperty("uploadRatio")]
        public double UploadRatio { get; set; }

        [JsonProperty("serverId")]
        public string? ServerId { get; set; }

        [JsonProperty("wait")]
        public bool Wait { get; set; }

        [JsonProperty("peersConnected")]
        public int PeersConnected { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("totalSize")]
        public long TotalSize { get; set; }

        [JsonProperty("files")]
        public List<TorrentFile>? Files { get; set; }

        [JsonProperty("trackers")]
        public List<Tracker>? Trackers { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("downloadPercent")]
        public int DownloadPercent { get; set; }

        [JsonProperty("downloadSpeed")]
        public int DownloadSpeed { get; set; }

        [JsonProperty("uploadSpeed")]
        public int UploadSpeed { get; set; }
    }

    public class Tracker
    {
        [JsonProperty("announce")]
        public string? Announce { get; set; }
    }
}
