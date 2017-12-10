# Progressive Web Applications Demo
This demo goes step by step in building a small Progressive Web Application using Visual Studio Code, ASP.NET Core 2.0, Bootstrap and a few other tools.

First start VS Code on an empty folder:

```
code .
´´´

Make sure you have the correct version of dotnet.exe and the three necessary templates:
```
dotnet --version
2.0.3

dotnet new --list
Templates                                         Short Name       Language          Tags
--------------------------------------------------------------------------------------------------------
xUnit Test Project                                xunit            [C#], F#, VB      Test/xUnit
ASP.NET Core Web App                              razor            [C#]              Web/MVC/Razor Pages
Solution File                                     sln                                Solution
´´´

Let's create the web project:
```
mkdir ProgressiveWebAppsDemo.Web
Push-Location ProgressiveWebAppsDemo.Web
dotnet new razor
Pop-Location
´´´

Now, lets create a unit-test project.
```
mkdir ProgressiveWebAppsDemo.Tests
Push-Location ProgressiveWebAppsDemo.Tests
dotnet new xunit
dotnet add reference ..\ProgressiveWebAppsDemo.Web\ProgressiveWebAppsDemo.Web.csproj
dotnet add package Microsoft.EntityFrameworkCore.InMemory
Pop-Location
´´´

Finally, we create a solution file and add both projects.
```
dotnet new sln
dotnet sln add .\ProgressiveWebAppsDemo.Tests\ProgressiveWebAppsDemo.Tests.csproj
dotnet sln add .\ProgressiveWebAppsDemo.Web\ProgressiveWebAppsDemo.Web.csproj
´´´

Since for this demo I'm not going to configure minification, we need to make sure we are running in the Development environment. Otherwise, we'll some javascript errors. Please set the following environment variable on your terminal before continuing:
```
$env:ASPNETCORE_ENVIRONMENT = "Development"
´´´

We could now run the web project, however there is now a handy watcher tool that's been added to ASP.NET Core 2.0 that we'll use through this demo. Lets add the NuGet package.

```
Push-Location ProgressiveWebAppsDemo.Web
dotnet add package Microsoft.DotNet.Watcher.Tools
´´´
Edit the .csproj to make the correspondig <PackageReference/> into a <DotNetCliToolReference/>. Then restore the package and run the site.

```
dotnet restore
dotnet watch run
´´´

Open your browser on http://localhost:5000/ and you should see the default ASP.NET Core web site.
