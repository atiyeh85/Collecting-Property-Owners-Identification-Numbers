using Macfa.MagfaSms;
using Macfa.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

namespace Macfa.utility
{
    public  static class SmSSender
    {
         
       
        public static  long SendSmS(string Mobile,string Body,int? Message_ID)
        {

            var db = new StoreDb();
            Macfa.Models.Message M = db.Messages.Where(s => s.Id == Message_ID).FirstOrDefault();

            long nn =0;
            string username = "itqazvin";
            string password = "DGRrsmBjyCivvZox";
            string domain = "GHSMS";

            // Service (Add a Web Reference)
            MagfaSoapServer service = new MagfaSoapServer();

            // Basic Auth
            NetworkCredential netCredential = new NetworkCredential(username + "/" + domain, password);
            Uri uri = new Uri(service.Url);
            ICredentials credentials = netCredential.GetCredential(uri, "Basic");
            service.Credentials = credentials;

            // SOAP Compression For .NET FrameWork 2 or later
            service.EnableDecompression = true;
          
            // Call
            sendResult result = service.send(
                new string[] { Body },
                new string[] { "300071281" },
                new string[] {Mobile },
                new long?[] { 198981, 123032 },
                new int?[] { 0 },
                new string[] { "" },
                new int?[] { 0 }
            );

            if (result.status != 0)
            {
                Console.WriteLine("error: " + result.status);
            }
            else
            {
                if (result.messages!=null)
                {
                    foreach (sendMessage msg in result.messages)
                    {

                        if (msg.status == 0)
                        {
                            M.mid = msg.id;
                            M.parts = msg.parts;
                            M.userId = msg.userId;
                        }
                        long?[] mids = new long?[] { msg.id };
                        deliveryResult res = service.statuses(mids);
                        if (result.status != 0)
                        {
                            Console.WriteLine("error: " + result.status);
                        }
                        else
                        {
                            foreach (deliveryStatus s in res.dlrs)
                            {
                                M.dlr = s.status;
                                db.Entry(M).State = EntityState.Modified;

                                db.SaveChanges();
                            }
                        }

                    }
                }
               
            }
            return nn;
        }
    }
}