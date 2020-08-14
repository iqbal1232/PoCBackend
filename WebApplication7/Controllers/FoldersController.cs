using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;

namespace demo.Controllers
{
    public class FoldersController : ApiController
    {
        public HttpResponseMessage GetAllFolders()
        {
            string path = HostingEnvironment.MapPath("~/FolderData.xml");
            
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

            HttpResponseMessage response = this.Request.
                    CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent
        (doc.OuterXml, Encoding.UTF8, "application/xml");
            return response;

        }

        public HttpResponseMessage GetFolderDetails(string description)
        {
            string path = HostingEnvironment.MapPath("~/FolderData.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            
            string xQryStr = "//Folder[Description='" + description + "']";
            XmlNode listOfNodes = doc.SelectSingleNode(xQryStr);
            var file= listOfNodes.LastChild.InnerText;

            HttpResponseMessage response = this.Request.
                    CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent
        (listOfNodes.OuterXml, Encoding.UTF8, "application/xml");
            return response;
        }

        public HttpResponseMessage AddFolder()
        {
            var request = HttpContext.Current.Request;
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~");
            var name = "";
            if (request != null && request.Files.Count > 0)
            {
                foreach (string file in request.Files)
                {
                    name = request.Files[file].FileName;
                    string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=tmbfile;AccountKey=V0x0M8c9WKHd8SzjXygtLCAqR9XuEmY86/nQFo260Ymr11rsaI5iNDma/Ap5Mw/KP+V6wZpXb6lh9tvar6F0Zw==;EndpointSuffix=core.windows.net";
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);
                    
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                    CloudBlobContainer Container = blobClient.GetContainerReference("tmbfile");
                    Container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                    var fileBlob = Container.GetBlockBlobReference(request.Params["Description"] + "/"+request.Files[file].FileName);
                    
                    fileBlob.UploadFromStream(request.Files[file].InputStream);
                    
                }
            }

            string path1 = HostingEnvironment.MapPath("~/FolderData.xml");
            var doc = XDocument.Load(path1);
            var Folder = new XElement("Folder");
            var des = request.Params["Description"];
            var Description = new XElement("Description", request.Params["Description"]);
            var File = new XElement("File", name);
            Folder.Add(Description, File);
            doc.Root.Add(Folder);
            doc.Save(path1);
            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPut]
        public HttpResponseMessage UpdateFolder(string description)
        {
            var request = HttpContext.Current.Request;
            string path = HostingEnvironment.MapPath("~/FolderData.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            //Display all the book titles.
            XmlNodeList elemList = doc.GetElementsByTagName("Description");
            for (int i = 0; i < elemList.Count; i++)
            {
                if (elemList[i].InnerText == description)
                {
                    var element = elemList[i].ParentNode;
                    element.FirstChild.InnerXml = HttpContext.Current.Request.Params["Description"];
                    if (request != null && request.Files.Count > 0) {
                        foreach (string file in request.Files)
                        {
                            try
                            {
                                var postedfile = request.Files[file];
                                var name = Path.GetFileName(postedfile.FileName);
                                string storageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=tmbfile;AccountKey=V0x0M8c9WKHd8SzjXygtLCAqR9XuEmY86/nQFo260Ymr11rsaI5iNDma/Ap5Mw/KP+V6wZpXb6lh9tvar6F0Zw==;EndpointSuffix=core.windows.net";
                                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountConnectionString);

                                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                                CloudBlobContainer Container = blobClient.GetContainerReference("tmbfile");
                                Container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                                var fileBlob = Container.GetBlockBlobReference(request.Params["Description"] + "/" + request.Files[file].FileName);

                                fileBlob.UploadFromStream(request.Files[file].InputStream);
                                element.LastChild.InnerXml = name;
                                //doc.Save(path);
                            }
                            catch (Exception e)
                            {
                                return Request.CreateResponse(HttpStatusCode.Created, e.Message + Environment.NewLine + e.StackTrace);
                            }
                        }
                    }
                    doc.Save(path);

                }
            }
            return Request.CreateResponse(HttpStatusCode.Created);
        }
    }
}