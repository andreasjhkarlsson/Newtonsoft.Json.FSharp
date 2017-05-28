module Newtonsoft.Json.FSharp.Tests.IntegrationTests

open System
open Expecto

open Newtonsoft.Json
open Newtonsoft.Json.FSharp

[<Tests>]
let expected_strings =
  testList "expected strings" [
    testCase "serialising list of ints" <| fun _ ->
      let settings =
        new JsonSerializerSettings()
        |> Newtonsoft.Json.FSharp.Serialisation.extend
      let str = JsonConvert.SerializeObject([1; 2; 3], settings)
      Expect.equal "[1,2,3]" str "should have correct representation"

  ]