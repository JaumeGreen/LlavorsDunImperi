// Learn more about F# at http://fsharp.org

open System
open System.Data.SQLite
open System.IO

[<Literal>]
let Univers = "univers.sqlite"
[<Literal>]
let NombreJugadors = 14
[<Literal>]
let creaTaulaEstrelles =
    "create table Estrelles (" +
    "id int, " +
    "xpos int, ypos int, zpos, int" +
    "posesio int, " + 
    "poblacio int, " + 
    "fabriques int"+
    "reproduccio int)"
[<Literal>]
let creaTaulaFlotes =
    "create table Flotes(" +
    "id int, " +
    "xpos int, ypos int, zpos, int" +
    "posesio int, " + 
    "atac int, " + 
    "defensa int"+
    "colons int)"

//let ConnexioUnivers = sprintf "Data Source=%s;Version=3;" Univers  

type Posicio = {x:int;y:int;z:int}
type Estrella = {id:int; pos:Posicio; posesio:int ; poblacio:int; fabriques:int ; reproduccio: int}
type Flota = {id:int ; pos: Posicio; nom:string; posesio:int; atac:int; defensa:int; colons:int}

let processArguments (argv:string[]) = 
    match argv.Length<1 with
    | true -> None
    | _ -> match Path.IsPathRooted(argv.[0]) with
            | true -> Some argv.[0]
            | false -> Some (Path.Combine(Environment.CurrentDirectory, argv.[0]))

let creaEstrellesBase =
    let id = 0
    let rnd = System.Random()
    seq {
    for xP in 1..5 do
        for yP in 1..5 do
            for zP in 1..5 do
                let id = id+1
                let posicio = {x=xP;y=yP;z=zP}
                {id = id; pos= posicio; posesio=0; poblacio=0; fabriques=0; reproduccio = rnd.Next(11)-2}
    }

let asignaEstrella id jugador llista= 
    llista |> List.map (fun(estrella)-> if estrella.id=id then {estrella with posesio=jugador; poblacio=1000; fabriques=100; reproduccio=7} else estrella)

let creaEstrelles = 
    let estrelles = creaEstrellesBase
    estrelles |> asignaEstrella 1 1 
    
let creaUnivers baseDades =
    SQLiteConnection.CreateFile(Univers)
    let connectionString = sprintf "Data Source=%s;Version=3;" Univers 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()
    let comandaEstrelles = new SQLiteCommand(creaTaulaEstrelles, connection)
    comandaEstrelles.ExecuteNonQuery() |> ignore
    let comandaFlotes = new SQLiteCommand(creaTaulaFlotes, connection)
    comandaFlotes.ExecuteNonQuery() |> ignore


let dadesUnivers directory = 
    let baseDades = Path.Combine(directory, Univers)
    match File.Exists (baseDades) with
    | true -> extreuDades baseDades
    | false -> creaUnivers baseDades


[<EntryPoint>]
let main argv =
    match (processArguments argv) with 
    | Some x -> creaUnivers x
    | None -> printfn "Benvinguts a Llavors d'un Imperi. Cal un nom de directori per començar nova partida o dos per processar ordres." 
    0 // return an integer exit code
