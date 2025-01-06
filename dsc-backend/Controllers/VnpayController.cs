using dsc_backend.DAO;
using dsc_backend.Helper;
using dsc_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace dsc_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VnpayController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DscContext _db;
        private readonly Md5Helper _hashpass;
        private readonly CloudinarySettings _cloudinarySettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VnpayController(ILogger<AuthenController> logger, DscContext db, IOptions<CloudinarySettings> cloudinarySettings, IWebHostEnvironment webHostEnvironment, Md5Helper hashpass)
        {
            _logger = logger;
            _hashpass = hashpass;
            _db = db;
            _cloudinarySettings = cloudinarySettings.Value;
            _webHostEnvironment = webHostEnvironment;
        }
            [HttpPost("create_payment_url")]
            public IActionResult CreatePaymentUrl([FromForm] PaymentRequest request)
            {
                // Set timezone
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);

                string createDate = date.ToString("yyyyMMddHHmmss");
                string orderId = date.ToString("ddHHmmss");

                // Get client IP
                string ipAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

                // Config params
                string tmnCode = "0JQSW1QI";
                string secretKey = "SEQOYCYJCOM9HQ6FEQC0O9KSA9TEKB1A";
                string vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string returnUrl = "http://localhost:3000/createTournament";
                //string bankCode = "VNBANK";
                string locale = "vn";

                // Build vnp_Params
                var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Locale", locale },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", request.TournamentId.ToString() },
            { "vnp_OrderType", "other" },
            { "vnp_Amount", (request.Amount*100).ToString() },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", ipAddr },
            { "vnp_CreateDate", createDate },
            { "vnp_BankCode", "VNBANK" }
        };


                // Sort params
                var sortedParams = new SortedDictionary<string, string>(vnpParams);

                // Build query string
                var queryBuilder = new StringBuilder();
                foreach (var param in sortedParams)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                    {
                        queryBuilder.Append(WebUtility.UrlEncode(param.Key));
                        queryBuilder.Append("=");
                        queryBuilder.Append(WebUtility.UrlEncode(param.Value));
                        queryBuilder.Append("&");
                    }
                }
                string signData = queryBuilder.ToString().TrimEnd('&');

                // Create HMAC SHA512 signature
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
                {
                    var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                    var sign = BitConverter.ToString(signBytes).Replace("-", "").ToLower();
                    sortedParams.Add("vnp_SecureHash", sign);
                }

                // Build final URL
                var finalUrl = vnpUrl + "?" + string.Join("&",
                    sortedParams.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));

                return Ok(finalUrl);
        }
        [HttpGet("vnpay_ipn")]
        public IActionResult VnPayIPN([FromQuery] Dictionary<string, string> vnpParams)
        {
            try
            {
                // Lấy các tham số
                string secureHash = vnpParams.GetValueOrDefault("vnp_SecureHash");
                string bookingName = vnpParams.GetValueOrDefault("vnp_OrderInfo");
                string orderId = vnpParams.GetValueOrDefault("vnp_TxnRef");
                string rspCode = vnpParams.GetValueOrDefault("vnp_ResponseCode");
                string OrderInfo = vnpParams.GetValueOrDefault("vnp_OrderInfo");

                // Xóa các tham số hash
                vnpParams.Remove("vnp_SecureHash");
                vnpParams.Remove("vnp_SecureHashType");

                // Tạo chữ ký
                string secretKey = "GSMYNKXFMYYUDFUCHAVBEJXXLIQZZUED";

                // Sắp xếp params và tạo query string
                var sortedParams = new SortedDictionary<string, string>(vnpParams);
                var signData = string.Join("&", sortedParams
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={kv.Value}"));

                // Tạo chữ ký HMAC SHA512
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
                {
                    var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                    var signed = BitConverter.ToString(signBytes).Replace("-", "").ToLower();

                    // Giả sử paymentStatus = "0" là trạng thái khởi tạo
                    string paymentStatus = "0";

                    if (paymentStatus == "0")
                    {
                        if (rspCode == "00")
                        {
                            // Thanh toán thành công
                            //await _customerService.Payment(bookingName);
                            return Ok(new { RspCode = "00", Message = "Success", tournamentId = OrderInfo });
                        }
                        else
                        {
                            return Ok(new { RspCode = "24", Message = "Failed", tournamentId = OrderInfo });
                        }
                    }
                    else
                    {
                        return Ok(new { RspCode = "02", Message = "This order has been updated to the payment status", tournamentId = OrderInfo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpPost("create_payment_url_clb")]
        public IActionResult CreatePaymentUrlClub([FromForm] PaymentRequest request)
        {
            // Set timezone
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);

            string createDate = date.ToString("yyyyMMddHHmmss");
            string orderId = date.ToString("ddHHmmss");

            // Get client IP
            string ipAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            // Config params
            string tmnCode = "0JQSW1QI";
            string secretKey = "SEQOYCYJCOM9HQ6FEQC0O9KSA9TEKB1A";
            string vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "http://localhost:3000/myclub";
            //string bankCode = "VNBANK";
            string locale = "vn";

            // Build vnp_Params
            var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Locale", locale },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", request.TournamentId.ToString() },
            { "vnp_OrderType", "other" },
            { "vnp_Amount", (request.Amount*100).ToString() },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", ipAddr },
            { "vnp_CreateDate", createDate },
            { "vnp_BankCode", "VNBANK" }
        };


            // Sort params
            var sortedParams = new SortedDictionary<string, string>(vnpParams);

            // Build query string
            var queryBuilder = new StringBuilder();
            foreach (var param in sortedParams)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    queryBuilder.Append(WebUtility.UrlEncode(param.Key));
                    queryBuilder.Append("=");
                    queryBuilder.Append(WebUtility.UrlEncode(param.Value));
                    queryBuilder.Append("&");
                }
            }
            string signData = queryBuilder.ToString().TrimEnd('&');

            // Create HMAC SHA512 signature
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                var sign = BitConverter.ToString(signBytes).Replace("-", "").ToLower();
                sortedParams.Add("vnp_SecureHash", sign);
            }

            // Build final URL
            var finalUrl = vnpUrl + "?" + string.Join("&",
                sortedParams.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));

            return Ok(finalUrl);
        }
        [HttpGet("vnpay_ipn_club")]
        public IActionResult VnPayIPNClub([FromQuery] Dictionary<string, string> vnpParams)
        {
            try
            {
                // Lấy các tham số
                string secureHash = vnpParams.GetValueOrDefault("vnp_SecureHash");
                string bookingName = vnpParams.GetValueOrDefault("vnp_OrderInfo");
                string orderId = vnpParams.GetValueOrDefault("vnp_TxnRef");
                string rspCode = vnpParams.GetValueOrDefault("vnp_ResponseCode");
                string OrderInfo = vnpParams.GetValueOrDefault("vnp_OrderInfo");

                // Xóa các tham số hash
                vnpParams.Remove("vnp_SecureHash");
                vnpParams.Remove("vnp_SecureHashType");

                // Tạo chữ ký
                string secretKey = "GSMYNKXFMYYUDFUCHAVBEJXXLIQZZUED";

                // Sắp xếp params và tạo query string
                var sortedParams = new SortedDictionary<string, string>(vnpParams);
                var signData = string.Join("&", sortedParams
                    .Where(kv => !string.IsNullOrEmpty(kv.Value))
                    .Select(kv => $"{kv.Key}={kv.Value}"));

                // Tạo chữ ký HMAC SHA512
                using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
                {
                    var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                    var signed = BitConverter.ToString(signBytes).Replace("-", "").ToLower();

                    // Giả sử paymentStatus = "0" là trạng thái khởi tạo
                    string paymentStatus = "0";

                    if (paymentStatus == "0")
                    {
                        if (rspCode == "00")
                        {
                            // Thanh toán thành công
                            //await _customerService.Payment(bookingName);
                            return Ok(new { RspCode = "00", Message = "Success", clubId = OrderInfo });
                        }
                        else
                        {
                            return Ok(new { RspCode = "24", Message = "Failed", clubId = OrderInfo });
                        }
                    }
                    else
                    {
                        return Ok(new { RspCode = "02", Message = "This order has been updated to the payment status", clubId = OrderInfo });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpPost("create_payment_clb")]
        public IActionResult CreatePaymentClub([FromForm] PaymentRequest request)
        {
            // Set timezone
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);

            string createDate = date.ToString("yyyyMMddHHmmss");
            string orderId = date.ToString("ddHHmmss");

            // Get client IP
            string ipAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            // Config params
            string tmnCode = "0JQSW1QI";
            string secretKey = "SEQOYCYJCOM9HQ6FEQC0O9KSA9TEKB1A";
            string vnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string returnUrl = "http://localhost:3000/createclub";
            //string bankCode = "VNBANK";
            string locale = "vn";

            // Build vnp_Params
            var vnpParams = new Dictionary<string, string>
        {
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_Locale", locale },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", orderId },
            { "vnp_OrderInfo", request.TournamentId.ToString() },
            { "vnp_OrderType", "other" },
            { "vnp_Amount", (request.Amount*100).ToString() },
            { "vnp_ReturnUrl", returnUrl },
            { "vnp_IpAddr", ipAddr },
            { "vnp_CreateDate", createDate },
            { "vnp_BankCode", "VNBANK" }
        };


            // Sort params
            var sortedParams = new SortedDictionary<string, string>(vnpParams);

            // Build query string
            var queryBuilder = new StringBuilder();
            foreach (var param in sortedParams)
            {
                if (!string.IsNullOrEmpty(param.Value))
                {
                    queryBuilder.Append(WebUtility.UrlEncode(param.Key));
                    queryBuilder.Append("=");
                    queryBuilder.Append(WebUtility.UrlEncode(param.Value));
                    queryBuilder.Append("&");
                }
            }
            string signData = queryBuilder.ToString().TrimEnd('&');

            // Create HMAC SHA512 signature
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                var signBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(signData));
                var sign = BitConverter.ToString(signBytes).Replace("-", "").ToLower();
                sortedParams.Add("vnp_SecureHash", sign);
            }

            // Build final URL
            var finalUrl = vnpUrl + "?" + string.Join("&",
                sortedParams.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));

            return Ok(finalUrl);
        }
    }
}
