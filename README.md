# Cat Facts App

> A robust .NET console application that fetches random cat facts from a public API and logs them to a local text file.

This project is built to demonstrate modern C# development practices, focusing on clean architecture, resilience, and higth testability.

## About The Project

The application showcases the following technical highlights and architectural decisions:
*	**Clean Architecture:** Strict separation of concerns utilizing interfaces ('ICatFactClient', IFileService') and native Dependency Injection.
*	**Resilient Networking:** Integrates **Polly** for transient fault handling ('WaitAndRetryAsync') ensuring stable communication with the external API.
*	**Options Pattern:** Strongly typed configuration managed seamlessly via 'appsettings.json'
*	**Testability:** Core logic is covered by unit tests using custom 'HttpMessageHandler' mocks to simulate API responses without external dependencies.

## Getting Started

To get a local copy up and running follow these simple steps:

*	Ensure you have the latest .NET SDK installed on your local machine.
*	Clone this repository to your preferred directory.
*	Open your terminal in the root folder (where the '.slnx' or '.sln' file is located).
*	Execute the application by running: 'dotnet run --project CatFactsApp'.
	
## Configuration

The application is highly configurable. You can modify the API endpoint or the output file name by editing the 'CatFactsApp/appsettings.json' file:

```json
{
	"AppOptions":{
		"ApiUrl": "[https://catfact.ninja/fact](https://catfact.ninja/fact)",
		"OutputFilePath": "facts.txt"
}
```

## Running Tests

The solution includes a dedicated xUnit test project (CatFactsApp.Tests) that verifies the HTTP client behavior against valid and invalid data.

*	Open your terminal in the root directory.
*	Run the test suite using the standard .NET CLI command: 'dotnet test'.
* 	The test will automatically validate the client logic and provide a green pass status.
