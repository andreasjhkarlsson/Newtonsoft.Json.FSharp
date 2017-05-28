module Newtonsoft.Json.FSharp.Tests.UnionConverter

open Expecto

open System
open Newtonsoft.Json
open Newtonsoft.Json.FSharp

type HelloWorld =
  | HelloWorld of string * int

[<Tests>]
let simple =
  testList "can serialise normally" [
    testCase "simple string" <| fun _ ->
      Serialisation.serialiseNoOpts "hi" |> ignore

    testCase "simple sum type" <| fun _ ->
      let sample = HelloWorld ("hi", 12345)
      let name, data =
        Serialisation.serialiseNoOpts sample

      Expect.equal true (data.Length > 0) "should have data"

      let o = Serialisation.deserialiseNoOpts (typeof<HelloWorld>, data) :?> HelloWorld
      Expect.equal sample o "should eq structurally"
    ]

let sampleStr = "{\r\n  \"_name\": \"urn:Newtonsoft.Json.FSharp.Tests:UnionConverter_Event|Created\",\r\n  \"Created\": null\r\n}"

type Event = Created | Other

type A =
  | D1 of string * int
  | D2 of B
  | D3 of C
  | D4 of C * int
  | D5 of int * C
and B =
  | E1 of Guid
  | E2 of float * string * bigint * C
and C = { lhs : string; rhs : string }

module Inner =

  type Nabla =
    | N1

let converters =
  [ BigIntConverter() :> JsonConverter
    UnionConverter() :> JsonConverter
    TupleArrayConverter() :> JsonConverter ]

let test<'a when 'a : equality> : 'a -> unit = test converters

[<Tests>]
let complex =
  testList "union converter tests" [

    testCase "serialising simple union" <| fun _ ->
      test <| D1("hello", -2)

    testCase "serialising simple nested union" <| fun _ ->
      test <| Inner.N1

    testCase "deserialising simple union" <| fun _ ->
      Expect.notEqual (JsonConvert.DeserializeObject(sampleStr, typeof<Event>, UnionConverter())) null "should not be null"

    testCase "serialising union containing record" <| fun _ ->
      test <| D3({ lhs = "LHS" ; rhs = "RHS" })

    testCase "serialising union containing tuple containing record" <| fun _ ->
      test <| D4({ lhs = "LHS" ; rhs = "RHS" }, -23)

    testCase "serialising union containing tuple containing record after int" <| fun _ ->
      test <| D5(-43, { lhs = "LHS" ; rhs = "RHS" })

    testCase "serialising E2 - union containing tuple" <| fun _ ->
      test <| E2(0., "mystring", 2I, { lhs = "a"; rhs = "b" })

    testCase "serialising nested union" <| fun _ ->
      test <| D2(E1(Guid.Empty))

    testCase "serialising complex nested union" <| fun _ ->
      test <| D2(E2(-3.220000e03, "Goodbye World", 21345I, { lhs = "e"; rhs = "mail" }))

    ]
