/*using Neo4jClient.DataAnnotations.Serialization;
using Neo4jClient.DataAnnotations;
using Neo4jClient;
using Neo4j.AspNetCore.Identity;
namespace BazePodatakaProjekat.Models
{


        public class ApplicationContext : AnnotationsContext
        {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public ApplicationContext(IGraphClient graphClient) : base(graphClient)
            {
            }

            public ApplicationContext(IGraphClient graphClient, EntityService entityService) : base(graphClient, entityService)
            {
            }

            public ApplicationContext(IGraphClient graphClient, EntityResolver resolver) : base(graphClient, resolver)
            {
            }

            public ApplicationContext(IGraphClient graphClient, EntityConverter converter) : base(graphClient, converter)
            {
            }

            public ApplicationContext(IGraphClient graphClient, EntityResolver resolver, EntityService entityService) : base(graphClient, resolver, entityService)
            {
            }

            public ApplicationContext(IGraphClient graphClient, EntityConverter converter, EntityService entityService) : base(graphClient, converter, entityService)
            {
            }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public EntitySet<AppUser> ApplicationUsers { get; set; }
            public EntitySet<IdentityRole> ApplicationRoles { get; set; }
        }
}


*/