using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;

namespace K12.AddressView
{
    public class AddressObj
    {
        public string ref_student_id { get; set; }
        public string StudentName { get; set; }
        public XmlElement xml { get; set; }
        public string 縣市 { get; set; }
        public string 鄉鎮市區 { get; set; }
        //public string 性別 { get; set; }

        public AddressObj(DataRow row)
        {
            ref_student_id = "" + row[0];
            StudentName = "" + row[1];
            if (!string.IsNullOrEmpty("" + row[2]))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("" + row[2]);
                xml = xmlDoc.DocumentElement;
                if (xml.SelectSingleNode("Address") != null)
                {
                    if (xml.SelectSingleNode("Address/County") != null)
                    {
                        縣市 = xml.SelectSingleNode("Address/County").InnerText;
                    }
                    else
                    {
                        縣市 = "";
                    }
                    if (xml.SelectSingleNode("Address/Town") != null)
                    {
                        鄉鎮市區 = xml.SelectSingleNode("Address/Town").InnerText;
                    }
                    else
                    {
                        鄉鎮市區 = "";
                    }
                }
                else
                {
                    縣市 = "";
                    鄉鎮市區 = "";
                }
            }
            else
            {
                縣市 = "";
                鄉鎮市區 = "";
            }

            //if ("" + row[3] == "1")
            //{
            //    性別 = "男";
            //}
            //else if ("" + row[3] == "0")
            //{
            //    性別 = "女";
            //}
            //else
            //{
            //    性別 = "未分性別";
            //}
        }

    }
}
