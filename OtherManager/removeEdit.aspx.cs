using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_After.Common;

namespace Web_After.OtherManager
{
    public partial class removeEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string action = Request["action"] + "";
            if (action == "remove")
            {
                // string formdata = Request["formdata"];
                string item = Request["item"] + "";
                string combo_bhxz = Request["combo_bhxz"] + "";
                string itembh = Request["itembh"] + "";
                string sqlorder, sqldec;
                string where;
                switch (item)
                {
                    case "kf":
                        if (combo_bhxz == "ddbh")
                        {
                            where = "where code='" + itembh + "'";
                        }
                        else
                        {
                            where = "where cusno='" + itembh + "'";

                        }
                        sqlorder = "update list_order set CSEDIT=0,CSCURRENTID=null,CSCURRENTNAME=null " + where;
                        DBMgr.ExecuteNonQuery(sqlorder);
                        break;
                    case "zd":
                        if (combo_bhxz == "ddbh")
                        {
                            where = "where code='" + itembh + "'";
                        }
                        else
                        {
                            where = "where cusno='" + itembh + "'";
                            DataTable dt_tmp = DBMgr.GetDataTable("select code from list_order " + where);
                            string ddbh_value = dt_tmp.Rows[0][0].ToString().Trim();
                            sqldec = "update list_declaration set MOEDIT=0,MOCURRENTID=null,MOCURRENTNAME=null where ordercode='" + ddbh_value + "'";
                            DBMgr.ExecuteNonQuery(sqldec);
                        }
                        sqlorder = "update list_order set MOEDIT=0 " + where;
                        DBMgr.ExecuteNonQuery(sqlorder);

                        break;

                    case "sd":
                        if (combo_bhxz == "ddbh")
                        {
                            where = "where code='" + itembh + "'";
                        }
                        else
                        {
                            where = "where cusno='" + itembh + "'";
                            DataTable dt_tmp = DBMgr.GetDataTable("select code from list_order " + where);
                            string ddbh_value = dt_tmp.Rows[0][0].ToString().Trim();
                            sqldec = "update list_declaration set COEDIT=0,COCURRENTID=null,COCURRENTNAME=null where ordercode='" + ddbh_value + "'";
                            DBMgr.ExecuteNonQuery(sqldec);


                        }
                        sqlorder = "update list_order set COEDIT=0 " + where;
                        DBMgr.ExecuteNonQuery(sqlorder);
                        break;
                }
                Response.Write("{success:true}");
                Response.End();
            }


        }
    }
}