module Newtonsoft.Json.FSharp.Tests.OptionConverter

open Expecto

open System
open Newtonsoft.Json
open Newtonsoft.Json.FSharp

[<Tests>]
let expected_strings =
  let serialise (x : _ option) = 
    JsonConvert.SerializeObject(x, [| OptionConverter() :> JsonConverter |])
  let deserialise (str : string) =
    JsonConvert.DeserializeObject(str, [| OptionConverter() :> JsonConverter |])
    
  testList "for serialisation of option" [
    testCase "serialising option of int (None)" <| fun _ ->
      let str = serialise (None : int option)
      Expect.equal "null" str "should have null representation"

    testCase "serialising option of int (Some 2)" <| fun _ ->
      let str = serialise (Some 2)
      Expect.equal "2" str "should have null representation"

    testCase "deserialising option of int (2)" <| fun _ ->
      let res = deserialise "2" : int option
      Expect.equal (Some 2) res "should have (Some 2) representation"

    testCase "deserialising option of int (null)" <| fun _ ->
      let res = deserialise "null" : int option
      Expect.equal None res "should have (None) representation"
    ]