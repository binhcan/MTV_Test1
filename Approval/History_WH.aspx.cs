﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Drawing;

namespace Approval
{
    public partial class History_WH : System.Web.UI.Page
    {
        string use_id, pat;
        DataProfile data = new DataProfile();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["per"] == null)
            {
                Response.Redirect("Default.aspx");
            }
            use_id = Session["id"].ToString();
            pat = Session["pat"].ToString();
            if (!IsPostBack)
            {
                //LoadReport();  
                Load_Org();
            }
        }
        public void Load_Org()
        {
            DataTable org = data.GetDataTable("select * from warehouse");
            drOrg.DataSource = org;
            drOrg.DataTextField = "warehouse";
            drOrg.DataValueField = "ID_WH";
            drOrg.DataBind();
        }
        public void LoadData()
        {
            string[] substrings = reservation.Value.Trim().Split('-');
            string starttime = substrings[0];
            string endtime = substrings[1];
            string sql1 = "select t.id, t.note_no, t.model,t.Jobname,t.reson,b.username as confirm_by,c.username as approval_by from it_note t"
                          + " left join it_note_user b on b.id =t.confirm_by " +
                          " left join it_note_user c on c.id =t.appro_by " +
                          " where t.warehouse='" + drOrg.SelectedValue + "' and t.status!=2  and t.part='" + pat + "' and t.checked = 1 and t.note_date between '" + starttime.Trim() + "' and '" + endtime.Trim() + "'";
            //string sql1 = "select t.note_no,b.ITEM,b.DESCRIPTION ,b.quantity,b.SEMILOT,b.quantity_act, t.reson,c.Description from it_note t"
            //             + " left join it_note_detail b on b.note_id=t.id left join IT_RESON_APPROVA c on  c.ID = t.Reasoncode where t.warehouse='" + drOrg.SelectedValue + "' and t.status!=2 and t.checked = 1 and t.note_date between '" + starttime.Trim() + "' and '" + endtime.Trim() + "' ";
            DataTable dt1 = data.GetDataTable(sql1);
            if (dt1.Rows.Count >= 0)
            {
                grvhistory.DataSource = dt1;
                grvhistory.DataBind();
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        int stt = 1;

        protected void grvhistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            grvhistory.PageIndex = e.NewPageIndex;
            int trang_thu = e.NewPageIndex;
            int so_dong = grvhistory.PageSize;
            stt = trang_thu * so_dong + 1;
            LoadData();
        }

        protected void btnexport_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=VoteHistoryExport.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                //To Export all pages
                grvhistory.AllowPaging = false;
                LoadData();

                grvhistory.HeaderRow.BackColor = Color.White;
                foreach (TableCell cell in grvhistory.HeaderRow.Cells)
                {
                    cell.BackColor = grvhistory.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in grvhistory.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = grvhistory.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = grvhistory.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                grvhistory.RenderControl(hw);

                //style to format numbers to string
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
        protected void btnEnd_Click(object sender, EventArgs e)
        {
            Response.Redirect("Manage_Detail.aspx");
        }

        protected void bthistorydetail_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in grvhistory.Rows)
            {
                CheckBox chk = (row.FindControl("cbSelectAll") as CheckBox);
                if (chk.Checked)
                {
                    int id = int.Parse(grvhistory.DataKeys[row.RowIndex].Value.ToString());
                    Response.Redirect("HistoryDetail.aspx?id=" + id);
                    break;
                }
            }
        }

        public string get_stt()
        {
            return Convert.ToString(stt++);
        }
    }
}