
namespace BazePodatakaProjekat.Controllers;
using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly IGraphClient _client;

    public PostController(IGraphClient client)
    {
        _client = client;
    }

    [Route("getAllPostsFromUser/{userId}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> getAllPostsFromUser(string userId)
    {

        var posts = await _client.Cypher.Match("(p:Post)")
                                  .Where((Post p) => p.Id.ToString() == userId)
                                  .Return(p => p.As<Post>()).ResultsAsync;


        return Ok(posts);
    }

    [Route("createPost/{userId}")]
    [HttpPost]
    public async Task<IActionResult> CreatePost( String userId, [FromBody] Post post)
    {
        post.Id = Guid.NewGuid();



        await _client.Cypher.Create("(d:Post $post)")
                           .WithParam("post", post)
                           .ExecuteWithoutResultsAsync();

        await _client.Cypher.Match("(usr:User)", "(p:Post)")
                            .Where((User usr) => usr.Id == userId)
                            .AndWhere((Post p) => p.Id == post.Id)
                            .Create("(usr)-[r:Created]->(p)")
                            .ExecuteWithoutResultsAsync();


        return Ok();

    }
}