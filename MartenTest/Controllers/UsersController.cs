using Bogus;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace MartenTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IDocumentStore _documentStore;

    public UsersController(ILogger<UsersController> logger, IDocumentStore documentStore)
    {
        _logger = logger;
        _documentStore = documentStore;
    }

    [HttpGet("[action]")]
    public IReadOnlyList<User> Get(string name)
    {
        using var session = _documentStore.QuerySession();

        var result = session.PlainTextSearch<User>(name);

        return result;
    }
    
    [HttpGet("[action]")]
    public ActionResult Fill()
    {
        using var session = _documentStore.OpenSession();
        
        for (int i = 0; i < 10000; i++)
        {
            var faker = new Faker();
    
            session.Insert(new User()
            {
                Id = new Guid(),
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                Created = faker.Date.PastOffset()
            });
        }
        
        session.SaveChanges();

        return Ok();
    }
}