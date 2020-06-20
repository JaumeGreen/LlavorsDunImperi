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
    "xpos int, ypos int, zpos int," +
    "posesio int, " + 
    "poblacio int, " + 
    "fabriques int,"+
    "reproduccio int)"
[<Literal>]
let creaTaulaFlotes =
    "create table Flotes(" +
    "id int, " +
    "xpos int, ypos int, zpos int," +
    "posesio int, " + 
    "atac int, " + 
    "defensa int,"+
    "colons int)"

//let ConnexioUnivers = sprintf "Data Source=%s;Version=3;" Univers  

type Posicio = {x:int;y:int;z:int}
type Estrella = {id:int; pos:Posicio; posesio:int ; poblacio:int; fabriques:int ; reproduccio: int}
type Flota = {id:int ; pos: Posicio; posesio:int; atac:int; defensa:int; colons:int}
type Galaxia = {estrelles:Estrella list; flotes:Flota list} 

let processArguments (argv:string[]) = 
    match argv.Length<1 with
    | true -> None
    | _ -> match Path.IsPathRooted(argv.[0]) with
            | true -> Some argv.[0]
            | false -> Some (Path.Combine(Environment.CurrentDirectory, argv.[0]))

let creaEstrellesBase =
    let mutable id = 0
    let rnd = System.Random()
    [
    for xP in 1..5 do
        for yP in 1..5 do
            for zP in 1..5 do
                id <- id+1
                let posicio = {x=xP;y=yP;z=zP}
                {id = id; pos= posicio; posesio=0; poblacio=0; fabriques=0; reproduccio = rnd.Next(11)-2}:Estrella
    ]

let asignaEstrella id jugador (llista:Estrella list)= 
    llista |> List.map (fun(estrella)-> if estrella.id=id then {estrella with posesio=jugador; poblacio=1000; fabriques=100; reproduccio=7} else estrella)

let creaEstrelles = 
    let estrelles = creaEstrellesBase
    estrelles |> asignaEstrella 1 1 
        |> asignaEstrella 5 2
        |> asignaEstrella 12 3
        |> asignaEstrella 20 4
        |> asignaEstrella 25 5
        |> asignaEstrella 100 6 
        |> asignaEstrella 105 7
        |> asignaEstrella 112 8
        |> asignaEstrella 120 9
        |> asignaEstrella 125 10
    
let creaUnivers baseDades =
    SQLiteConnection.CreateFile(baseDades)
    let connectionString = sprintf "Data Source=%s;Version=3;" baseDades 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()
    let comandaEstrelles = new SQLiteCommand(creaTaulaEstrelles, connection)
    comandaEstrelles.ExecuteNonQuery() |> ignore
    let comandaFlotes = new SQLiteCommand(creaTaulaFlotes, connection)
    comandaFlotes.ExecuteNonQuery() |> ignore
    let insertSql = 
        "insert into Estrelles(id, xpos, ypos, zpos, posesio, poblacio, fabriques, reproduccio) " + 
        "values (@id, @xpos, @ypos, @zpos, @posesio, @poblacio, @fabriques, @reproduccio)"
    creaEstrelles |> List.map(fun x ->
        use command = new SQLiteCommand(insertSql, connection)
        command.Parameters.AddWithValue("@id", x.id) |> ignore
        command.Parameters.AddWithValue("@xpos", x.pos.y) |> ignore
        command.Parameters.AddWithValue("@ypos", x.pos.y) |> ignore
        command.Parameters.AddWithValue("@zpos", x.pos.z) |> ignore
        command.Parameters.AddWithValue("@posesio", x.posesio) |> ignore
        command.Parameters.AddWithValue("@poblacio", x.poblacio) |> ignore
        command.Parameters.AddWithValue("@fabriques", x.fabriques) |> ignore
        command.Parameters.AddWithValue("@reproduccio", x.reproduccio) |> ignore    
        command.ExecuteNonQuery())
    |> List.sum
    |> (fun recordsAdded -> printfn "Creades %d estrelles" recordsAdded)
    connection.Close()
    baseDades

let toEstrella (id, posicio, posesio, poblacio, fabriques, reproducio)=
    {id = id; pos= posicio; posesio=posesio; poblacio=poblacio; fabriques=fabriques; reproduccio = reproducio}:Estrella

let toFlota (id, posicio, posesio, atac, defensa, colons)=
    {id = id; pos= posicio; posesio=posesio; atac=atac; defensa=defensa; colons=colons}:Flota

let llegirDades transforma (comanda:SQLiteCommand)=
    let lector = comanda.ExecuteReader()
    seq { while lector.Read() do yield transforma (lector.GetInt32 0, {x=lector.GetInt32 1; y=lector.GetInt32 2; z=lector.GetInt32 3}, lector.GetInt32 4, lector.GetInt32 5, lector.GetInt32 6, lector.GetInt32 7 ) } 
    |> Seq.toList

let mostraEstrelles (dades:Estrella list)=
    dades |> List.map (fun x -> printfn " estrella %d a [%d,%d,%d] es de %d i te %d poblacio, %d fabriques i taxa de reproduccio es %d" x.id x.pos.x x.pos.y x.pos.z x.posesio x.poblacio x.fabriques x.reproduccio)

let mostraFlotes (dades:Flota list)=
    dades |> List.map (fun x -> printfn "flota %d a [%d,%d,%d] es de %d i te A/D/C %d/%d/%d" x.id x.pos.x x.pos.y x.pos.z x.posesio x.atac x.defensa x.colons)

let extreuDades baseDades = 
    let connectionString = sprintf "Data Source=%s;Version=3;" baseDades 
    let connection = new SQLiteConnection(connectionString)
    connection.Open()
    let readEstrelles = "select * from estrelles"
    let readFlotes = "select * from flotes"
    let comandaEstrelles = new SQLiteCommand(readEstrelles, connection)
    let estrelles = llegirDades toEstrella comandaEstrelles
    let comandaFlotes = new SQLiteCommand(readFlotes, connection)
    let flotes = llegirDades toFlota comandaFlotes
    connection.Close()
    mostraEstrelles estrelles |> ignore
    mostraFlotes flotes |> ignore
    {estrelles=estrelles;flotes=flotes}

let dadesUnivers directory = 
    let baseDades = Path.Combine(directory, Univers)
    match File.Exists (baseDades) with
    | true -> extreuDades baseDades
    | false -> creaUnivers baseDades |> extreuDades

[<EntryPoint>]
let main argv =
    match (processArguments argv) with 
    | Some x -> dadesUnivers x |>ignore
    | None -> printfn "Benvinguts a Llavors d'un Imperi. Cal un nom de directori per començar nova partida o dos per processar ordres." 
    0 // return an integer exit code
