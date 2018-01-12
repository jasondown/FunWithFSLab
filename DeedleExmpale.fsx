#r "System.Xml.Linq.dll"
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

 
