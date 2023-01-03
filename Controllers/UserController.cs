
using BazePodatakaProjekat.Models;
//using BazePodatakaProjekat.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using Neo4jClient.Cypher;
using System.Web;
//using Microsoft.AspNetCore.Identity;
//using Neo4j.AspNet.Identity;
//using Microsoft.AspNetCore.Identity;

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
                        .With("n , n.Id as id")
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
    public async Task<IActionResult> CreateUser([FromBody] User user)
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


        var following = await _client.Cypher.Match("(usr1:User)", "(usr2:User)")
                            .Where((User usr1) => usr1.Id == userId)
                            .AndWhere((User usr2) => usr2.Id == followId)
                            .With("usr1,usr2, exists((usr1)-[:Following]->(usr2)) as ret")
                            //.Call("return exists((n) -[:Liked]->(:Post))")
                            .Return(ret => ret.As<bool>()).ResultsAsync;

        if(following.Single())
        {
            await _client.Cypher.Match("(usr1:User)-[r:Following]->(usr2)")
                           .Where((User usr1) => usr1.Id == userId)
                           .AndWhere((User usr2) => usr2.Id == followId)
                           .Delete("r")
                           .Set("usr2.NumbersOfFollowers = usr2.NumbersOfFollowers-1")
                           .Set("usr1.NumbersOfFollowings = usr1.NumbersOfFollowings-1")
                           .ExecuteWithoutResultsAsync();

        }
        else
        {
            await _client.Cypher.Match("(usr1:User)", "(usr2:User)")
                                .Where((User usr1) => usr1.Id == userId)
                                .AndWhere((User usr2) => usr2.Id == followId)
                                .Create("(usr1)-[r:Following]->(usr2)")
                                .Set("usr1.NumbersOfFollowings = usr1.NumbersOfFollowings+1")
                                .Set("usr2.NumbersOfFollowers = usr2.NumbersOfFollowers+1")
                                .ExecuteWithoutResultsAsync();

        }
        



        

        return Ok();
       
    }


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


   /* [AllowAnonymous]
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Registration([FromBody] Register reg)
    {

        if (ModelState.IsValid)
        {
            var userExist = await _userManager.FindByNameAsync(reg.UserName);
            if (userExist != null)
                return BadRequest("Ovaj username je vec u upotrebi!");

            var userEmail = await _userManager.FindByEmailAsync(reg.Email);
            if (userEmail != null)
                return BadRequest("Ovaj email je vec u upotrebi!");

            var applicationUser = new AppUser()
            {
                Name = reg.Name,
                Email = reg.Email,
                LastName = reg.LastName,
                UserName = reg.UserName,
                PhoneNumber = reg.Phone
            };
           *//* if (!await roleManager.RoleExistsAsync(UserRole.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
            }

            if (!await roleManager.RoleExistsAsync(UserRole.Moderator))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.Moderator));
            }
            if (!await roleManager.RoleExistsAsync(UserRole.LogedIn))
            {
                await roleManager.CreateAsync(new IdentityRole(UserRole.LogedIn));
            }*//*

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, reg.Password);
                //await userManager.AddToRoleAsync(applicationUser, UserRole.LogedIn);
                return Ok(result);
            }

            catch (Exception)
            {
                return BadRequest("Sifra mora da sadrzi najmanje 6 karaktera, da sadrzi jedno veliko slovo, jedan broj i jedan specijalni znak!");
            }
        }
        else
            return BadRequest("Podaci nisu validni!");

    }*/


}
