using BazePodatakaProjekat.Models;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;

namespace BazePodatakaProjekat.Controllers
{
    public class SuggestionController : Controller
    {
        private readonly IGraphClient _client;

        public SuggestionController(IGraphClient client)
        {
            _client = client;
        }

        [Route("getMostPopularUser")]
        [HttpGet]
        public async Task<IActionResult> getMostPopularUser()
        {


            var str = await _client.Cypher.Match("(n:User)")
                            .With("n , n.NumbersOfFollowers as num")
                            .OrderByDescending("num")
                            .Limit(2)
                            .Return(n => n.As<User>()).ResultsAsync;
            
            return Ok(str);
        }
        [Route("getMostLikedPosts")]
        [HttpGet]
        public async Task<IActionResult> getMostLikedPosts()
        {


            var str = await _client.Cypher.Match("(p:Post)")
                            .With("p , p.NumberOfLikes as num")
                            .OrderByDescending("num")
                            .Limit(2)
                            .Return(p => p.As<Post>()).ResultsAsync;

            return Ok(str);
        }


    }
}
