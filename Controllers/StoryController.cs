namespace BazePodatakaProjekat.Controllers;
using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

[ApiController]
[Route("[controller]")]
public class StoryController : ControllerBase
{
    private readonly IGraphClient _client;

    public StoryController(IGraphClient client)
    {
        _client = client;
    }

    [Route("createStory/{userId}")]
    [HttpPost]
    public async Task<IActionResult> CreateStory(String userId, [FromBody] Story story)
    {
        story.Id = Guid.NewGuid();
        story.DateTimeCreated = DateTime.Now;


        await _client.Cypher.Create("(d:Story $story)")
                           .WithParam("story", story)
                           .ExecuteWithoutResultsAsync();

        await _client.Cypher.Match("(usr:User)", "(s:Story)")
                            .Where((User usr) => usr.Id == userId)
                            .AndWhere((Story s) => s.Id == story.Id)
                            .Create("(usr)-[r:Published]->(s)")
                            .ExecuteWithoutResultsAsync();


        return Ok("stojanee");

    }
    [Route("deleteStory/{storyId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteStory(string storyId)
    {
        await _client.Cypher.Match("(usr:User)-[p:Published]->(pst:Post)")
                            .Where((Post pst) => pst.Id.ToString(storyId) == storyId)
                            .Delete("p")
                            .Delete("pst")
                            .ExecuteWithoutResultsAsync();
        
        
        return Ok();

    }







}
