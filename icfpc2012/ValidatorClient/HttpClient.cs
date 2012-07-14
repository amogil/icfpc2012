using System.IO;
using System.Net;
using System.Text;

namespace ValidatorClient
{
	internal static class HttpClient
	{
		public static string SendRequest(string map, string moves)
		{
			var postForm = new PostForm();
			postForm.AddParam("mapfile", map);
			postForm.AddParam("route", moves);
			return SendRequest(postForm);
		}

		private static string SendRequest(PostForm postForm)
		{
			var request = CreateRequest();

			var data = Encoding.UTF8.GetBytes(postForm.Content);
			request.ContentLength = data.Length;

			using(var requestStream = request.GetRequestStream())
				requestStream.Write(data, 0, data.Length);

			var webResponse = request.GetResponse();
			using(var streamReader = new StreamReader(webResponse.GetResponseStream()))
				return streamReader.ReadToEnd();
		}

		private static WebRequest CreateRequest()
		{
			var request = (HttpWebRequest)WebRequest.Create(Url);
			request.Method = "POST";
			request.Proxy = null;
			request.ContentType = "application/x-www-form-urlencoded";
			request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:12.0) Gecko/20100101 Firefox/12.0";
			request.ServicePoint.UseNagleAlgorithm = false;
			request.Referer = Url;
			return request;
		}

		private const string Url = "http://undecidable.org.uk/~edwin/cgi-bin/weblifter.cgi";
	}
}