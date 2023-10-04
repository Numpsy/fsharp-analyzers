namespace GR.FSharp.Analyzers.Tests

module UnionCaseAnalyzerTests =

    open System.Collections
    open System.IO
    open NUnit.Framework
    open FSharp.Compiler.CodeAnalysis
    open FSharp.Analyzers.SDK.Testing
    open GR.FSharp.Analyzers
    open GR.FSharp.Analyzers.Tests.Common

    let mutable projectOptions : FSharpProjectOptions = FSharpProjectOptions.zero

    [<SetUp>]
    let Setup () =
        task {
            let! options = mkOptionsFromProject "net7.0" []

            projectOptions <- options
        }

    type TestCases() =

        interface IEnumerable with
            member _.GetEnumerator () : IEnumerator =
                constructTestCaseEnumerator [| "unioncase" |]

    [<TestCaseSource(typeof<TestCases>)>]
    let UnionCaseTests (fileName : string) =
        task {
            let fileName = Path.Combine (dataFolder, fileName)

            let! messages =
                File.ReadAllText fileName
                |> getContext projectOptions
                |> UnionCaseAnalyzer.unionCaseAnalyzer

            do! assertExpected fileName messages
        }

    type NegativeTestCases() =

        interface IEnumerable with
            member _.GetEnumerator () : IEnumerator =
                constructTestCaseEnumerator [| "unioncase" ; "negative" |]

    [<TestCaseSource(typeof<NegativeTestCases>)>]
    let NegativeTests (fileName : string) =
        task {
            let fileName = Path.Combine (dataFolder, fileName)

            let! messages =
                File.ReadAllText fileName
                |> getContext projectOptions
                |> UnionCaseAnalyzer.unionCaseAnalyzer

            Assert.IsEmpty messages
        }