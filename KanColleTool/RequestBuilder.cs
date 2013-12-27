using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Fiddler;

namespace KanColleTool {

    public class RequestBuilder {

        static public string Token { get; private set; }

        static public bool IsInvoke { get; private set; }

        static public Session Session { get; private set; }

        static public RequestBuilder Instance {
            get {
                if (instance == null) {
                    if (Session != null) {
                        instance = new RequestBuilder();
                    } else {
                        throw new SystemException("Session is null");
                    }
                }
                return instance;
            }
            private set {
                instance = null;
            }
        }

        static private RequestBuilder instance;

        static private Queue<KCRequest> tasks;

        static public RequestBuilder Initialize (Session oS) {
            Session = oS;
            instance = null;
            return Instance;
        }

        private RequestBuilder () {
            Token = HttpUtility.ParseQueryString(Session.GetRequestBodyAsString())["api_token"];
            IsInvoke = false;
            tasks = new Queue<KCRequest>();
        }

        private void MissionReturn (int deckId) {
            DoActionlog();
            DoLoginCheck();
            DoMaterial();
            DoDeckPort();
            DoNDock();
            DoShip2();
            DoResult(deckId);
            DoBasic();
            DoDeckPort();
            DoBasic();
            DoShip2();
            DoMaterial();
            DoUseItem();
            Invoke();
        }

        public void EnterNDock () {
            DoNDock();
            DoShip2();
            Invoke();
        }

        public void StartMission (int deckId, int missionId) {
            DoStartMission(deckId, missionId);
            DoShip2();
            Invoke();
        }

        public void FleetCharge (int fleet, ICollection<string> ship) {
            if (!(ship.Count > 0)) {
                return;
            }
            DoCharge(3, ship);
            DoShip3();
            Invoke();
        }

        public void HenseiChange (int shipId, int shipIdx, int fleet) {
            DoChange(shipId, shipIdx, fleet);
            DoDeck();
            Invoke();
        }

        public void SlotSet (int slotIdx, int itemId, int shipId) {
            DoSlotSet(slotIdx, itemId, shipId);
            Invoke();
        }

        public void ReLoadShip3 () {
            DoShip3();
            Invoke();
        }

        public void ReLoadShipSpec () {
            DoMasterShip();
            Invoke();
        }

        public void ReLoadSlotItem () {
            DoSlotItem();
            Invoke();
        }

        public void DestroyItem (ICollection<string> items) {
            DoDestroyItem2(items);
            DoSlotItem();
            Invoke();
        }

        public void DestroyShip (ICollection<string> ships) {
            foreach (var ship in ships) {
                DoDestroyShip(ship);
            }
            DoShip3();
            Invoke();
        }

        public void NDockStart (int shipId, int ndockId, int speed) {
            DoNDockStart(shipId, ndockId, speed);
            DoNDock();
            Invoke();
        }

        public void QuestList (int page) {
            string body = String.Format("api%5Fpage%5Fno={0}&api%5Fverno=1&api%5Ftoken={1}", page, Token);
            KCRequest req = new KCRequest("api_get_member/questlist", body, 200);
            tasks.Enqueue(req);
            Invoke();
        }

        public void QuestStart (int id) {
            string body = String.Format("api%5Fquest%5Fid={0}&api%5Fverno=1&api%5Ftoken={1}", id, Token);
            KCRequest req = new KCRequest("api_req_quest/start", body, 500);
            tasks.Enqueue(req);
            Invoke();
        }

        public void QuestStop (int id) {
            string body = String.Format("api%5Fquest%5Fid={0}&api%5Fverno=1&api%5Ftoken={1}", id, Token);
            KCRequest req = new KCRequest("api_req_quest/stop", body, 500);
            tasks.Enqueue(req);
            Invoke();
        }

        public void QuestClear (int id) {
            string body = String.Format("api%5Fquest%5Fid={0}&api%5Fverno=1&api%5Ftoken={1}", id, Token);
            KCRequest req = new KCRequest("api_req_quest/clearitemget", body, 1000);
            tasks.Enqueue(req);
            Invoke();
        }

        private void bw_DoWork (object sender, DoWorkEventArgs e) {
            IsInvoke = true;
            lock (tasks) {
                try {
                    Queue<KCRequest> taskBlock = new Queue<KCRequest>();
                    for (int i = 0; i <= tasks.Count; i++) {
                        taskBlock.Enqueue(tasks.Dequeue());
                    }
                    while (taskBlock.Count > 0) {
                        KCRequest req = taskBlock.Dequeue();
                        HTTPRequestHeaders header = (HTTPRequestHeaders) Session.oRequest.headers.Clone();
                        header.RequestPath = "/kcsapi/" + req.Path;
                        byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(req.Body);
                        Session sess = new Session(header, postBytes);
                        sess.utilSetRequestBody(req.Body);
                        FiddlerApplication.oProxy.SendRequestAndWait(sess.oRequest.headers, sess.requestBodyBytes, null, null);
                        Thread.Sleep(req.Sleep);
                    }
                } catch (Exception ex) {
                    Debug.Print(ex.ToString());
                }
            }
        }

        void bw_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e) {
            IsInvoke = false;
            Debug.Print("RunWorkerCompleted");
            if (tasks.Count > 0) {
                Debug.Print("But queue still has job to do...");
                BackgroundWorker bw = sender as BackgroundWorker;
                bw.RunWorkerAsync();
            }
        }

        private void Invoke () {
            if (!IsInvoke) {
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                bw.RunWorkerAsync();
            }
        }

        #region 各種post

        private void DoCharge (int kind, ICollection<string> ships) {
            string body = "api%5Fkind=" + kind + "&api%5Fid%5Fitems=" + String.Join("%2C", ships) + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_hokyu/charge", body, 100);
            tasks.Enqueue(req);
        }

        private void DoUseItem () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/useitem", body, 100);
            tasks.Enqueue(req);
        }

        private void DoResult (int deckId) {
            string body = "api%5Fdeck%5Fid=" + deckId + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_mission/result", body, 1000);
            tasks.Enqueue(req);
        }

        private void DoActionlog () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/actionlog", body, 100);
            tasks.Enqueue(req);
        }

        private void DoLoginCheck () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_auth_member/logincheck", body, 100);
            tasks.Enqueue(req);
        }

        private void DoMaterial () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/material", body, 100);
            tasks.Enqueue(req);
        }

        private void DoDeckPort () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/deck_port", body, 100);
            tasks.Enqueue(req);
        }

        private void DoDeck () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/deck", body, 100);
            tasks.Enqueue(req);
        }

        private void DoNDock () {
            string body = String.Format("api%5Fverno=1&api%5Ftoken={0}", Token);
            KCRequest req = new KCRequest("api_get_member/ndock", body, 100);
            tasks.Enqueue(req);
        }

        private void DoShip2 () {
            string body = "api%5Fsort%5Forder=2&api%5Fsort%5Fkey=2&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/ship2", body, 500);
            tasks.Enqueue(req);
        }

        private void DoShip3 () {
            string body = String.Format("api%5Fsort%5Forder=2&api%5Fsort%5Fkey=1&api%5Fverno=1&api%5Ftoken={0}", Token);
            KCRequest req = new KCRequest("api_get_member/ship3", body, 500);
            tasks.Enqueue(req);
        }

        private void DoBasic () {
            string body = String.Format("api%5Fverno=1&api%5Ftoken={0}", Token);
            KCRequest req = new KCRequest("api_get_member/basic", body, 100);
            tasks.Enqueue(req);
        }

        private void DoStartMission (int deckId, int missionId) {
            string body = String.Format("api%5Fdeck%5Fid={0}&api%5Fmission%5Fid={1}&api%5Fverno=1&api%5Ftoken={2}", deckId, missionId, Token);
            KCRequest req = new KCRequest("api_req_mission/start", body, 100);
            tasks.Enqueue(req);
        }

        private void DoChange (int shipId, int shipIdx, int fleet) {
            string body = "api%5Fship%5Fid=" + shipId + "&api%5Fship%5Fidx=" + shipIdx + "&api%5Fid=" + fleet + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_hensei/change", body, 100);
            tasks.Enqueue(req);
        }

        private void DoSlotSet (int slotIdx, int itemId, int shipId) {
            string body = "api%5Fslot%5Fidx=" + slotIdx + "&api%5Fitem%5Fid=" + itemId + "&api%5Fid=" + shipId + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_kaisou/slotset", body, 100);
            tasks.Enqueue(req);
        }

        private void DoSlotItem () {
            string body = String.Format("api%5Fverno=1&api%5Ftoken={0}", Token);
            KCRequest req = new KCRequest("api_get_member/slotitem", body, 300);
            tasks.Enqueue(req);
        }

        private void DoNDockStart (int shipId, int ndockId, int speed) {
            string body = String.Format("api%5Fship%5Fid={0}&api%5Fndock%5Fid={1}&api%5Fhighspeed={2}&api%5Fverno=1&api%5Ftoken={3}", shipId, ndockId, speed, Token);
            KCRequest req = new KCRequest("api_req_nyukyo/start", body, 1000);
            tasks.Enqueue(req);
        }

        private void DoDestroyItem2 (ICollection<string> items) {
            string body = String.Format("api%5Fslotitem%5Fids={0}&api%5Fverno=1&api%5Ftoken={1}", String.Join("%2C", items), Token);
            KCRequest req = new KCRequest("api_req_kousyou/destroyitem2", body, 500);
            tasks.Enqueue(req);
        }

        private void DoDestroyShip (string ship) {
            string body = String.Format("api%5Fship%5Fid={0}&api%5Fverno=1&api%5Ftoken={1}", ship, Token);
            KCRequest req = new KCRequest("api_req_kousyou/destroyship", body, 500);
            tasks.Enqueue(req);
        }

        private void DoMasterShip () {
            string body = String.Format("api%5Fverno=1&api%5Ftoken={0}", Token);
            KCRequest req = new KCRequest("api_get_master/ship", body, 500);
            tasks.Enqueue(req);
        }
        #endregion

    }

    class KCRequest {
        public KCRequest (string path, string body, int sleep) {
            Path = path;
            Body = body;
            Sleep = sleep;
        }
        public string Path { get; set; }
        public string Body { get; set; }
        public int Sleep { get; set; }
    }
}
