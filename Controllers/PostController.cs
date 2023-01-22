
namespace BazePodatakaProjekat.Controllers;
using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;



[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly IGraphClient _client;
    private UserController _userController;

    public PostController(IGraphClient client, UserController userController)
    {
        _client = client;
        _userController = userController;
    }

    [Route("getAllPostsFromUser/{userId}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> getAllPostsFromUser(string userId)
    {

        var posts = await _client.Cypher.Match("(u:User)-[r:Created]->(p:Post)")
                                  .Where((User u) => u.Id.ToString() == userId)
                                  .Return(p => p.As<Post>()).ResultsAsync;


        return Ok(posts);
    }

    [Route("createPost/{userId}")]
    [HttpPost]
    public async Task<IActionResult> CreatePost(String userId, [FromBody] Post post)
    {
        post.Id = Guid.NewGuid();
        post.DateTimeCreated= DateTime.Now;
        post.CreatorId = userId;

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
    
    
    
    [Route("deletePost/{postId}")] //samo ako je user kreirao
    [HttpDelete]
    public async Task<IActionResult> DeletePost(string postId)
    {

        await _client.Cypher.Match("(kom:Comment)-[createComRel:Comment_on_post]->(p:Post)", "(:User)-[madeComRel:Made_comment]->(kom)")
                            .Where((Post p) => p.Id.ToString() == postId)
                            .Delete("madeComRel")
                            .Delete("createComRel")
                            .Delete("kom")
                            .ExecuteWithoutResultsAsync();

                            


        await _client.Cypher.Match("(:User)-[likeRel:Liked]->(p:Post)")
                          .Where((Post p) => p.Id.ToString() == postId)
                          .Delete("likeRel")
                          .ExecuteWithoutResultsAsync();
                        


        await _client.Cypher.Match("(:User)-[createPostRel:Created]->(p:Post)")
                          .Where((Post p) => p.Id.ToString() == postId)
                          .Delete("createPostRel")
                          .Delete("p")
                          .ExecuteWithoutResultsAsync();





        return Ok();
    }
    


    [Route("updatePost/{postId}")] //samo ako je user kreirao post 
    [HttpPut]
    public async Task<IActionResult> UpdatePost(string postId, [FromBody] Post post)
    {

        await _client.Cypher.Match("(p:Post)")
                                    .Where((Post p) => p.Id.ToString() == postId)
                                    .Set("p=$post")
                                    .Set("p.Id=$oldId")
                                    .WithParam("post",post)
                                    .WithParam("oldId",postId)
                                    .ExecuteWithoutResultsAsync();
        return Ok();
    }

    [Route("likePost/{userId}/{postId}")] //ima like i unlike
    [HttpPut]
    public async Task<IActionResult> LikePost(string userId, string postId)
    {


        //cekiraj da li je lajkovao

        var liked = await _client.Cypher.Match("(usr1:User)", "(p:Post)")
                           .Where((User usr1) => usr1.Id == userId)
                           .AndWhere((Post p) => p.Id.ToString() == postId)
                           .With("usr1 , exists((usr1) -[:Liked]->(p)) as ret")
                           //.Call("return exists((n) -[:Liked]->(:Post))")
                           .Return(ret => ret.As<bool>()).ResultsAsync;


        if (liked.Single()) //odlajkuj
        {
            await _client.Cypher.Match("(u:User)-[likeRel:Liked]->(p:Post)")
                          .Where((Post p) => p.Id.ToString() == postId)
                          .AndWhere((User u) => u.Id == userId)
                          .Delete("likeRel")
                          .Set("p.NumberOfLikes = p.NumberOfLikes-1")
                          .ExecuteWithoutResultsAsync();

        }
        else // lajkuj
        {
            await _client.Cypher.Match("(usr1:User)", "(p:Post)")
                          .Where((User usr1) => usr1.Id == userId)
                          .AndWhere((Post p) => p.Id.ToString() == postId)
                          .Create("(usr1)-[r:Liked]->(p)")
                          .Set("p.NumberOfLikes = p.NumberOfLikes+1")
                          .ExecuteWithoutResultsAsync();

        }
        /* await _client.Cypher.Match("(usr1:User)", "(p:Post)")
                             .Where((User usr1) => usr1.Id == userId)
                             .AndWhere((Post p) => p.Id.ToString() == postId)
                             .Create("(usr1)-[r:Liked]->(p)")
                             .Set("p.NumberOfLikes = p.NumberOfLikes+1")
                             .ExecuteWithoutResultsAsync();*/
        return Ok();

    }

    [Route("getPostLikes/{postId}")]
    [HttpGet]
    public async Task<IActionResult> GetPostLikes(string postId)
    {
        var likes = await _client.Cypher.Match("(u:User)-[l:Liked]->(p:Post)")
                                              .Where((Post p) => p.Id.ToString() == postId)
                                              .Return(u => new {
                                                  Id = u.As<User>().Id,
                                                  UserName = u.As<User>().UserName
                                              }).ResultsAsync;

        return Ok(likes);
    }

    [Route("getLikesCount/{postId}")]
    [HttpGet]
    public async Task<IActionResult> GetLikesCount(string postId)
    {
        var likes = await _client.Cypher.Match("(d:User)-[r:Liked]->(p:Post)")
                                              .Where((Post p) => p.Id.ToString() == postId)
                                              .Return(d => d.As<User>()).ResultsAsync;

        return Ok(likes.Count());
    }



    //vraca random rasporedjene postove followera koji su kreirani najkasnije pre 3 dana 
    [Route("getFollowingsRecentPosts/{myId}")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> getFollowingsRecentPosts(string myId)
    {

        var followings = await _client.Cypher.Match("(d:User)-[Following]->(f:User)")
                                              .Where((User d) => d.Id == myId)
                                              .Return(f => new {
                                                  Id = f.As<User>().Id,
                                              }).ResultsAsync;

        IEnumerable<Post> recent = Enumerable.Empty<Post>();
        var date = DateTime.Now;
        foreach (var following in followings)
        {
            var posts = await _client.Cypher.Match("(u:User)-[r:Created]->(p:Post)")
                                     .Where((User u) => u.Id.ToString() == following.Id)
                                     .AndWhere("datetime(p.DateTimeCreated) > datetime.realtime() - duration({days:3})")
                                     .Return(p => p.As<Post>())
                                     .Limit(2)
                                     .ResultsAsync;

            recent = recent.Concat(posts);
        }
        var r = new Random();    
        recent = recent.OrderBy(x => r.Next());


        return Ok(recent);

    }




}

