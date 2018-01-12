﻿#r "System.Xml.Linq.dll"
#load "./packages/FsLab/FsLab.fsx"

open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

type WorldData = XmlProvider<"http://api.worldbank.org/countries/indicators/NY.GDP.PCAP.CD?date=2010:2010">

let indUrl = "http://api.worldbank.org/countries/indicators/"

let getData year indicator =
    let query =
        [ ("per_page","1000"); 
        ("date",sprintf "%d:%d" year year) ]
    let data = Http.RequestString(indUrl + indicator, query)
    let xml = WorldData.Parse(data)
    let orNaN value = 
        defaultArg (Option.map float value) nan
    series [ for d in xml.Datas -> d.Country.Value, orNaN d.Value ]

let wb = WorldBankData.GetDataContext()
let inds = wb.Countries.World.Indicators
let code = inds.``CO2 emissions (kt)``.IndicatorCode

let co2000 = getData 2000 code
let co2010 = getData 2010 code

// Plot the data for years 2000 and 2010
Chart.Geo(co2000)
Chart.Geo(co2010)

// Calculate the change (as %) over the last 10 years
let change = (co2010 - co2000) / co2000 * 100.0

// Plot the change between 2000 and 2010 and
// configure chart options (colors) to make it nicer
let cs = "#77D53D;#D1C855;#E8A958;#EA4C41;#930700"
let vs = [| -10;0;100;200;1000 |]
let axis = ColorAxis(values=vs, colors=cs.Split(';'))

Chart.Geo(change)
|> Chart.WithOptions(Options(colorAxis=axis))

let codes =
    [ "CO2", inds.``CO2 emissions (metric tons per capita)``
      "Uni-F", inds.``School enrollment, tertiary, female (% gross)``
      "Uni-M", inds.``School enrollment, tertiary, male (% gross)``
      "Life", inds.``Life expectancy at birth, total (years)``
      "Growth", inds.``GDP per capita growth (annual %)``
      "Pop", inds.``Population growth (annual %)``
      "GDP", inds.``GDP per capita (current US$)`` ]

let world = 
  frame [ for name, ind in codes -> 
            name, getData 2010 ind.IndicatorCode ]
