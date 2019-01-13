# ExchangeServiceXML

## Functional Requirements
The local regional bank needs a web page developed for converting currencies. They currently generate an XML file from their Foreign exchange system. To keep costs down, they would like to use that as the data source for exchange rates. The file can be updated multiple times per day and results on the web page need to be up to date as quickly as possible.

The web page should contain 3 editable fields, one to select the From currency, one for the To currency and one for the user to enter the amount to be converted. The page should communicate with the server asynchronously to convert the amount between currencies (the page should not be reloaded when the user requests a currency to be converted). Currency exchange rates should be read from the supplied file (Currencies.xml). All exchange rates are in US Dollars.

## Assumptions

Since the data source is an XML file, this may cause issues if there is load balancing of the web servers. We take advantage of two features of Azure to solve the problem: the Redis cache and the Azure File Storage

## Features

Feature 1: the XML file is uploaded to Azure File Storage.
Advantages:
* Secure: the XML file can be only accessed by the web servers since it is within the Azure VPN. The secret SAS Token is required to access the file.
* High speed: the web servers can access the file at the speed of Azure's internal VPN, i.e. several Gigabytes per second.
* Single point: when switching to a new file, it can be done in a single location. There is no need to change it on all the web servers.

Feature 2: we use the Redis cache for speed. When a web server parses the data of the XML file, it stores the result into the Azure Redis cache, a high speed and high availability cache framework.
* High availability
* Solves the load balancing issues. We don't have independent caches on multiple web servers working independently. We have only one cache to maintain.

Front end features:
* Simple jQuery script with .NET MVC. Tested on IE, Chrome and FF
* Responsive design with Twitter Bootstrap

Back end features:
* Asynchronous programming
* Separation of concerns
* Unit testing using SpecFlow (BDD) and MOQ

Technology stack:
* C#7 with Visual Studio 2017
* .NET 4.7.2
* Simple Injector for its simplicity and its speed compared to other DI frameworks https://github.com/danielpalme/IocPerformance
