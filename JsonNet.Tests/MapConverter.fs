module Newtonsoft.Json.FSharp.Tests.MapConverter

open Expecto
open Newtonsoft.Json
open Newtonsoft.Json.FSharp

open System

module NestedModule =
  type PropWithMap =
    { b : Map<string, string> }

  type ThisFails =
    { a : string * string
      m : Map<string, string> }

  type ThisWorks =
    { y : Map<string, string>
      z : string * string }

  type ThirdAttempt =
    { t : string * string
      s : string }

open NestedModule

[<Tests>]
let mapTests =
  let mapSer o      = JsonConvert.SerializeObject(o, Serialisation.converters |> Array.ofList)
  let mapDeser aStr = JsonConvert.DeserializeObject<Map<string, int>>(aStr, Serialisation.converters |> Array.ofList)

  testList "map tests" [
    testCase "baseline: serialising empty map {} to string (defaults)" <| fun _ ->
      let js = mapSer Map.empty<string, int>
      Expect.equal js "{}" "js should equal {}"

    testCase "serialising empty map {} to string" <| fun _ ->
      Expect.equal (mapSer Map.empty<string, int>) "{}" "should equal {}"

    testCase "deserialising {} to Map.empty<string, int>" <| fun _ ->
      Expect.equal (mapDeser "{}") Map.empty<string, int> "should equal..."

    testCase @"deserialising { ""a"": 3 } to map [ ""a"" => 3 ]" <| fun _ ->
      let res = JsonConvert.DeserializeObject<Map<string, int>>("""{ "a": 3 }""", MapConverter())
      Expect.equal res ([("a", 3)] |> Map.ofList) "should equal..."

    testCase "serialising empty map roundtrip" <| fun _ ->
      test [MapConverter()] Map.empty

    testCase "serialising nonempty map roundtrip" <| fun _ ->
      test [MapConverter()] ([("a", 1); ("b", 2); ("c", 3)] |> Map.ofList)

    testCase "prop with map" <| fun () ->
      test Serialisation.converters { b = Map.empty }

    testCase "prop with map (explicit)" <| fun () ->
      let res = deserialise<PropWithMap> Serialisation.converters
                                  """{"b":{}}"""
      Expect.equal { b = Map.empty } res "should be eq to res"

    testCase "failing test - array before object" <| fun _ ->
      let res = deserialise<ThisFails> Serialisation.converters """{"a":["xyz","zyx"],"m":{}}"""
      Expect.equal { a = "xyz", "zyx"; m = Map.empty } res "should be eq to res"

    testCase "passing test - array after object" <| fun _ ->
      let res = deserialise<ThisWorks> Serialisation.converters """{"y":{},"z":["xyz","zyx"]}"""
      Expect.equal { y = Map.empty; z = "xyz", "zyx" } res "should be eq to res"

    testCase "string after tuple" <| fun _ ->
      let res = deserialise<ThirdAttempt> Serialisation.converters """{"t":["",""],"s":"xyz"}"""
      Expect.equal { t = "", ""; s = "xyz" } res "should be eq to res"


    //testProp "playing with map alias" <| fun (dto : NestedModule.ADto) ->
    //  test Serialisation.converters dto
    ]
