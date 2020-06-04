using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PaymentHandler.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        [Route("notify_url")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> NotifyHandler([FromForm] IFormCollection value)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"============ {DateTime.Now:HH:mm:ss} ============");
            sb.AppendLine(HttpContext.Connection.RemoteIpAddress.ToString());

            sb.AppendLine($"Method: {Request.Method} using {Request.Scheme}");
            
            sb.AppendLine("******* Queries:");
            foreach (var q in Request.Query)
            {
                sb.AppendLine("***");
                sb.AppendLine(q.Key);
                sb.AppendLine("=>");
                foreach (var v in q.Value)
                {
                    sb.AppendLine(v);
                }
            }

            sb.AppendLine();
            
            sb.AppendLine("******* Headers:");
            foreach (var h in Request.Headers)
            {
                sb.AppendLine("***");
                sb.AppendLine(h.Key);
                sb.AppendLine("=>");
                foreach (var v in h.Value)
                {
                    sb.AppendLine(v);
                }
            }

            sb.AppendLine();
            
            var req = HttpContext.Request;

            // Allows using several time the stream in ASP.Net Core

            sb.AppendLine("******* Body:");

            try
            {
                using var reader 
                    = new StreamReader(req.Body, Encoding.UTF8, true, 1024, true);
                sb.AppendLine(await reader.ReadToEndAsync());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                sb.AppendLine($"<read failure: {e.Message}>");
            }

            sb.AppendLine("******* RAW Body:");

            foreach (var vl in value)
            {
                sb.AppendLine("***");
                sb.AppendLine(vl.Key);
                sb.AppendLine("=>");
                foreach (var v in vl.Value)
                {
                    sb.AppendLine(v);
                }
            }

            Console.WriteLine(sb.ToString());
            
            // Rewind, so the core is not lost when it looks the body for the request
            return Ok();
        }
    }
}