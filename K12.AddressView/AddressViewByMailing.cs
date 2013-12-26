using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using FISCA.UDT;
using K12.Data;

namespace K12.AddressView
{
    //依畢業年度檢視
    public partial class AddressViewByMailing : NavView
    {

        //排序 預設排序 學生學號


        private AccessHelper _AccessHelper = new AccessHelper();

        //縣市別+鄉鎮市區別
        private Dictionary<string, List<string>> Address_T_Dic = new Dictionary<string, List<string>>();

        //依地址
        private Dictionary<string, Dictionary<string, List<string>>> AddressDic = new Dictionary<string, Dictionary<string, List<string>>>();

        string NoAddress = "無地址";
        string NoAddress2 = "無鄉鎮市區";

        //提供一個預設排序(由北至南)
        List<string> AddressSort = new List<string>();

        public AddressViewByMailing()
        {
            InitializeComponent();

            BudeAddressSort();

            NavText = "依聯絡地址檢視";
            SourceChanged += new EventHandler(NoteView_SourceChanged);
        }

        void NoteView_SourceChanged(object sender, EventArgs e)
        {
            AddressDic.Clear();
            Address_T_Dic.Clear();
            List<AddressObj> addressList = new List<AddressObj>();

            FISCA.Data.QueryHelper queryHelper = new FISCA.Data.QueryHelper();
            string qh = string.Join("','", Source);
            DataTable dt = queryHelper.Select("select id,name,mailing_address from student where id in('" + qh + "')");
            foreach (DataRow row in dt.Rows)
            {
                AddressObj obj = new AddressObj(row);
                addressList.Add(obj);
            }

            foreach (AddressObj address in addressList)
            {
                //縣市
                if (!AddressDic.ContainsKey(address.縣市))
                {
                    AddressDic.Add(address.縣市, new Dictionary<string, List<string>>());
                }

                //鄉鎮市區
                if (!AddressDic[address.縣市].ContainsKey(address.鄉鎮市區))
                {
                    AddressDic[address.縣市].Add(address.鄉鎮市區, new List<string>());
                }

                string pp = address.縣市 + "_" + address.鄉鎮市區;
                if (!Address_T_Dic.ContainsKey(pp))
                {
                    Address_T_Dic.Add(pp, new List<string>());
                }
                Address_T_Dic[pp].Add(address.ref_student_id);
                AddressDic[address.縣市][address.鄉鎮市區].Add(address.ref_student_id);
            }

            AddressDic = SortDic(AddressDic);



            #region 增加Node

            //記錄前一個選擇的名稱
            string jj = "";
            if (advTree1.SelectedNode != null)
            {
                jj = "" + advTree1.SelectedNode.Tag;
            }

            //第一層Node
            advTree1.Nodes.Clear();

            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "依聯絡地址檢視(" + Source.Count() + ")";
            Node1.Tag = "All";

            List<DevComponents.AdvTree.Node> Node2List = new List<DevComponents.AdvTree.Node>();

            //縣市別
            foreach (string each1 in AddressDic.Keys)
            {
                //Count該縣市有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in AddressDic[each1].Keys)
                {
                    ClassStudentCount += AddressDic[each1][each2].Count;
                }

                if (each1 != "") //不是無地址者
                {
                    //增加分類Node
                    DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                    Node2.Text = each1 + "(" + ClassStudentCount + ")";
                    Node2.Tag = each1;
                    Node2List.Add(Node2);
                    List<DevComponents.AdvTree.Node> Node3List = new List<DevComponents.AdvTree.Node>();
                    //鄉鎮市區名稱Node
                    foreach (string each2 in AddressDic[each1].Keys)
                    {
                        if (each2 != "") //不是無鄉鎮市區者
                        {
                            DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                            Node3.Text = each2 + "(" + AddressDic[each1][each2].Count() + ")";
                            Node3.Tag = each1 + "_" + each2;
                            Node3List.Add(Node3);
                        }
                        else
                        {

                        }
                    }

                    //增加無鄉鎮市區Node
                    if (AddressDic[each1].ContainsKey(""))
                    {

                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = NoAddress2 + "(" + AddressDic[each1][""].Count + ")";
                        Node3.Tag = each1 + "_" + "";
                        Node3List.Add(Node3);
                    }
                    Node2.Nodes.AddRange(Node3List.ToArray());
                }
            }

            if (AddressDic.ContainsKey(""))
            {
                //Count該學年度有多少學生
                int ClassStudentCount = 0;
                foreach (string each2 in AddressDic[""].Keys)
                {
                    ClassStudentCount += AddressDic[""][each2].Count;
                }

                //增加無市區Node
                DevComponents.AdvTree.Node Node2 = new DevComponents.AdvTree.Node();
                Node2.Text = NoAddress + "(" + ClassStudentCount + ")";
                Node2.Tag = "";
                Node2List.Add(Node2);
                List<DevComponents.AdvTree.Node> Node3List = new List<DevComponents.AdvTree.Node>();

                //班級名稱Node
                foreach (string each3 in AddressDic[""].Keys)
                {
                    if (each3 != "") //不是無地址者
                    {
                        DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                        Node3.Text = each3 + "(" + AddressDic[""][each3].Count() + ")";
                        Node3.Tag = "_" + each3;
                        Node3List.Add(Node3);
                    }
                }

                if (AddressDic[""].ContainsKey(""))
                {
                    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
                    Node3.Text = NoAddress2 + "(" + AddressDic[""][""].Count() + ")";
                    Node3.Tag = "_";
                    Node3List.Add(Node3);
                }
                Node2.Nodes.AddRange(Node3List.ToArray());
            }
            #endregion

            Node1.Nodes.AddRange(Node2List.ToArray());
            advTree1.Nodes.Add(Node1); //加入

            advTree1.ExpandAll();

            if (jj != "")
            {
                DevComponents.AdvTree.Node FindNode = advTree1.FindNodeByText(jj);
                if (FindNode != null)
                {
                    DevComponents.AdvTree.eTreeAction eT = new DevComponents.AdvTree.eTreeAction();
                    eT = DevComponents.AdvTree.eTreeAction.Mouse;
                    advTree1.SelectNode(FindNode, eT);
                }
            }
        }

        private Dictionary<string, Dictionary<string, List<string>>> SortDic(Dictionary<string, Dictionary<string, List<string>>> _dic)
        {
            Dictionary<string, Dictionary<string, List<string>>> Dic = new Dictionary<string, Dictionary<string, List<string>>>();

            foreach (string each1 in AddressSort)
            {
                if (_dic.ContainsKey(each1))
                {
                    if (!Dic.ContainsKey(each1))
                    {
                        Dic.Add(each1, _dic[each1]);
                    }
                }
            }

            foreach (string each2 in _dic.Keys)
            {
                if (!Dic.ContainsKey(each2))
                {
                    Dic.Add(each2, _dic[each2]);
                }
            }

            return Dic;
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //判斷是否有按Control,Shift
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            //傳入ID

            string NodeClick = "" + e.Node.Tag;
            #region 當使用者是選取標頭(所有學生)
            if (NodeClick == "All")
            {
                SetListPaneSource(Source, SelectedAll, AddToTemp);
            }
            else if (Address_T_Dic.ContainsKey(NodeClick)) //當使用者是選取鄉鎮市區名稱
            {
                //填入
                SetListPaneSource(Address_T_Dic[NodeClick], SelectedAll, AddToTemp);
            }
            else if (AddressDic.ContainsKey(NodeClick))//未選取
            {
                List<string> list = new List<string>();
                foreach (string each in AddressDic[NodeClick].Keys)
                {
                    list.AddRange(AddressDic[NodeClick][each]);
                }
                SetListPaneSource(list, SelectedAll, AddToTemp);
            }
            #endregion

        }

        /// <summary>
        /// 建立預設的排序內容
        /// </summary>
        private void BudeAddressSort()
        {
            AddressSort.Clear();
            AddressSort.Add("臺北市");
            AddressSort.Add("台北市");

            AddressSort.Add("新北市");
            AddressSort.Add("臺北縣");
            AddressSort.Add("台北縣");

            AddressSort.Add("臺中市");
            AddressSort.Add("台中市");

            AddressSort.Add("臺南市");
            AddressSort.Add("台南市");
            AddressSort.Add("高雄市");

            AddressSort.Add("基隆市");
            AddressSort.Add("新竹市");
            AddressSort.Add("嘉義市");

            AddressSort.Add("桃園縣");
            AddressSort.Add("新竹縣");
            AddressSort.Add("苗栗縣");

            AddressSort.Add("彰化縣");
            AddressSort.Add("南投縣");
            AddressSort.Add("雲林縣");
            AddressSort.Add("嘉義縣");
            AddressSort.Add("屏東縣");

            AddressSort.Add("宜蘭縣");
            AddressSort.Add("花蓮縣");
            AddressSort.Add("臺東縣");
            AddressSort.Add("台東縣");

            AddressSort.Add("澎湖縣");
            AddressSort.Add("金門縣");
            AddressSort.Add("馬祖縣");
        }
    }
}
