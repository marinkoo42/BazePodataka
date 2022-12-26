
using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace BazePodatakaProjekat.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IGraphClient _client;

    public UserController(IGraphClient client)
    {
        _client = client;
    }

    
    [Route("getMaxId")]
    [HttpGet]
    public async Task<IEnumerable<string>> getMaxId()
    {


        var str = await _client.Cypher.Match("(n:User)")
                        .With("n , ID(n) as id")
                        .OrderByDescending("id")
                        .Limit(1)
                        .Return(id => id.As<string>()).ResultsAsync;
                        
           //Return(k => k.As<string>() ).ResultsAsync;

        //.OrderBy("k desc").Limit(1)
       // var query = _client.Cypher.Match("(n) return ID(n) order by ID(n) desc limit 1");

        //((IRawGraphClient)_client).ExecuteGetCypherResultsAsync<String>(query).ToList().FirstOrDefault();
        

         

        return str;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _client.Cypher.Match("(n: User)")
                                              .Return(n => n.As<User>()).ResultsAsync;

        return Ok(users);
    }

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var users = await _client.Cypher.Match("(d:User)")
    //                                          .Where((User d) => d.id == id)
    //                                          .Return(d => d.As<User>()).ResultsAsync;

    //    return Ok(users.LastOrDefault());
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {



        await _client.Cypher.Create("(d:User $user)")
                            .WithParam("user", user)
                            .ExecuteWithoutResultsAsync();

        return Ok();
    }




}
