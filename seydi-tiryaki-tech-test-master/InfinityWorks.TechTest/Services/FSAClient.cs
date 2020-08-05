using InfinityWorks.TechTest.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace InfinityWorks.TechTest.Services
{
				public class FsaClient : IFsaClient
				{
								private readonly IHttpClientFactory _httpClientFactory;

								public FsaClient(IHttpClientFactory httpClientFactory)
								{
												_httpClientFactory = httpClientFactory;
								}

								public async Task<FsaAuthorityList> GetAuthorities()
								{
												return await GetFsaResource<FsaAuthorityList>("Authorities");
								}

								public async Task<List<AuthorityRatingItem>> GetRating(string authorityId)
								{
												return await GetFsaRating<List<AuthorityRatingItem>>($"OpenDataFiles/FHRS{authorityId}en-GB.xml");
								}

								private async Task<T> GetFsaResource<T>(string path)
								{
												var serializer = new JsonSerializer();
												var client = _httpClientFactory.CreateClient();
												client.DefaultRequestHeaders.Add("x-api-version", "2");

												var stream = await client.GetStreamAsync($"https://api.ratings.food.gov.uk/{path}");

												using (var sr = new StreamReader(stream))
												using (var jsonTextReader = new JsonTextReader(sr))
												{
																return serializer.Deserialize<T>(jsonTextReader);
												}
								}

								private async Task<List<AuthorityRatingItem>> GetFsaRating<T>(string path)
								{
												var serializer = new JsonSerializer();
												var client = _httpClientFactory.CreateClient();
												client.DefaultRequestHeaders.Add("x-api-version", "2");

												var xmlString = await client.GetStringAsync($"https://ratings.food.gov.uk/{path}");
												xmlString = xmlString.Trim();

												var ratings = new List<AuthorityRatingItem>();

												if (xmlString.StartsWith("<?xml") && xmlString.EndsWith(">"))
												{
																XmlDocument xmldoc = new XmlDocument();

																try
																{
																				xmldoc.LoadXml(xmlString);
																}
																catch (System.Exception ex)
																{
																				// handle
																}

																XmlNodeList listOfRatings5 = xmldoc.SelectNodes("//*[RatingValue = '5' or RatingValue = 'Pass']");
																XmlNodeList listOfRatings4 = xmldoc.SelectNodes("//*[RatingValue = '4' or RatingValue = 'Pass and Eat Safe']");
																XmlNodeList listOfRatings3 = xmldoc.SelectNodes("//*[RatingValue = '3']");
																XmlNodeList listOfRatings2 = xmldoc.SelectNodes("//*[RatingValue = '2']");
																XmlNodeList listOfRatings1 = xmldoc.SelectNodes("//*[RatingValue = '1' or RatingValue = 'Improvement Required']");
																XmlNodeList listOfRatingsE = xmldoc.SelectNodes("//*[RatingValue = 'Exempt']");
																XmlNodeList listOfRatingsA = xmldoc.SelectNodes("//*[RatingValue = 'AwaitingInspection' or RatingValue = 'Awaiting Inspection']");

																int total = listOfRatings5.Count + listOfRatings4.Count + listOfRatings3.Count + listOfRatings2.Count + listOfRatings1.Count + listOfRatingsE.Count + listOfRatingsA.Count;

																double p = (double)100 / total;

																ratings.Add(new AuthorityRatingItem { Name = "5-star", Value = (double)(listOfRatings5.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "4-star", Value = (double)(listOfRatings4.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "3-star", Value = (double)(listOfRatings3.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "2-star", Value = (double)(listOfRatings2.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "1-star", Value = (double)(listOfRatings1.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "Exempt", Value = (double)(listOfRatingsE.Count * p) });
																ratings.Add(new AuthorityRatingItem { Name = "Awaiting Inspection", Value = (double)(listOfRatingsA.Count * p) });
												}
												else
												{
																ratings.Add(new AuthorityRatingItem { Name = "5-star n/a", Value = 0 });
																ratings.Add(new AuthorityRatingItem { Name = "4-star n/a", Value = 0 });
																ratings.Add(new AuthorityRatingItem { Name = "3-star n/a", Value = 0 });
																ratings.Add(new AuthorityRatingItem { Name = "2-star n/a", Value = 0 });
																ratings.Add(new AuthorityRatingItem { Name = "1-star n/a", Value = 0 });
																ratings.Add(new AuthorityRatingItem { Name = "Exempt n/a", Value = 0 });
												}

												return ratings;
								}

				}
}
