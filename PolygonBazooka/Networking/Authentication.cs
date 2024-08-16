using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PolygonBazooka.Networking;

public class Authentication
{
    public Cookie Token { get; private set; }
    public bool IsLoggedIn => Token != null;

    private bool _stayLoggedIn;

    public Authentication()
    {
        if (File.Exists("token.txt"))
            Token = new Cookie("token", File.ReadAllText("token.txt").TrimEnd());
    }

    public async Task<bool> LoginAsync(string username, string password, bool stayLoggedIn = false)
    {
        _stayLoggedIn = stayLoggedIn;

        var clientHandler = new HttpClientHandler();
        var client = new HttpClient(clientHandler);

        string uri = "http://localhost:8080/api/auth/login";

        var request = new HttpRequestMessage(HttpMethod.Post, uri);

        request.Content = new StringContent("{\"username\":\"" + username + "\",\"password\":\"" + password + "\"}",
            Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        
        var cookies = clientHandler.CookieContainer.GetCookies(new(uri));
        
        foreach (Cookie cookie in cookies)
        {
            if (cookie.Name == "token")
            {
                Token = cookie;
                break;
            }
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(string username, string password, string email, bool stayLoggedIn = false)
    {
        _stayLoggedIn = stayLoggedIn;

        var clientHandler = new HttpClientHandler();
        var client = new HttpClient(clientHandler);

        string uri = "http://localhost:8080/api/auth/register";
        
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(
            $"{{\"username\":\"{username}\",\"password\":\"{password}\",\"email\":\"{email}\"}}",
            Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);

        var cookies = clientHandler.CookieContainer.GetCookies(new(uri));
        
        foreach (Cookie cookie in cookies)
        {
            if (cookie.Name == "token")
            {
                Token = cookie;
                break;
            }
        }

        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        Token = null;

        if (File.Exists("token.txt"))
            File.Delete("token.txt");
    }

    public void Dispose()
    {
        if (!_stayLoggedIn)
            Logout();
        else
            File.WriteAllText("token.txt", Token.Value);
    }
}