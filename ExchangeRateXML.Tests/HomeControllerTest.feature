Feature: HomeControllerTest
    We test that the cache and XML repositories are called, depending on the situtation

Scenario: The cache is called when accessing the Index page
	Given there is a Home Controller
 	And the cache is not null or outdated
  When the user accesses the Index page
  Then the currencies should be returned

Scenario: The XML is called if the cache is empty when accessing the Index page
	Given there is a Home Controller
 	And the cache is null or outdated
  When the user accesses the Index page
  Then XML should be called

Scenario: Requesting the same base and target currencies should return 1
	Given there is a Home Controller
  When the user requests a conversion of the same currencies
  Then the result should be 1

Scenario: Requesting the conversion of two set currencies should return the correct result
	Given there is a Home Controller
 	And the cache is not null or outdated
  When the user requests a conversion of two different set currencies
  Then the result should be 2