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
    public partial class htmlS : UCBase
    {
        public htmlS()
        {
            dataUpdate = 0;
            InitializeComponent();
            InitializeControls();
        }
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
             
            }
        }
       

        #region ����

        /// <summary>
        /// �ؼ���ʼ��
        /// </summary>
        private void InitializeControls()
        {
            BeginUpdate();

          

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

           
                mshtml.IHTMLDocument2 document = (mshtml.IHTMLDocument2)webBrowserBody.Document.DomDocument;
                EndUpdate();
              
        }
        public void updating() {
       
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

       

        private void toolStripButtonPicture_Click(object sender, EventArgs e)
        {
            if (Updating)
            {
                return;
            }


  
            webBrowserBody.Document.ExecCommand("InsertImage", true, null);
        
            RefreshToolBar();
        }

     
        public string GetHtmlValue()
        {
            return Html.Substring(Html.LastIndexOf("<BODY>") + 6).Replace("</BODY>", "").Replace("</HTML>", "");
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
        public string  HandlData()
        {
            string content = GetHtmlValue();
            string strData="";
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

                        strData = "";
                        continue;
                    }
                    else if (ImageDatas.Length <= 0)
                    {
                        strData ="";
                        continue;
                    }
                    else
                    {
                        strData = "data:image/jgp;base64," + DataBytes;
                     }

                }
            }
            return strData;
        }
        public string HandlHtml()
        {
            string content = GetHtmlValue();
            string[] imgSrcs = GetImageUrlList(content);
            string[] Img = GetImageList(content);


            if (imgSrcs.Length > 0)
            {
                for (int i = 0; i < imgSrcs.Length; i++)
                {
                    string str = imgSrcs[i];
                    if (str.Contains("data:image"))
                    {
                        continue;
                    }
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
