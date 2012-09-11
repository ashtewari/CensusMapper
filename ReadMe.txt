Windows 8 Metro App to visualize Census data on Bing maps.

* Windows 8
* WinRT
* XAML
* C#
* Census API
* Bing Maps API
* JSON.Net

Before Running - 

1. Get your Census Api key here - http://www.census.gov/developers/tos/key_request.html
2. Get your Bing Maps API key here - http://msdn.microsoft.com/en-us/library/ff428642.aspx
3. Create an xml file ApiKeys.xml. 
       
// ******************* ApiKeys.xml Begin **************************************    

<?xml version="1.0" encoding="utf-8" ?> 
<Keys>
  <Key name="Bing" value="YOUR_BING_MAPS_API_Key"></Key>
  <Key name="Census" value="YOUR_CENSUS_API_Key"></Key>
</Keys>

// ******************* ApiKeys.xml End **************************************

4. Replace the following values in ApiKeys.cs with your own keys -
YOUR_CENSUS_API_Key; 
YOUR_BING_MAPS_API_Key; 

5. Place the ApiKeys.xml file in the output folder where CensusMapper.exe is location.
