#load "packages/FsLab/Themes/DefaultWhite.fsx"
#load "./packages/FsLab/FsLab.fsx"

open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

type Weather = JsonProvider<"http://api.openweathermap.org/data/2.5/forecast/daily?units=metric&q=Kitchener,CA&appid=APIKEYHERE">
let w = Weather.GetSample()

for day in w.List do
    printfn "%A" day.Weather

