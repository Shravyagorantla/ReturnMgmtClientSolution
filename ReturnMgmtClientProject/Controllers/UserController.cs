using ComponentProcessingAPIProject.Test.view_model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReturnMgmtClientProject.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ReturnMgmtClientProject.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProcessRequest()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProcessRequest(ProcessRequest processRequest)
        {
            ProcessRequest pRequest = new ProcessRequest();
            ProcessResponse pResponse = new ProcessResponse();
            using (var httpClient = new HttpClient())
            {
                //int id = verify.VerificationId;
                StringContent content = new StringContent(JsonConvert.SerializeObject(processRequest), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("http://localhost:54592/api/Process/ProcessRequest", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //ViewBag.Result = "Success";
                    pResponse = JsonConvert.DeserializeObject<ProcessResponse>(apiResponse);
                    //ViewBag.Result = "Successfully Registered, Please Login.....THANKYOU";
                    TempData["pResponse"] = JsonConvert.SerializeObject(pResponse);
                    
                }
                TempData["RequestId"] = pResponse.RequestId;
                TempData["PandD"] = JsonConvert.SerializeObject(pResponse.PackagingAndDeliveryCharge);
                //TempData["PandD"] = pResponse.PackagingAndDeliveryCharge;
                TempData["Processing"] = JsonConvert.SerializeObject(pResponse.ProcessingCharge);
            }
            return RedirectToAction("ProcessResponse");
        }

        [HttpGet]
        public IActionResult ProcessResponse(ProcessResponse pResponse)
        {
            string strUser = TempData.Peek("pResponse").ToString();
            pResponse = JsonConvert.DeserializeObject<ProcessResponse>(strUser);

            return View(pResponse);
        }

        [HttpGet]
        public async Task<IActionResult> CompleteProcess(int RequestId)
        {

            ProcessResponse pResponse = new ProcessResponse();
            string pay;

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://localhost:54592/api/Process/CompleteProcessing/" +RequestId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    pay = Convert.ToString(apiResponse);
                }
                TempData["pay"] = pay;
            }

            
            return View();
        }

        [HttpGet]
        public IActionResult Payment()
        {
            PaymentModel paymentModel = new PaymentModel();
            paymentModel.RequestId = (int)TempData.Peek("RequestId");
            paymentModel.CreditLimit = 40000;
            return View(paymentModel);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(PaymentModel paymentModel)
        {
            //PaymentModel paymentModel = new PaymentModel();
            //paymentModel.RequestId = RequestId;
            string apiResponse;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(paymentModel), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:44316/api/Payment/GetPaymentDetails" ,content))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                    //paymentModel = JsonConvert.DeserializeObject<PaymentModel>(apiResponse);

                }
                TempData["Balance"] = apiResponse;
            }


            return RedirectToAction("Success", "User" ,apiResponse);

        }

        public IActionResult Success(string apiResponse)
        {
            //TempData["Balance"]= apiResponse;
            return View(); 
        }

    }

}
