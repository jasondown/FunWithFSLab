//#load "packages/FsLab/Themes/DefaultWhite.fsx"
#load "./packages/FsLab/FsLab.fsx"

open FSharp.Data
open XPlot.GoogleCharts
open System
open System.Drawing

[<Literal>]
let sample5DayWeather = """
{"city":{"id":5992996,"name":"Kitchener","coord":{"lon":-80.4945,"lat":43.4532},"country":"CA","population":409112},"cod":"200","message":30.1393462,"cnt":7,"list":[{"dt":1515603600,"temp":{"day":5,"min":5,"max":5.83,"night":5.83,"eve":5,"morn":5},"pressure":996.68,"humidity":95,"weather":[{"id":500,"main":"Rain","description":"light rain","icon":"10d"}],"speed":10.47,"deg":210,"clouds":92,"rain":2.08},{"dt":1515690000,"temp":{"day":7.78,"min":5.6,"max":8.57,"night":8.57,"eve":7.85,"morn":5.6},"pressure":995.7,"humidity":90,"weather":[{"id":501,"main":"Rain","description":"moderate rain","icon":"10d"}],"speed":7.91,"deg":203,"clouds":92,"rain":4.67},{"dt":1515776400,"temp":{"day":-0.1,"min":-8.05,"max":3.85,"night":-8.05,"eve":-4.11,"morn":3.85},"pressure":991.28,"humidity":100,"weather":[{"id":601,"main":"Snow","description":"snow","icon":"13d"}],"speed":7.08,"deg":352,"clouds":92,"rain":6.98,"snow":11.25},{"dt":1515862800,"temp":{"day":-10.77,"min":-14.11,"max":-10.35,"night":-14.11,"eve":-12.57,"morn":-10.35},"pressure":1001.88,"humidity":0,"weather":[{"id":600,"main":"Snow","description":"light snow","icon":"13d"}],"speed":8.75,"deg":330,"clouds":29,"snow":1.49},{"dt":1515949200,"temp":{"day":-10.36,"min":-14.28,"max":-10.36,"night":-11.45,"eve":-11.92,"morn":-14.28},"pressure":1025.15,"humidity":0,"weather":[{"id":600,"main":"Snow","description":"light snow","icon":"13d"}],"speed":6.38,"deg":238,"clouds":7,"snow":0.21},{"dt":1516035600,"temp":{"day":-8.55,"min":-14.47,"max":-8.55,"night":-14.47,"eve":-11.34,"morn":-9.28},"pressure":1026.77,"humidity":0,"weather":[{"id":600,"main":"Snow","description":"light snow","icon":"13d"}],"speed":4.22,"deg":262,"clouds":91,"snow":0.28},{"dt":1516122000,"temp":{"day":-9.97,"min":-15.11,"max":-9.97,"night":-10.92,"eve":-11.46,"morn":-15.11},"pressure":1025.95,"humidity":0,"weather":[{"id":800,"main":"Clear","description":"sky is clear","icon":"01d"}],"speed":4.87,"deg":233,"clouds":17,"snow":0.03}]}
"""

type Weather = JsonProvider<sample5DayWeather>
let wb = WorldBankData.GetDataContext()

let apiKey = "Go get your own api key, it's free!"
let baseUrl = "http://api.openweathermap.org/data/2.5"
let forecastUrl = sprintf "%s/forecast/daily?units=metric&q=%s&appid=%s" baseUrl

let toHex (c : System.Drawing.Color) =
    String.Format("#{0:X6}", (c.ToArgb() &&& 0x00FFFFFF))

let colours = [| Color.Blue; Color.AliceBlue; Color.Green; Color.LightGreen; Color.Yellow; Color.Orange; Color.Red |] |> Array.map toHex
let values = [| -50; -35; -20; -5; 10; 25; 40 |]
let axis = ColorAxis(values=values, colors = colours)

let getTomorrowTemp place key =
    try
        let w = Weather.Load(forecastUrl place key)
        let tomorrow = Seq.head w.List
        tomorrow.Temp.Max |> Some
    with
        | _ -> None

let worldTemps =
    [ for c in wb.Countries ->
        let place = c.CapitalCity + "," + c.Name
        printfn "Getting temperature in: %s" place
        let temp = getTomorrowTemp place apiKey
        match temp with 
        | Some t -> Some (c.Name, t)
        | None -> None ]

worldTemps
|> List.choose id
|> Chart.Geo
|> Chart.WithTitle ("Temperatures (Based on Captial Cities)")
|> Chart.WithOptions(Options(colorAxis=axis))
|> Chart.WithLabel "Temp (C)"
