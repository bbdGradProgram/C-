using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Server.Services {
public class GitHubOAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserService _userService;

    public GitHubOAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,IUserService userService,
        UrlEncoder encoder,
        IHttpClientFactory httpClientFactory)
        : base(options, logger, encoder)
    {
        _httpClientFactory = httpClientFactory;
         _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Authorization header missing.");

        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers.Authorization);
        var accessToken = authHeader.Parameter;

        // Validate the access token with GitHub
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("AppName", "1.0"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync("https://api.github.com/user");
        if (!response.IsSuccessStatusCode)
            return AuthenticateResult.Fail("Invalid access token.");

        var githubUserData = await JsonSerializer.DeserializeAsync<GitHubUser>(await response.Content.ReadAsStreamAsync());
        if (githubUserData == null)
            return AuthenticateResult.Fail("Failed to deserialize GitHub user data.");

        var userId = await _userService.GetUserIDFromGithubIDAsync(githubUserData.id);
        
        // GitHub's API endpoint for fetching user emails
        var emailResponse = await client.GetAsync("https://api.github.com/user/emails");
        if (!emailResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch GitHub emails: {emailResponse.StatusCode}");
            return null;
        }

        var emailJson = await emailResponse.Content.ReadAsStringAsync();
        var emailList = JsonSerializer.Deserialize<List<GitHubEmail>>(emailJson);

        if (emailList == null)
        {
            Console.WriteLine("Failed to deserialize GitHub email response.");
            return null;
        }
        var primaryEmail = emailList.FirstOrDefault(email => email.primary)?.email;

        if (userId == null)
        {
            try{
            var newUser = await _userService.CreateOrUpdateUserAsync(githubUserData.id, primaryEmail!, githubUserData.login);
            userId = newUser.UserID;
            }
            catch(Exception ex){
                AuthenticateResult.Fail(ex.Message);
            }

        }
        // Construct the claims and principal
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()!),
            new Claim(ClaimTypes.Name, githubUserData.login),
            new Claim(ClaimTypes.Email, primaryEmail!),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }

    private class GitHubUser
    {
        public int id { get; set; }
        public string login { get; set; } // GitHub username
        public string email { get; set; }
    }

    private class GitHubEmail
    {
        public string email { get; set; }
        public bool primary { get; set; }
    }
}
}