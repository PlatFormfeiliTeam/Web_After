using System;

namespace Web_After.model
{
    public class BASE_COMPANY
    {
        public BASE_COMPANY()
		{}
		#region Model
		private decimal _id;
		private string _code;
		private string _name;
		private string _remark;
		private decimal? _enabled;
		private decimal? _createman;
		private decimal? _stopman;
		private DateTime? _startdate;
		private DateTime? _enddate;
		private DateTime? _createdate;
		private string _englishname;
		private string _declnature;
		private string _inspcode;
		private string _incode;
		private string _inspnature;
		private string _goodslocal;
		private string _receivertype;
		private string _socialcreditno;
		/// <summary>
		/// 
		/// </summary>
		public decimal ID
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 海关代码
		/// </summary>
		public string CODE
		{
			set{ _code=value;}
			get{return _code;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string NAME
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string REMARK
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 1启用
		/// </summary>
		public decimal? ENABLED
		{
			set{ _enabled=value;}
			get{return _enabled;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal? CREATEMAN
		{
			set{ _createman=value;}
			get{return _createman;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal? STOPMAN
		{
			set{ _stopman=value;}
			get{return _stopman;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? STARTDATE
		{
			set{ _startdate=value;}
			get{return _startdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? ENDDATE
		{
			set{ _enddate=value;}
			get{return _enddate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CREATEDATE
		{
			set{ _createdate=value;}
			get{return _createdate;}
		}
		/// <summary>
		/// 英文名称
		/// </summary>
		public string ENGLISHNAME
		{
			set{ _englishname=value;}
			get{return _englishname;}
		}
		/// <summary>
		/// 报关企业性质
		/// </summary>
		public string DECLNATURE
		{
			set{ _declnature=value;}
			get{return _declnature;}
		}
		/// <summary>
		/// 报检代码
		/// </summary>
		public string INSPCODE
		{
			set{ _inspcode=value;}
			get{return _inspcode;}
		}
		/// <summary>
		/// 内部代码
		/// </summary>
		public string INCODE
		{
			set{ _incode=value;}
			get{return _incode;}
		}
		/// <summary>
		/// 商检性质
		/// </summary>
		public string INSPNATURE
		{
			set{ _inspnature=value;}
			get{return _inspnature;}
		}
		/// <summary>
		/// 货物存放地
		/// </summary>
		public string GOODSLOCAL
		{
			set{ _goodslocal=value;}
			get{return _goodslocal;}
		}
		/// <summary>
		/// 收货人类型
		/// </summary>
		public string RECEIVERTYPE
		{
			set{ _receivertype=value;}
			get{return _receivertype;}
		}
		/// <summary>
		/// 社会信用
		/// </summary>
		public string SOCIALCREDITNO
		{
			set{ _socialcreditno=value;}
			get{return _socialcreditno;}
		}
		#endregion Model

    }
}