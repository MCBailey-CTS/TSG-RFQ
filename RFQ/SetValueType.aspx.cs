using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace RFQ
{
    public partial class SetValueType : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String idx = Request["idx"];
            String FieldName=Request["crit"];
            String Condition = Request["cndt"];
            String Operation = Request["op"];
            Site master = new Site();
            litReturnText.Text = master.renderField(System.Convert.ToInt64(idx), FieldName, Condition);
        }
    }
}