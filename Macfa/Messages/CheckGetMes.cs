using Quartz;
using Macfa.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Macfa.MagfaSms;
using System.Text.RegularExpressions;

namespace Macfa.Messages
{
    public class CheckGetMes : IJob
    {
        private StoreDb db = new StoreDb();
        public Task Execute(IJobExecutionContext context)
        {
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
            int count = 100;
            string shortNumber = "";
            messagesResult result = service.messages(count, "300071281");
            if (result.status != 0)
            {
                Console.WriteLine("error: " + result.status);
            }
            else
            {
                Macfa.Models.Message M = new Macfa.Models.Message();
                var BillID = "";
                if (result.messages != null)
                {
                    foreach (var item in result.messages)
                    {
                        var ConvertBody = utility.PertionDate.ConvertNumbersToEnglish(item.body);
                        string[] numbers = Regex.Split(ConvertBody, @"\D+");
                        foreach (var Num in numbers)
                        {

                            if (Num.Length == 0)
                            {
                                continue;
                            }
                            else if (Num.Length == 13)
                            {
                                BillID = Num;
                            }
                            else if (Num.Length > 13 || Num.Length < 13)
                            {
                                M.Date = utility.PertionDate.Today();
                                M.Time = DateTime.Now.ToShortTimeString();
                                M.Text = ConvertBody;
                                M.BillID = BillID;
                                M.Mobile = item.senderNumber;
                                M.IsSuccessFull = false;

                                var Body = "شهروند گرامی" + "\n" + "شناسه قبض وارد شده صحیح نمی باشد.لطفا دوباره تلاش کنید ." + "\n" + "با تشکر  شهرداری قزوین";
                                db.Messages.Add(M);
                                db.SaveChanges();
                                var Id = utility.SmSSender.SendSmS(M.Mobile, Body, M.Id);
                            }
                        }
                        if (BillID != "" )
                        {
                            var BillInfo = db.Informations.Where(s => s.BillID == BillID).FirstOrDefault();

                            if (BillInfo != null)
                            {
                                if (item.senderNumber == BillInfo.Mobile)
                                {

                                    if (BillInfo.BillID == BillID)
                                    {
                                        //چک تکراری بودن -اگر تکراریه فقط در جدول گزارش ثبت شود و یک پیام به کاربر
                                        M.Date = utility.PertionDate.Today();
                                        M.Time = DateTime.Now.ToShortTimeString();

                                        M.Text = ConvertBody;
                                        M.BillID = BillID;
                                        M.Info_ID = BillInfo.Id;
                                        M.IsSuccessFull = false;
                                        M.Mobile = item.senderNumber;
                                        var Body = "شهروند گرامی" + "\n" + "اطلاعات شما قبلا در  سامانه ثبت شده است." + "\n" + "با تشکر    شهرداری قزوین";
                                        db.Messages.Add(M);
                                        db.SaveChanges();
                                        var Id = utility.SmSSender.SendSmS(M.Mobile, Body, M.Id);
                                    }
                                    else if (BillInfo.BillID != BillID)
                                    {
                                        //ثبت دوباره شناسه قبض و ثبت در دو جدول و ارسال پیام مناسب به کاربر
                                        BillInfo.Mobile = item.senderNumber;
                                        BillInfo.Date = utility.PertionDate.Today();
                                        M.Time = DateTime.Now.ToShortTimeString();
                                        BillInfo.GetBillID = BillID;
                                        M.IsSuccessFull = true;
                                        M.Date = utility.PertionDate.Today();
                                        M.Time = DateTime.Now.ToShortTimeString();
                                        M.Text = ConvertBody;
                                        M.Info_ID = BillInfo.Id;
                                        M.BillID = BillID;
                                        M.Mobile = item.senderNumber;
                                        var body = "شهروند گرامی" + "\n" + "اطلاعات شما در سامانه ثبت شد." + "\n" + "با تشکر    شهرداری قزوین";
                                        db.Messages.Add(M);
                                        db.Entry(BillInfo).State = EntityState.Modified;
                                        db.SaveChanges();
                                        utility.SmSSender.SendSmS(M.Mobile, body, M.Id);
                                    }
                                }
                                else if (item.senderNumber != BillInfo.Mobile)
                                {
                                    //if (BillInfo.BillID == BillID)
                                    //{
                                    //ثبت و جایگزین شده  موبایل جدید  و شناسه قبض به عنوان مالک جدید و ارسال پیام مناسب به کاربر
                                    BillInfo.Mobile = item.senderNumber;
                                    BillInfo.GetBillID = BillID;
                                    BillInfo.Date = utility.PertionDate.Today();
                                    M.Time = DateTime.Now.ToShortTimeString();
                                    M.IsSuccessFull = true;
                                    M.Info_ID = BillInfo.Id;
                                    M.Date = utility.PertionDate.Today();
                                    M.Time = DateTime.Now.ToShortTimeString();
                                    M.Text = ConvertBody;
                                    M.BillID = BillID;
                                    M.Mobile = item.senderNumber;
                                    var body = "شهروند گرامی" + "\n" + "اطلاعات شما در سامانه ثبت شد." + "\n" + "با تشکر   شهرداری قزوین";
                                    db.Messages.Add(M);
                                    db.Entry(BillInfo).State = EntityState.Modified;
                                    db.SaveChanges();
                                    utility.SmSSender.SendSmS(M.Mobile, body, M.Id);
                                    
                                }
                            }
                            else
                            {
                                M.Date = utility.PertionDate.Today();
                                M.Time = DateTime.Now.ToShortTimeString();
                                M.Text = ConvertBody;
                                M.BillID = BillID;
                                M.IsSuccessFull = false;
                                M.Mobile = item.senderNumber;
                                var body = "شهروند گرامی" + "\n" + "شماره قبض وارد شده در سامانه یافت نشد .لطفا دوباره تلاش کنید ." + "\n" + "با تشکر    شهرداری قزوین";
                                utility.SmSSender.SendSmS(M.Mobile, body, M.Id);
                                db.Messages.Add(M);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            //TempData["Message"] = "شهروند گرامی" + "\n" + "شماره قبض فیش به درستی وارد نشده است .لطفا دوباره تلاش کنید." + "\n" + "با تشکر سازمان فناوری اطلاعات شهرداری قزوین";
                        }
                    }
                }


            }

            return Task.CompletedTask;
        }
    }
}
