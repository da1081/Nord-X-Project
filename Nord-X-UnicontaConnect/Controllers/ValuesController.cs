using Nord_X_UnicontaConnect.CustomExtensions;
using Nord_X_UnicontaConnect.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Uniconta.API.Service;
using Uniconta.API.System;
using Uniconta.ClientTools.DataModel;
using Uniconta.Common;
using Uniconta.DataModel;

namespace Nord_X_UnicontaConnect.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        // E.g. /api/values?username=XXXXXXX&password=XXXXXXX&compId=52075&dayRange=300
        public async Task<IEnumerable<PdfModel>> GetAsync(string username, string password, string portalGuid = null, string compName = null, int compId = 0, int dayRange = 1)
        {
            // Set portalGuid.
            if (portalGuid == null)
                portalGuid = "";

            // Login against Uniconta.
            UnicontaConnection connection = new UnicontaConnection(APITarget.Live);
            Session session = new Session(connection);
            ErrorCodes errorCodes = await session.LoginAsync(username, password, Uniconta.Common.User.LoginType.API, new Guid(portalGuid), Language.en);
            if (errorCodes != ErrorCodes.Succes)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            // Select company.
            Company company = null;
            if (compName != null)
                company = await session.GetCompany(Name: compName);
            if (company == null && compId != 0)
                company = await session.GetCompany(CompanyId: compId);
            if (company == null)
                company = session.DefaultCompany;

            // Create CrudAPI.
            CrudAPI crudAPI = new CrudAPI(session, company);

            // Create filter.
            DateTime now = DateTime.Now;
            DateTime then = now.AddDays(dayRange * -1);
            List<PropValuePair> whereFilter = new List<PropValuePair> {
                PropValuePair.GenereteWhereElements("Created", typeof(DateTime),
                    $"{then.Day}/{then.Month}-{then.Year}..{now.Day}/{now.Month}-{now.Year}"),
                PropValuePair.GenereteWhereElements("Fileextension", typeof(string), "PDF"),
            };

            // Fetch VouchersClient (aka. Faktura)
            VouchersClient[] result = await crudAPI.Query<VouchersClient>(whereFilter);

            // Read VouchersClient attachments.
            errorCodes = await crudAPI.Read(result);
            if (errorCodes != ErrorCodes.Succes)
                throw new HttpResponseException(HttpStatusCode.InternalServerError);

            // Extract Text from PDF.
            IEnumerable<PdfModel> pdfModels = NordlysExtractVersion01(result);
            // IEnumerable<PdfModel> pdfModels = NordlysExtractVersion02(result);


            // Logout
            if (session.LoggedIn)
                await session.LogOut();

            // return result
            return pdfModels;
        }

        private IEnumerable<PdfModel> NordlysExtractVersion01(VouchersClient[] result)
        {
            List<PdfModel> pdfModels = new List<PdfModel>();
            foreach (VouchersClient vc in result)
            {
                try
                {
                    List<string> res = new List<string>();
                    var stream = PdfReader.Open(new MemoryStream(vc.Buffer), PdfDocumentOpenMode.ReadOnly);
                    for (int i = 0; i < stream.Pages.Count; i++)
                    {
                        PdfPage page = stream.Pages[i];
                        res.AddRange(PdfSharpExtensions.ExtractText(ContentReader.ReadContent(page)));
                    }

                    if (res.Contains("Norlys Energi A/S"))
                    {
                        string watt = string.Empty;
                        for (int i = 0; i < res.Count; i++)
                        {
                            if (res[i] == "kWh")
                            {
                                watt = res[i - 1];
                                continue;
                            }
                        }
                        pdfModels.Add(new PdfModel
                        {
                            Time = vc.Created,
                            Measure = watt,
                        });
                    }
                }
                catch (PdfSharp.Pdf.IO.PdfReaderException)
                {
                    continue; // Ignore Invalid PDF files.
                }
            }
            return pdfModels;
        }

        private IEnumerable<PdfModel> NordlysExtractVersion02(VouchersClient[] result)
        {
            List<PdfModel> pdfModels = new List<PdfModel>();
            foreach (VouchersClient vc in result)
            {
                try
                {
                    List<string> res = new List<string>();
                    var stream = PdfReader.Open(new MemoryStream(vc.Buffer), PdfDocumentOpenMode.ReadOnly);
                    for (int i = 0; i < stream.Pages.Count; i++)
                    {
                        PdfPage page = stream.Pages[i];
                        res.AddRange(PdfSharpExtensions.ExtractText(ContentReader.ReadContent(page)));
                    }

                    if (res.Contains("Norlys Energi A/S"))
                    {
                        string watt = string.Empty;
                        for (int i = 0; i < res.Count; i++)
                        {
                            if (res[i] == "kWh")
                            {
                                watt = res[i - 1];
                                continue;
                            }
                        }
                        pdfModels.Add(new PdfModel
                        {
                            Time = vc.Created,
                            Measure = watt,
                        });
                    }
                }
                catch (PdfSharp.Pdf.IO.PdfReaderException)
                {
                    continue; // Ignore Invalid PDF files.
                }
            }
            return pdfModels;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
