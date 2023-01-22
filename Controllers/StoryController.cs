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


        return Ok("");

    }
    [Route("deleteStory/{storyId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteStory(string storyId)
    {

        await _client.Cypher.Match("(:User)-[viewRel:Viewed]->(s:Story)")
                          .Where((Story s) => s.Id.ToString() == storyId)
                          .Delete("viewRel")
                          .ExecuteWithoutResultsAsync();

        await _client.Cypher.Match("(usr:User)-[p:Published]->(str:Story)")
                            .Where((Story str) => str.Id.ToString() == storyId)
                            .Delete("p")
                            .Delete("str")
                            .ExecuteWithoutResultsAsync();
        
        
        return Ok();

    }

    [Route("getAllStoryFromUser/{userId}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> getAllPostsFromUser(string userId)
    {

        var storys = await _client.Cypher.Match("(u:User)-[r:Published]->(s:Story)")
                                  .Where((User u) => u.Id.ToString() == userId)
                                  .Return(s => s.As<Story>()).ResultsAsync;


        return Ok(storys);
    }
    [Route("viewStory/{userId}/{storyId}")]
    [HttpPost]
    public async Task<IActionResult> viewStory(string userId, string storyId)
    {
        await _client.Cypher.Match("(usr1:User)", "(s:Story)")
                              .Where((User usr1) => usr1.Id == userId)
                              .AndWhere((Story s) => s.Id.ToString() == storyId)
                              .Create("(usr1)-[v:Viewed]->(s)")
                              .ExecuteWithoutResultsAsync();
        
        return Ok();
    }





}
