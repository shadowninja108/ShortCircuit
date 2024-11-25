using Microsoft.AspNetCore.Http.Timeouts;
using ShortCircuit.Server.Model;
using FirstStrike.Thunder.Save;
using System.IO.Pipelines;
using System.Buffers;

namespace ShortCircuit.Server.Controllers
{
    public static class SaveApiController
    {
        private const string Path = "api/save";

        [EndpointName("Save decode")]
        [EndpointDescription("Decodes provided binary save.")]
        [RequestTimeout(milliseconds: 5000)]
        public static async Task Decode(HttpContext context)
        {
            /* Do an upfront check for the size and immediately reject. */
            if(context.Request.ContentLength > Save.MaxSize)
            {
                await ReturnError(context, StatusCodes.Status413PayloadTooLarge, ResponseType.SaveTooBig);
                return;
            }

            var reader = context.Request.BodyReader;
            ReadResult read;
            while (true)
            {
                read = await reader.ReadAsync();
                if (read.IsCompleted)
                    break;

                /* If the user cancels, we're done receiving data. */
                if(read.IsCanceled)
                {
                    await ReturnError(context, StatusCodes.Status499ClientClosedRequest, ResponseType.Cancelled);
                    return;
                }

                /* If the user has sent too much data, return error. */
                if(read.Buffer.Length > Save.MaxSize)
                {
                    await ReturnError(context, StatusCodes.Status413PayloadTooLarge, ResponseType.SaveTooBig);
                    return;
                }

                reader.AdvanceTo(read.Buffer.Start, read.Buffer.End);
            }

            try
            {
                var save = await Save.Decrypt(new MemoryStream(read.Buffer.ToArray()));

                await ReturnSuccess(context, save);
                return;
            } 
            catch(BadSaveException e)
            {
                await ReturnError(context, StatusCodes.Status400BadRequest, ResponseType.SaveBad, e.Message);
                return;
            } 
            catch(Exception e)
            {
                /* TODO: log... */
                await ReturnError(context, StatusCodes.Status500InternalServerError, ResponseType.UnknownFailure);
                return;
            }
        }

        public static async Task Dictionary(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            /* TODO: consolidate this... */
            await new FileInfo("main.strings").OpenRead().CopyToAsync(context.Response.Body);
        }

        private static async Task ReturnSuccess<T>(HttpContext context, T value)
        {
            context.Response.ContentType = "application/json";
            await JsonSerializationContext.Serialize(
                context.Response.Body,
                new SuccessResponse<T>(value)
            );
        }

        private static async Task ReturnError(HttpContext context, int code, ResponseType type, string? message = null)
        {
            context.Response.StatusCode = code;
            context.Response.ContentType = "application/json";
            await JsonSerializationContext.Serialize(
                context.Response.Body,
                new FailedResponse(type, message)
            );
        }

        public static void MapSaveApi(this WebApplication app)
        {
            var api = app.MapGroup(Path);
            api.MapPost("decode", Decode);
            api.MapGet("dictionary", Dictionary);
        }
    }
}