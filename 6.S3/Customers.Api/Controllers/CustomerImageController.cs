using System.Net;
using Amazon.S3;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerImageController : ControllerBase
{
    private readonly ICustomerImageService _customerImageService;

    public CustomerImageController(ICustomerImageService customerImageService)
    {
        _customerImageService = customerImageService;
    }

    [HttpPost("customers/{id:guid}/image")]
    public async Task<IActionResult> Upload([FromRoute] Guid id,
        [FromForm(Name = "Data")] IFormFile file)
    {
        var ret = await _customerImageService.UploadImageAsync(id, file);
        if (ret.HttpStatusCode == HttpStatusCode.OK)
        {
            return Ok(ret);
        }

        return BadRequest();
    }

    [HttpGet("customers/{id:guid}/image")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        try
        {
            var ret = await _customerImageService.GetImageAsync(id);
            return File(ret.ResponseStream, ret.Headers.ContentType);
        }
        catch (AmazonS3Exception e)
        {
            return NotFound("Image with provided Id was not found");
        }
    }

    [HttpDelete("customers/{id:guid}/image")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var ret = await _customerImageService.DeleteImageAsync(id);
        return ret.HttpStatusCode switch
        {
            HttpStatusCode.NoContent => Ok(),
            HttpStatusCode.NotFound => NotFound(),
            _ => BadRequest()
        };
    }
}