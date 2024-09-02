# WebsiteMonitor
an automated application designed to monitor the status of websites by making HTTP requests and reporting the status code responses. It aims to provide developers with a simple yet effective tool for monitoring multiple websites simultaneously, receiving timely alerts in case of downtime, and accessing uptime reports for analysis.

## Branch
[WebsiteMonitor/feat/DPP-7/add-API](https://github.com/Website-Monitor-Inc/WebsiteMonitor/tree/feat/DPP-7/add-API)

## API Design
[API Design Documentation](https://lavanianaidoo1.atlassian.net/wiki/spaces/~71202023eaed64944d476b9cdd0ad8e4d3da21/pages/4390913/API+Design)

## Running the API
To run the API, navigate to the Server directory and use the following command:
> `dotnet run --launch-profile https`

## If it encounters security issues, try running:
> `dotnet run`

This will create a server on HTTP, which should still function correctly.

After the API is up, open [Swagger](https://localhost:7212/swagger/index.html/) to view all the endpoints. Note that the port might differ, and it will be displayed after launching the API.

## Reference
[Tutorial: Create a web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio-code)


