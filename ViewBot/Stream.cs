using System.Runtime.Serialization;

namespace ViewBot
{
	[DataContract]
	public class Headers
	{
		[DataMember(Name = "__invalid_name__Client-ID")]
		public string Invalid_name__Client_ID { get; set; }
		[DataMember(Name = "__invalid_name__Accept-Encoding")]
		public string Invalid_name__Accept_Encoding { get; set; }
		[DataMember(Name = "Accept")]
		public string Accept { get; set; }
		[DataMember(Name = "__invalid_name__User-Agent")]
		public string Invalid_name__User_Agent { get; set; }
	}

	[DataContract]
	public class Medium
	{
		[DataMember(Name= "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Mobile
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class High
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Source
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Worst
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Low
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Audio
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Best
	{
		[DataMember(Name = "headers")]
		public Headers Headers { get; set; }
		[DataMember(Name = "type")]
		public string Type { get; set; }
		[DataMember(Name = "url")]
		public string Url { get; set; }
	}

	[DataContract]
	public class Streams
	{
		[DataMember(Name = "medium")]
		public Medium Medium { get; set; }
		[DataMember(Name = "mobile")]
		public Mobile Mobile { get; set; }
		[DataMember(Name = "hign")]
		public High High { get; set; }
		[DataMember(Name = "source")]
		public Source Source { get; set; }
		[DataMember(Name = "worst")]
		public Worst Worst { get; set; }
		[DataMember(Name = "low")]
		public Low Low { get; set; }
		[DataMember(Name = "audio")]
		public Audio Audio { get; set; }
		[DataMember(Name = "best")]
		public Best Best { get; set; }
	}

	[DataContract]
	public class StreamData
	{
		[DataMember(Name = "streams")]
		public Streams Streams { get; set; }
		[DataMember(Name = "plugin")]
		public string Plugin { get; set; }
	}

	[DataContract]
	public class Token
	{
		[DataMember(Name = "token")]
		public string SToken { get; set; }

		[DataMember(Name = "sig")]
		public string Sig { get; set; }
	}

}
