Feature: XmlClientAdapterTest
  We test the different scenarios when using the XML adapter

Scenario: An error when connecting to the XML raises an Exception
	Given there is an error when connecting to the XML file
	Then there is an error when running the adapter

Scenario: The adapter parses the data correctly
	Given The data given is correct
	When the API is called
	Then the values returned by the adapter are correct