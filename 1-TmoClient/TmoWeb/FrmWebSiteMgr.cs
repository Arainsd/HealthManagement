using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TmoSkin;
using TmoCommon;
using DevExpress.XtraNavBar;

namespace TmoWeb
{
    public partial class FrmWebSiteMgr : UCBase
    {
        ucReadList _ucReadList = null;
        ucAboutUs _ucAbout = null;
        /// <summary>
        /// �����Ķ�
        /// </summary>
        //UcReadList _ucReadList = null;
        ///// <summary>
        ///// ��������
        ///// </summary>
        //UcAbout _ucAbout = null;
        public FrmWebSiteMgr()
        {
            InitializeComponent();
            foreach (NavBarItem nbi in navBarControl1.Items)
                nbi.LinkClicked += new DevExpress.XtraNavBar.NavBarLinkEventHandler(nbi_LinkClicked);
            Title = "��վ����";
            _ucReadList = new ucReadList();
            PlWorkAdd(_ucReadList);
        }


        void nbi_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            string objName = (sender as NavBarItem).Name.ToLower();
            switch (objName)
            {
                case "nbi_read":
                    if (_ucReadList == null)
                        _ucReadList = new ucReadList();
                    PlWorkAdd(_ucReadList);
                    break;
                case "nbi_about":
                    if (_ucAbout == null)
                        _ucAbout = new ucAboutUs();
                    PlWorkAdd(_ucAbout);
                    break;
                default:

                    break;
            }
            this.Text = "��վ���� - " + (sender as NavBarItem).Caption; ;
        }


        /// <summary>
        /// ����������ӿؼ�
        /// </summary>
        /// <param name="control"></param>
        private void PlWorkAdd(System.Windows.Forms.Control control)
        {
            try
            {
                foreach (Control col in this.plWork.Controls)
                {
                    if (col != control)
                    {
                        col.Visible = false;
                    }
                }
                if (!this.plWork.Controls.Contains(control))
                    this.plWork.Controls.Add(control);
                control.Visible = true;
                control.Dock = DockStyle.Fill;
            }
            catch
            {

            }
        }

    }
}