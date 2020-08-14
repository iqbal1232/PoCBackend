using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;

namespace WebApplication7.Controllers
{
    public class DownloadsController : ApiController
    {
        // GET: Downloads
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Index(string description)
        {
            string path = HostingEnvironment.MapPath("~/FolderData.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            string xQryStr = "//Folder[Description='" + description + "']";
            XmlNode listOfNodes = doc.SelectSingleNode(xQryStr);
            HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
            var file = listOfNodes.LastChild.InnerText;
            string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=tmbfile;AccountKey=V0x0M8c9WKHd8SzjXygtLCAqR9XuEmY86/nQFo260Ymr11rsaI5iNDma/Ap5Mw/KP+V6wZpXb6lh9tvar6F0Zw==;EndpointSuffix=core.windows.net";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var pathn = System.Web.Hosting.HostingEnvironment.MapPath("~");
            CloudBlobContainer container = blobClient.GetContainerReference("tmbfile");
            var blob = container.GetBlockBlobReference(file);
            MemoryStream memStream = new MemoryStream();
            blob.DownloadToStream(memStream);
            
            response.Content = new StreamContent(memStream);
            var headers = response.Content.Headers;
            headers.ContentDisposition = new ContentDispositionHeaderValue("attachement");
            headers.ContentDisposition.FileName = file;
            headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            headers.ContentLength = blob.Properties.Length;

            return response;

        }
    }
}