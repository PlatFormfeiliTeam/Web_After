using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Web_After
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“ISplitPDF”。
    [ServiceContract]
    public interface ISplitPDF
    {
        [OperationContract]
        string loadform(string ordercode);
        [OperationContract]
        string merge(string ordercode, string userid, string fileids);
        [OperationContract]
        string loadpdf(string fileid, string ordercode);
        [OperationContract]
        string split(string ordercode, string pages, string fileid, string userid, string filetype,string username);
        [OperationContract]
        string cancelsplit(string ordercode, string fileid, string userid, string username);
        [OperationContract]
        string loadfile(string fileid);
        [OperationContract]
        string adjustpage(string currentPageTmp, string direction, string fileid, string ordercode);
        [OperationContract]
        void compress(string fileid,string path);
        [OperationContract]
        string loadattach(string ordercode);
        [OperationContract]
        string delete(string del_fileids, string ordercode);
        [OperationContract]
        Byte[] ReadPdf(string path);
       
    }
}
