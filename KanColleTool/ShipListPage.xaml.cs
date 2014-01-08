using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KanColleTool {
    /// <summary>
    /// ShipListPage.xaml 的互動邏輯
    /// </summary>
    public partial class ShipListPage : Page {

        Thread UIThread;

        List<MenuItem> Panel = new List<MenuItem>();

        IEnumerable<JToken> SubMenuItems;

        static List<int> dcFigWeapon = new List<int>() { 5453, 5455, 5898, 242, 1771 };
        static List<int> dcAtkWeapon = new List<int>() { 5453, 5455, 5898, 242, 4616 };
        static List<int> dcAssWeapon = new List<int>() { 5129, 1174, 4538, 5689, 4413 };

        static List<int> bcFigWeapon = new List<int>() { 1771, 1947, 2374, 5453, 5455 };
        static List<int> bcAtkWeapon = new List<int>() { 5717, 1046, 6508, 5453, 5455 };
        static List<int> bcAssWeapon = new List<int>() { 5717, 6145, 5129, 1771, 5455 };

        static List<int> cvlFigWeapon = new List<int>() { 3987, 1554, 1108, 1683, 1771 };
        static List<int> cvlAtkWeapon = new List<int>() { 1046, 6508, 1082, 1683, 242 };
        static List<int> cvlAssWeapon = new List<int>() { 5717, 6145, 6038, 1683, 5129 };

        static List<int> cvFigWeapon = new List<int>() { 3987, 1554, 1108, 6038, 1771 };
        static List<int> cvAtkWeapon = new List<int>() { 1046, 6508, 1082, 1089, 242 };

        static List<int> ssAtkWeapon = new List<int>() { 3650, 4616, 210, 3709, 242 };

        static List<int> emptyWeapon = new List<int>() { -1, -1, -1, -1, -1 };

        public ShipListPage () {
            UIThread = Thread.CurrentThread;
            InitializeComponent();
            InitializeMenuPanel();
            KCODt.Instance.ItemDataChanged += new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        ~ShipListPage () {
            KCODt.Instance.ItemDataChanged -= new KCODt.ItemDataChangedEventHandler(KCODt_ItemDataChanged);
        }

        private void InitializeMenuPanel () {
            Panel.Add(eq0);
            Panel.Add(eq1);
            Panel.Add(eq2);
            Panel.Add(eq3);
            Panel.Add(eq4);

            dcFig.Header = new SlotTemplate("巡洋-對空", dcFigWeapon, SlotTemplate.OrderType.Normal);
            dcAtk.Header = new SlotTemplate("巡洋-火力", dcAtkWeapon, SlotTemplate.OrderType.Normal);
            dcAss.Header = new SlotTemplate("巡洋-對潛", dcAssWeapon, SlotTemplate.OrderType.Normal);

            bcFig.Header = new SlotTemplate("航戰-對空", bcFigWeapon, SlotTemplate.OrderType.Normal);
            bcAtk.Header = new SlotTemplate("航戰-火力", bcAtkWeapon, SlotTemplate.OrderType.Size);
            bcAss.Header = new SlotTemplate("航戰-對潛", bcAssWeapon, SlotTemplate.OrderType.Size);

            cvlFig.Header = new SlotTemplate("輕母-對空", cvlFigWeapon, SlotTemplate.OrderType.Size);
            cvlAtk.Header = new SlotTemplate("輕母-火力", cvlAtkWeapon, SlotTemplate.OrderType.Size);
            cvlAss.Header = new SlotTemplate("輕母-對潛", cvlAssWeapon, SlotTemplate.OrderType.Size);

            cvFig.Header = new SlotTemplate("航母-對空", cvFigWeapon, SlotTemplate.OrderType.Normal);
            cvAtk.Header = new SlotTemplate("航母-火力", cvAtkWeapon, SlotTemplate.OrderType.Size);

            ssAtk.Header = new SlotTemplate("潛-火力", ssAtkWeapon, SlotTemplate.OrderType.Normal);

            empty.Header = new SlotTemplate("解除武裝", emptyWeapon, SlotTemplate.OrderType.AlwaysOne);
        }

        private void reflash () {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    ShipGrid.ItemsSource = null;
                    if (KCODt.Instance.ShipData != null && KCODt.Instance.ShipSpec != null) {
                        var qm = (from spec in KCODt.Instance.ShipSpec
                                  from ship in KCODt.Instance.ShipData
                                  from stype in KCODt.Instance.ShipType
                                  where spec["api_id"].ToString() == ship["api_ship_id"].ToString()
                                  && spec["api_stype"].ToString() == stype["api_id"].ToString()
                                  select JToken.FromObject(new ShipDetail(spec, ship, stype)))
                                 .OrderBy(x => x["FleetInfo"].ToString());
                        ShipGrid.ItemsSource = qm;
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void queryItems () {
            var qm = (from spec in KCODt.Instance.ItemSpec
                      from item in KCODt.Instance.ItemData
                      where spec["api_id"].ToString() == item["api_slotitem_id"].ToString()
                      select JToken.FromObject(new ItemDetail(spec, item)))
                     .OrderByDescending(x => x["Spec"]["api_sortno"].ToString());
            SubMenuItems = qm;
        }

        #region even handler
        private void KCODt_ShipDataChanged (object sender, DataChangedEventArgs e) {
            reflash();
        }

        private void KCODt_ItemDataChanged (object sender, DataChangedEventArgs e) {
            Dispatcher.FromThread(UIThread).Invoke((MainWindow.Invoker) delegate {
                try {
                    queryItems();
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }, null);
        }

        private void HenseiItem_Click (object sender, RoutedEventArgs e) {
            try {
                MenuItem muPos = sender as MenuItem;
                MenuItem muFle = muPos.Parent as MenuItem;
                MenuItem muExe = muFle.Parent as MenuItem;
                ContextMenu contextMenu = muExe.Parent as ContextMenu;
                DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
                JToken detail = dataGrid.CurrentItem as JToken;
                int shipId = int.Parse(detail["Ship"]["api_id"].ToString());
                int shipIdx = int.Parse(muPos.Uid);
                int fleet = int.Parse(muFle.Uid);
                RequestBuilder.Instance.HenseiChange(shipId, shipIdx, fleet);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void ShipGrid_ContextMenuOpening (object sender, ContextMenuEventArgs e) {
            try {
                DataGrid dataGrid = sender as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                int shipId = int.Parse(shipDetail["Ship"]["api_id"].ToString());
                for (int i = 0; i < 5; i++) {
                    string onslot = shipDetail["Ship"]["api_onslot"][i].ToString();
                    string eqid = shipDetail["Ship"]["api_slot"][i].ToString();
                    string eqName = "無";
                    if (eqid != "-1") {
                        if (KCODt.Instance.ItemDataMap.ContainsKey(eqid)) {
                            eqName = KCODt.Instance.ItemDataMap[eqid]["api_name"].ToString();
                        } else {
                            eqName = "未讀入";
                        }
                        Panel[i].Icon = String.Format("{0}", onslot);
                        Panel[i].Header = String.Format("{0} \t {1}", eqid, eqName);
                    } else {
                        Panel[i].Header = String.Format("{0}", eqName);
                    }
                    Panel[i].ItemsSource = null;
                    Panel[i].ItemsSource = SubMenuItems;
                }
                miFl1.Header = KCODt.Instance.DeckData[0]["api_name"].ToString();
                miFl2.Header = KCODt.Instance.DeckData[1]["api_name"].ToString();
                miFl3.Header = KCODt.Instance.DeckData[2]["api_name"].ToString();
                miFl4.Header = KCODt.Instance.DeckData[3]["api_name"].ToString();
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void SlotSet_Click (object sender, RoutedEventArgs e) {
            try {
                MenuItem item = e.OriginalSource as MenuItem;
                JToken itemDetail = item.Header as JToken;
                int itemId = int.Parse(itemDetail["Item"]["api_id"].ToString());

                MenuItem muPos = sender as MenuItem;
                ContextMenu contextMenu = muPos.Parent as ContextMenu;
                DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
                JToken shipDetail = dataGrid.CurrentItem as JToken;
                int shipId = int.Parse(shipDetail["Ship"]["api_id"].ToString());
                int slotIdx = int.Parse(muPos.Uid);
                RequestBuilder.Instance.SlotSet(slotIdx, itemId, shipId);
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void Destroy_click (object sender, RoutedEventArgs e) {
            try {
                HashSet<string> destroyShips = new HashSet<string>();
                foreach (var item in ShipGrid.SelectedItems) {
                    JToken shipDetail = item as JToken;
                    destroyShips.Add(shipDetail["Ship"]["api_id"].ToString());
                }
                MessageBoxResult messageBoxResult = MessageBox.Show("解體確認", "Delete Confirmation", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) {
                    RequestBuilder.Instance.DestroyShip(destroyShips);
                }
            } catch (Exception ex) {
                Debug.Print(ex.ToString());
            }
        }

        private void Page_Loaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ShipDataChanged += new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged += new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
            if (KCODt.Instance.ShipData == null) {
                RequestBuilder.Instance.ReLoadShip3();
            } else {
                reflash();
            }
        }

        private void Page_Unloaded (object sender, RoutedEventArgs e) {
            KCODt.Instance.ShipDataChanged -= new KCODt.ShipDataChangedEventHandler(KCODt_ShipDataChanged);
            KCODt.Instance.DeckDataChanged -= new KCODt.DeckDataChangedEventHandler(KCODt_ShipDataChanged);
        }

        private void ShipGrid_KeyDown (object sender, KeyEventArgs e) {
            if (e.Key == Key.F5) {
                RequestBuilder.Instance.ReLoadShip3();
            }
        }

        private void SlotTemplate_click (object sender, RoutedEventArgs e) {
            changeCVSlot(sender);
        }

        private void changeCVSlot (object sender) {
            MenuItem muPos = sender as MenuItem;
            MenuItem muFle = muPos.Parent as MenuItem;
            ContextMenu contextMenu = muFle.Parent as ContextMenu;
            DataGrid dataGrid = contextMenu.PlacementTarget as DataGrid;
            JToken shipDetail = dataGrid.CurrentItem as JToken;
            SlotTemplate st = muPos.Header as SlotTemplate;
            List<int> slots = new List<int>();
            int shipId = int.Parse(shipDetail["Ship"]["api_id"].ToString());
            switch (st.Order) {
                case SlotTemplate.OrderType.Normal:
                    slots = st.Weapon;
                    RequestBuilder.Instance.SlotSet(slots, shipId);
                    break;
                case SlotTemplate.OrderType.Size:
                    List<KeyValuePair<int, int>> slotList = getSlotBySize(shipDetail, st.Weapon);
                    for (int i = 0; i < 5; i++) {
                        slots.Add(slotList[i].Value);
                    }
                    RequestBuilder.Instance.SlotSet(slots, shipId);
                    break;
                case SlotTemplate.OrderType.AlwaysOne:
                    slots = st.Weapon;
                    RequestBuilder.Instance.EmptySlot(slots, shipId);
                    break;
                default:
                    break;
            }
        }

        private List<KeyValuePair<int, int>> getSlotBySize (JToken shipDetail, List<int> eqList) {
            List<KeyValuePair<int, int>> slotList = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < 5; i++) {
                int onslot = int.Parse(shipDetail["Ship"]["api_onslot"][i].ToString());
                slotList.Add(new KeyValuePair<int, int>(i, onslot));
            }
            slotList.Sort((firstPair, nextPair) => {
                return firstPair.Value.CompareTo(nextPair.Value);
            });
            for (int i = 4; i >= 0; i--) {
                int key = slotList[i].Key;
                int value = eqList[4 - i];
                slotList[i] = new KeyValuePair<int, int>(key, value);
            }
            slotList.Sort((firstPair, nextPair) => {
                return firstPair.Key.CompareTo(nextPair.Key);
            });
            return slotList;
        }
        #endregion
    }

    class SlotTemplate {

        public enum OrderType { Normal, Size, AlwaysOne }

        public List<int> Weapon { get; private set; }

        public string Name { get; private set; }

        public OrderType Order { get; private set; }

        public SlotTemplate (string name, List<int> weapon, OrderType order) {
            Name = name;
            Weapon = weapon;
            Order = order;
        }

        public override string ToString () {
            return Name;
        }
    }
}
