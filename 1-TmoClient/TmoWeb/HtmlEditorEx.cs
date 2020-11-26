using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using TmoSkin;

namespace TmoWeb
{
    /// <summary>
    /// Html�༭��
    /// Դ�������Ի�������������è(DeltaCat)���˽�һ����չ�Ͳ�������
    /// http://www.zu14.cn/
    /// ��ע������֧�ֿ�Դ
    /// </summary>
    [Description("Html�༭��"), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public partial class HtmlEditorEx : UCBase
    {
        public HtmlEditorEx()
        {
            dataUpdate = 0;
            InitializeComponent();
            InitializeControls();
        }

        #region ��չ����
        /// <summary>
        /// �ӿ��ַ���
        /// </summary>
        string attachmentXml = TmoCommon.TmoShare.XML_TITLE +
@"<well_web_attachment>
<att_id></att_id>
<filename></filename>
<filesize></filesize>
<content></content>
<source></source>
<doc_code></doc_code>
<article_id></article_id>
<input_time></input_time>
</well_web_attachment>";
        /// <summary>
        /// ��ȡ�����õ�ǰ��Html�ı�
        /// </summary>
        public string Html
        {
            get
            {
                return webBrowserBody.DocumentText;
            }
            set
            {

                webBrowserBody.Document.Write(value.Replace("\r\n", ""));
                //webBrowserBody.DocumentText = value.Replace("\r\n", "");
                //webBrowserBody.DocumentText = "";


                //JAFly 2014-11-09
                //var s = webBrowserBody.Document.Images[0];
                //<IMG border=0 hspace=0 alt="" src="~\Read\newFileName.jpg" align=baseline>
                //2 ����bin\Read ����ͼƬ�Ƿ����
                //3 ������ز����ڣ����÷���ˣ���ϵͳ�������л�ȡͼƬ��������ͼƬ������bin\Read��
                //4 ˢ��ҳ��

                //ע����վͬ��
            }
        }

        public string GetHtmlValue()
        {
            return Html.Substring(Html.LastIndexOf("<BODY>") + 6).Replace("</BODY>", "").Replace("</HTML>", "");
        }

        /// <summary>
        /// ��ȡ������ͼƬ��Դ��HTML BODY �� MailMessage
        /// </summary>
        public MailMessage XMailMessage
        {
            get
            {
                MailMessage msg = new MailMessage();
                msg.IsBodyHtml = true;

                string html = webBrowserBody.DocumentText;

                HtmlElementCollection images = webBrowserBody.Document.Images;
                if (images.Count > 0)
                {
                    List<string> imgs = new List<string>();

                    foreach (HtmlElement image in images)
                    {
                        string imagePath = image.GetAttribute("src");
                        if (imagePath.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
                        {
                            imgs.Add(imagePath);
                        }
                    }

                    for (int i = 0; i < imgs.Count; i++)
                    {
                        string cid = string.Format("em_image_{0:00}", i);
                        string imagePath = Path.GetFullPath(imgs[i].Replace("%20", " ").Replace("file:///", ""));
                        Attachment attach = new Attachment(imagePath);
                        attach.ContentId = cid;
                        attach.Name = Path.GetFileNameWithoutExtension(imagePath);
                        msg.Attachments.Add(attach);

                        html.Replace(imgs[i], string.Format("cid:{0}", cid));
                    }
                }

                msg.Body = html;

                return msg;
            }
        }

        /// <summary>
        /// ��ȡ�����ͼƬ���Ƽ���
        /// </summary>
        public string[] Images
        {
            get
            {
                List<string> images = new List<string>();

                foreach (HtmlElement element in webBrowserBody.Document.Images)
                {
                    string image = element.GetAttribute("src");
                    if (!images.Contains(image))
                    {
                        images.Add(image);
                    }
                }

                return images.ToArray();
            }
        }

        #endregion

        #region ����

        /// <summary>
        /// �ؼ���ʼ��
        /// </summary>
        private void InitializeControls()
        {
            BeginUpdate();

            //������
            foreach (FontFamily family in FontFamily.Families)
            {
                toolStripComboBoxName.Items.Add(family.Name);
            }

            toolStripComboBoxSize.Items.AddRange(FontSize.All.ToArray());

            //�����
            webBrowserBody.DocumentText = string.Empty;

            webBrowserBody.Document.Click += new HtmlElementEventHandler(webBrowserBody_DocumentClick);
            webBrowserBody.Document.Focusing += new HtmlElementEventHandler(webBrowserBody_DocumentFocusing);
            webBrowserBody.Document.ExecCommand("EditMode", false, null);
            webBrowserBody.Document.ExecCommand("LiveResize", false, null);

            EndUpdate();
        }


        /// <summary>
        /// ˢ�°�ť״̬
        /// </summary>
        private void RefreshToolBar()
        {
            BeginUpdate();

            try
            {
                mshtml.IHTMLDocument2 document = (mshtml.IHTMLDocument2)webBrowserBody.Document.DomDocument;

                toolStripComboBoxName.Text = document.queryCommandValue("FontName").ToString();
                toolStripComboBoxSize.SelectedItem = FontSize.Find((int)document.queryCommandValue("FontSize"));
                toolStripButtonBold.Checked = document.queryCommandState("Bold");
                toolStripButtonItalic.Checked = document.queryCommandState("Italic");
                toolStripButtonUnderline.Checked = document.queryCommandState("Underline");

                toolStripButtonNumbers.Checked = document.queryCommandState("InsertOrderedList");
                toolStripButtonBullets.Checked = document.queryCommandState("InsertUnorderedList");

                toolStripButtonLeft.Checked = document.queryCommandState("JustifyLeft");
                toolStripButtonCenter.Checked = document.queryCommandState("JustifyCenter");
                toolStripButtonRight.Checked = document.queryCommandState("JustifyRight");
                toolStripButtonFull.Checked = document.queryCommandState("JustifyFull");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                EndUpdate();
            }
        }

        #endregion

        #region �������

        private int dataUpdate;
        private bool Updating
        {
            get
            {
                return dataUpdate != 0;
            }
        }

        private void BeginUpdate()
        {
            ++dataUpdate;
        }
        private void EndUpdate()
        {
            --dataUpdate;
        }

        #endregion

        #region ������

        private void toolStripComboBoxName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("FontName", false, toolStripComboBoxName.Text);
        }
        private void toolStripComboBoxSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            int size = (toolStripComboBoxSize.SelectedItem == null) ? 1 : (toolStripComboBoxSize.SelectedItem as FontSize).Value;
            webBrowserBody.Document.ExecCommand("FontSize", false, size);
        }
        private void toolStripButtonBold_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("Bold", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonItalic_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("Italic", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonUnderline_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("Underline", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            int fontcolor = (int)((mshtml.IHTMLDocument2)webBrowserBody.Document.DomDocument).queryCommandValue("ForeColor");

            ColorDialog dialog = new ColorDialog();
            dialog.Color = Color.FromArgb(0xff, fontcolor & 0xff, (fontcolor >> 8) & 0xff, (fontcolor >> 16) & 0xff);

            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string color = dialog.Color.Name;
                if (!dialog.Color.IsNamedColor)
                {
                    color = "#" + color.Remove(0, 2);
                }

                webBrowserBody.Document.ExecCommand("ForeColor", false, color);
            }
            RefreshToolBar();
        }

        private void toolStripButtonNumbers_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("InsertOrderedList", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonBullets_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("InsertUnorderedList", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonOutdent_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("Outdent", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonIndent_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("Indent", false, null);
            RefreshToolBar();
        }

        private void toolStripButtonLeft_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("JustifyLeft", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonCenter_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("JustifyCenter", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonRight_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("JustifyRight", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonFull_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("JustifyFull", false, null);
            RefreshToolBar();
        }

        private void toolStripButtonLine_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("InsertHorizontalRule", false, null);
            RefreshToolBar();
        }
        private void toolStripButtonHyperlink_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("CreateLink", true, null);
            RefreshToolBar();
        }
        private void toolStripButtonPicture_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }


   //var s = webBrowserBody.Document.Images[0];
            webBrowserBody.Document.ExecCommand("InsertImage", true, null);
            #region ��ʱ����
            //            if (Directory.Exists(Server.MapPath(~/upimg/hufu)) == false)//��������ھʹ���file�ļ���{
            //Directory.CreateDirectory(Server.MapPath(~/upimg/hufu));}
            ////Directory.Delete(Server.MapPath(~/upimg/hufu), true);//ɾ���ļ����Լ��ļ����е���Ŀ¼���ļ�
            ////�ж��ļ��Ĵ���
            //if (File.Exists(Server.MapPath(~/upimg/Data.html))){
            //Response.Write(Yes);//�����ļ�}else{
            //Response.Write(No);
            ////�������ļ�
            //File.Create(MapPath(~/upimg/Data.html));//�������ļ�}
            //string name = GetFiles.FileName;//��ȡ���ϴ��ļ�������
            //string size = GetFiles.PostedFile.ContentLength.ToString();//��ȡ���ϴ��ļ��Ĵ�С
            //string type = GetFiles.PostedFile.ContentType;//��ȡ���ϴ��ļ���MIME
            //string postfix = name.Substring(name.LastIndexOf(.) + 1);//��ȡ���ϴ��ļ��ĺ�׺
            //string ipath = Server.MapPath(upimg) +\\+ name;//��ȡ�ļ���ʵ��·��
            //string fpath = Server.MapPath(upfile) + \\ + name;
            //string dpath = upimg\\ + name;//�ж�д�����ݿ������·��
            //ShowPic.Visible = true;//����
            //ShowText.Visible = true;//����
            ////�ж��ļ���ʽ
            //if (name == ) {
            //Response.Write(<scriptalert('�ϴ��ļ�����Ϊ��')</script);}else{
            //if (postfix == jpg || postfix == gif || postfix == bmp || postfix == png){
            //GetFiles.SaveAs(ipath);
            //ShowPic.ImageUrl = dpath;
            //ShowText.Text = ���ϴ���ͼƬ������: + name +  + �ļ���С: + size + KB +  + �ļ�����: + type +  + ��ŵ�ʵ��·��Ϊ: + ipath;}else{
            //ShowPic.Visible = false;//����ͼƬ
            //GetFiles.SaveAs(fpath);//���ڲ���ͼƬ�ļ�,���ת����upfile����ļ���
            //ShowText.Text = ���ϴ����ļ�������: + name +  + �ļ���С: + size + KB +  + �ļ�����: + type +  + ��ŵ�ʵ��·��Ϊ: + fpath;} 
            #endregion
         
            RefreshToolBar();
        }

        private void toolStripButtonUnDo_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("UnDo", false, null);
            RefreshToolBar();
        }

        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }

            webBrowserBody.Document.ExecCommand("ReDo", false, null);
            RefreshToolBar();
        }
        public string  HandlHtml()
        {
            string content = GetHtmlValue();
            string[] imgSrcs = GetImageUrlList(content);
            string[] Img = GetImageList(content);
      

            if (imgSrcs.Length > 0)
            {
                for (int i = 0; i < imgSrcs.Length; i++)
                {
                    string str = imgSrcs[i];
                    byte[] ImageDatas = ImageDatabytes(str);
                    string DataBytes = ToBase64Str(ImageDatas);
                    if (ImageDatas.Length > (1048576 * 5))//ͼƬ���ܴ���2��
                    {

                        content = content.Replace(Img[i], "");
                        continue;
                    }
                    else if (ImageDatas.Length <= 0)
                    {
                        content = content.Replace(Img[i], "");
                        continue;
                    }
                    else
                    {
                        content = content.Replace(str, "data:image/jgp;base64," + DataBytes);
                     }

                }
            }
            return content;
        }
        public string SaveImg(out object objAllAttInfo)
        {
            objAllAttInfo = null;
            DataSet dsXml = TmoCommon.TmoShare.getDataSetFromXML(attachmentXml);

            if (TmoCommon.TmoShare.DataSetIsNotEmpty(dsXml))
            {
                dsXml.Tables[0].Rows.Clear();
            }

            string content = GetHtmlValue();
            string[] imgSrcs = GetImageUrlList(content);
            string[] Img = GetImageList(content);
            if (imgSrcs.Length > 0)
            {
                for (int i = 0; i < imgSrcs.Length; i++)
                {
                    string str = imgSrcs[i];
                    byte[] ImageDatas = ImageDatabytes(str);
                    string DataBytes = ToBase64Str(ImageDatas);
                    if (ImageDatas.Length > (1048576 * 5))//ͼƬ���ܴ���2��
                    {

                        content = content.Replace(Img[i], "");
                        continue;
                    }
                    else if (ImageDatas.Length <= 0)
                    {
                        content = content.Replace(Img[i], "");
                        continue;
                    }
                    else
                    {

                        string FileName = "";
                        string FileNameWithoutExtension = Path.GetFileNameWithoutExtension(str);
                        string OldDirePath = Path.GetDirectoryName(str);
                        string OldfullPath = Path.GetFullPath(str);
                        string strpath = TmoCommon.TmoShare.GetRootPath();
                        Bitmap Newimg = GetImage(ImageDatas);
                        //�ļ���·��
                        string direPath = strpath + "\\" + "Read";
                        if (OldDirePath == direPath)
                        {

                            if (Directory.Exists(direPath) == false)
                                Directory.CreateDirectory(direPath);
                            if (!File.Exists(OldfullPath))
                            {
                                Newimg.Save(OldfullPath, System.Drawing.Imaging.ImageFormat.Png);
                                content = content.Replace(str, OldfullPath);

                                //webBrowserBody.DocumentText = webBrowserBody.DocumentText.Replace(str, filePath);
                            }

                            DataRow newrow = dsXml.Tables[0].NewRow();
                            newrow["att_id"] = FileNameWithoutExtension;
                            newrow["filesize"] = ImageDatas.Length;
                            newrow["filename"] = OldfullPath;
                            newrow["content"] = DataBytes;
                            newrow["source"] = 2;
                            dsXml.Tables[0].Rows.Add(newrow);

                        }
                        else
                        {

                            //�ļ�·��
                            string fileNameNoEx = FileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            FileName = fileNameNoEx + ".png";
                            string filePath = direPath + "\\" + FileName;
                            if (Directory.Exists(direPath) == false)
                                Directory.CreateDirectory(direPath);
                            if (!File.Exists(filePath))
                            {
                                Newimg.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                                Newimg.Dispose();
                                content = content.Replace(str, filePath);
                                //webBrowserBody.DocumentText = webBrowserBody.DocumentText.Replace(str, filePath);
                            }
                            else
                            {
                                content = content.Replace(str, filePath);
                                TmoCommon.UserMessageBox.MessageQuestion("�Ѵ��ڸ�ͼƬ");
                                return "";
                            }

                            DataRow newrow = dsXml.Tables[0].NewRow();
                            newrow["att_id"] = fileNameNoEx;
                            newrow["filesize"] = ImageDatas.Length;
                            newrow["filename"] = filePath;
                            newrow["content"] = DataBytes;
                            newrow["source"] = 2;
                            dsXml.Tables[0].Rows.Add(newrow);


                        }

                    }

                }
                objAllAttInfo = TmoCommon.TmoShare.getXMLFromDataSet(dsXml).ToString();
                //if (!well_web_attachmentManager.Instance.AddAttachment(WellCommon.WellCareShare.LoginCode, WellCommon.WellCareShare._GetXml(dsXml).ToString()))
                //{
                //    UserMessageBox.MessageQuestion("����ͼƬʧ��");
                //}
            }
            return content;
        }
        #endregion

        #region �����

        private void webBrowserBody_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
        }

        private void webBrowserBody_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.IsInputKey)
            {
                return;
            }

            RefreshToolBar();
        }

        private void webBrowserBody_DocumentClick(object sender, HtmlElementEventArgs e)
        {
            RefreshToolBar();
        }

        private void webBrowserBody_DocumentFocusing(object sender, HtmlElementEventArgs e)
        {
            RefreshToolBar();
        }

        #endregion

        #region �����Сת��

        private class FontSize
        {
            private static List<FontSize> allFontSize = null;
            public static List<FontSize> All
            {
                get
                {
                    if (allFontSize == null)
                    {
                        allFontSize = new List<FontSize>();
                        allFontSize.Add(new FontSize(8, 1));
                        allFontSize.Add(new FontSize(10, 2));
                        allFontSize.Add(new FontSize(12, 3));
                        allFontSize.Add(new FontSize(14, 4));
                        allFontSize.Add(new FontSize(18, 5));
                        allFontSize.Add(new FontSize(24, 6));
                        allFontSize.Add(new FontSize(36, 7));
                    }

                    return allFontSize;
                }
            }

            public static FontSize Find(int value)
            {
                if (value < 1)
                {
                    return All[0];
                }

                if (value > 7)
                {
                    return All[6];
                }

                return All[value - 1];
            }

            private FontSize(int display, int value)
            {
                displaySize = display;
                valueSize = value;
            }

            private int valueSize;
            public int Value
            {
                get
                {
                    return valueSize;
                }
            }

            private int displaySize;
            public int Display
            {
                get
                {
                    return displaySize;
                }
            }

            public override string ToString()
            {
                return displaySize.ToString();
            }
        }

        #endregion

        #region ������

        private class ToolStripComboBoxEx : ToolStripComboBox
        {
            public override Size GetPreferredSize(Size constrainingSize)
            {
                Size size = base.GetPreferredSize(constrainingSize);
                size.Width = Math.Max(Width, 0x20);
                return size;
            }
        }

        #endregion
        #region �����ַ���������img��ǩ��srcֵ
        /// <summary>
        /// ���ܷ����ַ���������img��ǩ��srcֵ
        /// ������Ա�����
        /// ʱ�䣺2014-11-10
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public string[] GetImageUrlList(string ImgStr)
        {
            // ����������ʽ����ƥ�� img ��ǩ   
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // ����ƥ����ַ���   
            MatchCollection matches = regImg.Matches(ImgStr);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // ȡ��ƥ�����б�   
            foreach (Match match in matches)
                sUrlList[i++] = match.Groups["imgUrl"].Value;
            return sUrlList;
        }
        /// <summary>
        /// ���ܷ�������IMG��ǩ
        /// ������Ա�����
        /// ʱ�䣺2014-11-10
        /// </summary>
        /// <param name="sHtmlText"></param>
        /// <returns></returns>
        public string[] GetImageList(string ImgStr)
        {
            // ����������ʽ����ƥ�� img ��ǩ   
            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

            // ����ƥ����ַ���   
            MatchCollection matches = regImg.Matches(ImgStr);
            int i = 0;
            string[] sUrlList = new string[matches.Count];

            // ȡ��ƥ�����б�   
            foreach (Match match in matches)
                sUrlList[i++] = match.Value;
            // match.Groups["imgUrl"].Value;
            return sUrlList;
        }
        #endregion

        #region ͼƬ�Ͷ����Ƶ�ת������
        /// <summary>
        /// ���ܣ�����·����ͼƬת���ɶ�������
        /// ������Ա�����
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private byte[] ImageDatabytes(string FilePath)
        {
            if (!File.Exists(FilePath))
                return null;
            Bitmap myBitmap = new Bitmap(Image.FromFile(FilePath));

            using (MemoryStream curImageStream = new MemoryStream())
            {
                myBitmap.Save(curImageStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                curImageStream.Flush();

                byte[] bmpBytes = curImageStream.ToArray();
                //���ת�ַ����Ļ�
                //string BmpStr = Convert.ToBase64String(bmpBytes);
                return bmpBytes;
            }
        }
        /// <summary>
        /// ���ݶ�����������Ϊbase64�ַ���
        /// </summary>
        /// <param name="bmpBytes"></param>
        /// <returns></returns>
        private string ToBase64Str(byte[] bmpBytes)
        {
            if (bmpBytes == null)
                return null;
            //���ת�ַ����Ļ�
            string BmpStr = Convert.ToBase64String(bmpBytes);
            return BmpStr;
        }
        /// <summary>
        /// ����base64�ַ���,ת��Ϊ��������
        /// </summary>
        /// <param name="bmpBytes"></param>
        /// <returns></returns>
        private byte[] ToBytes(string base64Str)
        {
            //������ַ����Ļ�
            byte[] resultBytes = Convert.FromBase64String(base64Str);
            return resultBytes;
        }
        /// <summary>
        /// ���ܣ����ݶ�����ת����ͼƷ
        /// ������Ա:���
        /// ʱ�䣺
        /// </summary>
        /// <param name="ImageDatas"></param>
        /// <returns></returns>
        private Bitmap GetImage(byte[] ImageDatas)
        {
            try
            {
                //������ַ����Ļ�
                //byte[] resultBytes = Convert.FromBase64String(ImageDatas);

                using (MemoryStream ImageMS = new MemoryStream())
                {
                    ImageMS.Write(ImageDatas, 0, ImageDatas.Length);

                    Bitmap resultBitmap = new Bitmap(ImageMS);
                    return resultBitmap;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
