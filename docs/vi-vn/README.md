# Halcyon Core

Halcyon I / O là miễn phí, mã nguồn mở và nền tảng CMS dựa trên ASP.NET Core. Nó được xây dựng bằng cách sử dụng tốt nhất và các công cụ hiện đại nhất và ngôn ngữ (Visual Studio 2017, C # vv). Hãy là người tốt nhất và tham gia nhóm của chúng tôi!

- Demo: [http://demo.halcyoncore.net](http://demo.halcyoncore.net)
- Screenshot:  
  - Admin:
![Halcyon Core Admin Portal Bootstrap 4](https://halcyon-core.github.io/Halcyon-Core-Admin/img/white.png "Halcyon Core Admin Portal Bootstrap 4")

## Halcyon - swas·ti·ka (/ˈswästəkə/) mean:

 ![Halcyon History](http://docs.halcyoncore.net/_images/readme/halcyon-history.png)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

Note: This project is under heavy construction and is not intended for general production use yet. As such, we are not accepting bugs at the moment and documentation is quite lacking.

### Prerequisites

What things you need to install the software and how to install them

* [.NET](https://www.microsoft.com/net/core) - .NET Core framework
* [Visual Studio Community 2017](https://www.visualstudio.com/downloads/) - Free, fully-featured IDE for students, open-source and individual developers
* [SQL Server 2016+](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express) - Database server


### Installing

1. Download the source code from [Github](https://github.com/Halcyon-Core/Halcyon-Core)
2. Restore dotnet core Nuget's packages
```bash
cd [github-project-folder]\src\Halcyon.Cms.Web.Mvc]
dotnet restore
```
3. Build dotnet core packages
```bash
cd [github-project-folder]\src\Halcyon.Cms.Web.Mvc]
dotnet build
```
4. Then run! That it's!
```bash
cd [github-project-folder]\src\Halcyon.Cms.Web.Mvc]
dotnet run
```
5. Now you can access the site from your localhost (e.g. http://localhost:58511)

Note: Please read the step how to prepare MS SQL Server Database [here](/installing?id=step-2-create-the-database-and-a-user).

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/Halcyon-Core/Halcyon-Core/tags). 

## Authors

* **Smileway Team** - *Initial work* - [Smileway.co](http://www.smileway.co)

See also the list of [contributors](https://github.com/Halcyon-Core/Halcyon-Core/graphs/contributors) who participated in this project.

## References
(TBC)

## License

This project is licensed under the GPL-3.0 License - see the [LICENSE.md](LICENSE.md) file for details

## Thanks to

This project has been developed using:
* [Creative Tim](https://www.creative-tim.com/)
* [Bootstrap](https://getbootstrap.com/)
* [BrowserStack](https://www.browserstack.com/)
* [.NET](https://www.microsoft.com/net/core)
* And more...