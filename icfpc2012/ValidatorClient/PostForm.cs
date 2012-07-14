using System;
using System.Text;
using System.Web;

namespace ValidatorClient
{
	internal class PostForm
	{
		public PostForm()
		{
			builder = new StringBuilder();
		}

		public void AddParam(string name, object value)
		{
			if(builder.Length > 0)
				builder.Append('&');
			var text = value is Enum ? ((Enum)value).ToString("d") : value.ToString();
			builder.AppendFormat("{0}={1}", name, HttpUtility.UrlEncode(text));
		}

		public string Content { get { return builder.ToString(); } }

		private readonly StringBuilder builder;
	}
}