
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
    public async Task<string> getMaxId()
    {


        var str = await _client.Cypher.Match("(n:User)")
                        .With("n , ID(n) as id")
                        .OrderByDescending("id")
                        .Limit(1)
                        .Return(id => id.As<string>()).ResultsAsync;

        var lista = str.Any();
        
        if (lista)
        {
            var s = str.Single();
            return s;

        }
        else return "-1";
    }

    [Route("getAllUsers")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _client.Cypher.Match("(n: User)")
                                              .Return(n => n.As<User>()).ResultsAsync;

        return Ok(users);
    }

    [Route("getUserById/{id}")]
    [HttpGet]
    public async Task<IActionResult> GetById(string id)
    {
        var users = await _client.Cypher.Match("(d:User)")
                                              .Where((User d) => d.Id == id)
                                              .Return(d => d.As<User>()).ResultsAsync;

        return Ok(users.LastOrDefault());
    }

    [Route("createUser")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user)
    {

        string result = getMaxId().Result;
        int resultInt = Int32.Parse(result);
        resultInt += 1;
        result = resultInt.ToString();
        user.Id = result;



        await _client.Cypher.Create("(d:User $user)")
                            .WithParam("user", user)
                            .ExecuteWithoutResultsAsync();

        return Ok();
    }



    [Route("followUser/{userId}/{followId}")]
    [HttpPut]
    public async Task<IActionResult> followUser(string userId, string followId)
    {
      
        await _client.Cypher.Match("(usr1:User)", "(usr2:User)")
                            .Where((User usr1) => usr1.Id == userId)
                            .AndWhere((User usr2) => usr2.Id == followId)
                            .Create("(usr1)-[r:Following]->(usr2)")
                            .ExecuteWithoutResultsAsync();

        

        return Ok();
       
    }

    //match(us:User)-[Following]->(f:User) where(us.Id = "0") return count(f)

    [Route("getUserFollowings/{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetUserFollowings(string userId)
    {
        var users = await _client.Cypher.Match("(d:User)-[Following]->(f:User)")
                                              .Where((User d) => d.Id == userId)
                                              .Return(f => new {
                                                  Id = f.As<User>().Id,
                                                  UserName = f.As<User>().UserName,
                                                  Email = f.As<User>().Email
                                              }).ResultsAsync;
                                              //f.As<User>().UserName).ResultsAsync;//f.As<User>()).ResultsAsync;

        return Ok(users);
    }

    [Route("getUserFollowingsCount/{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetUserFollowingsCount(string userId)
    {
        var users = await _client.Cypher.Match("(d:User)-[Following]->(f:User)")
            
                                              .Where((User d) => d.Id == userId)
                                              .Return(f => f.As<User>()).ResultsAsync;

        return Ok(users.Count());
    }


    [Route("getUserFollowers/{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetUserFollowers(string userId)
    {
        var users = await _client.Cypher.Match("(d:User)<-[Following]-(f:User)")
                                              .Where((User d) => d.Id == userId)
                                              .Return(f => new {
                                                  Id = f.As<User>().Id,
                                                  UserName = f.As<User>().UserName,
                                                  Email = f.As<User>().Email
                                              }).ResultsAsync;

        return Ok(users);
    }

    [Route("getUserFollowersCount/{userId}")]
    [HttpGet]
    public async Task<IActionResult> GetUserFollowersCount(string userId)
    {
        var users = await _client.Cypher.Match("(d:User)<-[Following]-(f:User)")

                                              .Where((User d) => d.Id == userId)
                                              .Return(f => f.As<User>()).ResultsAsync;

        return Ok(users.Count());
    }




}
