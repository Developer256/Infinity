using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfinityWorks.TechTest.Model;
using InfinityWorks.TechTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace InfinityWorks.TechTest.Controllers
{
				[Route("api/")]
				[ApiController]
				public class RatingController : Controller
				{
								private readonly IFsaClient _fSaClient;

								public RatingController(IFsaClient fSaClient)
								{
												_fSaClient = fSaClient;
								}

								/// <summary>
								/// Produces a list of authorities, for the select dropdown
								/// </summary>
								/// <returns>
								/// List of authorities
								/// </returns>
								[HttpGet]
								public async Task<JsonResult> GetAsync()
								{
												var fsaAuthorities = await _fSaClient.GetAuthorities();

												var authorityList = fsaAuthorities.Authorities.Select(authority => new Authority { Id = authority.LocalAuthorityIdCode, Name = authority.Name });

												return Json(authorityList);
								}

								/// <summary>
								/// Produces the ratings for a specific authority for the table
								/// </summary>
								/// <param name="authorityId">The authority to calculate ratings for</param>
								/// <returns>
								/// The ratings to display
								/// </returns>
								[HttpGet("{authorityId}")]
								public async Task<JsonResult> Get(string authorityId)
								{
												var ratings = await _fSaClient.GetRating(authorityId);

												return Json(ratings);
								}
				}
}