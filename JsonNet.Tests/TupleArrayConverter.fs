module Newtonsoft.Json.FSharp.Tests.TupleArrayConverter
open Expecto

open System
open Newtonsoft.Json
open Newtonsoft.Json.FSharp

type TupleAlias = float * int
type TupleAlias2 = string * string

[<Tests>]
let tests =
  testList "tuple serialisation" [
    testCase "expected" <| fun _ ->
      let converter = TupleArrayConverter()
      test' converter ("", "")

    testCase "simple" <| fun _ ->
      test Serialisation.converters (0., 23 : TupleAlias)

    testCase "empty strings" <| fun _ ->
      test Serialisation.converters ("", "")

    testCase "empty strings (explicit)" <| fun _ ->
      let res = deserialise<TupleAlias2> Serialisation.converters """["", ""]"""
      Expect.equal ("", "") res "should be eq to res"
  ]