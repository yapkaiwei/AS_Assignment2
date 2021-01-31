using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Assignment2
{
    public partial class _400 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new HttpException(400, "Click on continue to see it on the webpage!");
        }
    }
}