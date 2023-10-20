using System;
using DebridLinkFrNET.Models;
using Newtonsoft.Json;

public class UserInfos
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [JsonProperty("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's email address is verified.
    /// </summary>
    [JsonProperty("emailVerified")]
    public bool EmailVerified { get; set; }

    /// <summary>
    /// Gets or sets the user's account type.
    /// </summary>
    [JsonProperty("accountType")]
    public int AccountType { get; set; }

    /// <summary>
    /// Gets or sets the remaining time of the premium subscription in seconds.
    /// </summary>
    [JsonProperty("premiumLeft")]
    public int PremiumLeft { get; set; }

    /// <summary>
    /// Gets or sets the user's points (pts).
    /// </summary>
    [JsonProperty("pts")]
    public int Pts { get; set; }

    /// <summary>
    /// Gets or sets the URL to upgrade the user's account.
    /// </summary>
    [JsonProperty("upgradeAccountUrl")]
    public string? UpgradeAccountUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL for the user's vouchers.
    /// </summary>
    [JsonProperty("vouchersUrl")]
    public string? VouchersUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL to edit the user's password.
    /// </summary>
    [JsonProperty("editPasswordUrl")]
    public string? EditPasswordUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL to edit the user's email address.
    /// </summary>
    [JsonProperty("editEmailUrl")]
    public string? EditEmailUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL to view the user's session ID (sessid).
    /// </summary>
    [JsonProperty("viewSessidUrl")]
    public string? ViewSessidUrl { get; set; }

    /// <summary>
    /// Gets or sets the user's registration date.
    /// </summary>
    [JsonProperty("registerDate")]
    public DateTime RegisterDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the server has detected the user.
    /// </summary>
    [JsonProperty("serverDetected")]
    public bool ServerDetected { get; set; }

    /// <summary>
    /// Gets or sets the user's settings.
    /// </summary>
    [JsonProperty("settings")]
    public UserSettings? Settings { get; set; }

    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    [JsonProperty("username")]
    public string? Username { get; set; }
}
