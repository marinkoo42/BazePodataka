using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4j;
using BazePodatakaProjekat.Models;

namespace BazePodatakaProjekat.Controllers
{
    public class CommentController : ControllerBase
    {
        private IGraphClient _client;

        public CommentController(IGraphClient client)
        {
            _client = client;
        }



        [Route("createComment/{userId}/{postId}")]
        [HttpPost]
        public async Task<IActionResult> CreateComment(String userId, String postId , [FromBody]Comment comment)
        {

            comment.Id = Guid.NewGuid();



            await _client.Cypher.Create("(d:Comment $com)")
                               .WithParam("com", comment)
                               .ExecuteWithoutResultsAsync();

            await _client.Cypher.Match("(usr:User)", "(p:Post)", "(c:Comment)")
                                .Where((User usr) => usr.Id == userId)
                                .AndWhere((Post p) => p.Id.ToString() == postId)
                                .AndWhere((Comment c) => c.Id == comment.Id)
                                .Create("(usr)-[:Made_comment]->(c)")
                                .Create("(c)-[:Comment_on_post]->(p)")
                                .ExecuteWithoutResultsAsync();


            return Ok();

        }


        [Route("deleteComment/{commentId}/{userId}/{postId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(string commentId , string userId , string postId) 
        {
            await _client.Cypher.Match("(kom:Comment)-[createComRel:Comment_on_post]->(p:Post)", "(u:User)-[madeComRel:Made_comment]->(kom)")
                           .Where((Post p) => p.Id.ToString() == postId)
                           .AndWhere((User u) => u.Id == userId)
                           .AndWhere((Comment kom) => kom.Id.ToString() == commentId )
                           .Delete("createComRel")
                           .Delete("madeComRel")
                           .Delete("kom")
                           .ExecuteWithoutResultsAsync();

     
                          
            return Ok();

        }


        [Route("editComment/{commentId}")] //samo ako je user kreirao comment 
        [HttpPut]
        public async Task<IActionResult> EditComment(string commentId, [FromBody] Comment comment)
        {
            await _client.Cypher.Match("(c:Comment)")
                                        .Where((Comment c) => c.Id.ToString() == commentId)
                                        .Set("c=$comment")
                                        .Set("c.Id=$oldId")
                                        .WithParam("comment", comment)
                                        .WithParam("oldId", commentId)
                                        .ExecuteWithoutResultsAsync();
            return Ok();
        }

        [Route("getPostComments/{postId}")] 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetPostComments(string postId)
        {
            var komentari = await _client.Cypher.Match("(c:Comment)-[:Comment_on_post]->(p:Post)")
                                        .Where((Post p) => p.Id.ToString() == postId)
                                        .Return(c => c.As<Comment>()).ResultsAsync;
                                        
            return Ok(komentari);
        }

    }
}
