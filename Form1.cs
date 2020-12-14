using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Test
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 调用动态链接库入口文件名：NISEC_SKSC.dll
        /// </summary>
        /// <param name="pszPost">pszPost输入XML信息</param>
        /// <param name="pszRecv">pszRecv返回XML信息</param>
         //[DllImport("NISEC_SKSC.dll",EntryPoint = "PostAndRecvEx", CharSet = CharSet.Auto, CallingConvention=CallingConvention.Winapi)]
         [DllImport("NISEC_SKSC.dll")]
        public static extern void PostAndRecvEx(ref IntPtr pszPost, out string pszRecv);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string input = GetXML();
            byte[] send = System.Text.Encoding.GetEncoding("GBK").GetBytes(input + "\0");
            IntPtr szReaderVersion = Marshal.StringToBSTR(input);
            //IntPtr szReaderVersion = Marshal.StringToCoTaskMemAuto(input);


            GCHandle hObject = GCHandle.Alloc(send, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();
            byte[] result = new byte[1000000];
            //PostAndRecvEx(ref szReaderVersion, out result);//


            string ret = "";
            PostAndRecvEx(ref szReaderVersion, out ret);//

            int size = 100000;
            byte[] managedArray = new byte[size];
            IntPtr szAPIVersion = Marshal.AllocHGlobal(1000000);
            Marshal.Copy(szAPIVersion, managedArray, 0, size);


            //string receivedata = (string)Marshal.PtrToStringAuto(szAPIVersion);
            string a = System.Text.Encoding.GetEncoding("GBK").GetString(managedArray);

            //string ret = Marshal.PtrToStringAuto(szAPIVersion);
            //string ret=Request(GetXML());//HTTP方式
            //MessageBox.Show(string.Format("{0}\n\n{1}", GetXML(), ret));
        }

        private string Request(string postDataStr)
        {
            Uri myuri = new Uri("http://10.8.8.211:8081/SKServer/SKDo");
            var request = WebRequest.Create(myuri) as HttpWebRequest;
            request.Method = "POST";　　//post
            request.ContentType = "application/json";
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.Default);
            myStreamWriter.Write(postDataStr);
            myStreamWriter.Close();
            var Response = request.GetResponse() as HttpWebResponse;
            Stream myResponseStream = Response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.Default);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        private string GetXML()
        {
            string result = string.Empty;
            string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "1.txt");
                //c#文件流读文件 
                using (FileStream fsRead = new FileStream(path, FileMode.Open))
                {
                    int fsLen = (int)fsRead.Length;
                    byte[] heByte = new byte[fsLen];
                    int r = fsRead.Read(heByte, 0, heByte.Length);
                    result = System.Text.Encoding.UTF8.GetString(heByte);
                }

            return result;
        }
    }
}
