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

        static public bool OnInvoke { get; private set; }

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
            OnInvoke = false;
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
            DoUseItem();
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
            DoShip2();
            Invoke();
        }

        public void HenseiChange (int shipId, int shipIdx, int fleet) {
            DoChange(shipId, shipIdx, fleet);
            DoShip2();
            Invoke();
        }

        public void SlotSet (int slotIdx, int itemId, int shipId) {
            DoSlotSet(slotIdx, itemId, shipId);
            //DoShip2();
            Invoke();
        }

        public void QuestList (int page) {
            string body = String.Format("api%5Fpage%5Fno={0}&api%5Fverno=1&api%5Ftoken={1}", page, Token);
            KCRequest req = new KCRequest("api_get_member/questlist", body, 100);
            tasks.Enqueue(req);
            Invoke();
        }

        private void Invoke () {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(
                delegate(object o, DoWorkEventArgs args) {
                    lock (tasks) {
                        OnInvoke = true;
                        try {
                            while (tasks.Count > 0) {
                                KCRequest req = tasks.Dequeue();
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
                });
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                delegate(object o, RunWorkerCompletedEventArgs args) {
                    OnInvoke = false;
                    Debug.Print("RunWorkerCompleted");
                });
            bw.RunWorkerAsync();
        }

        #region 各種post

        private void DoCharge (int kind, ICollection<string> ship) {
            string body = "api%5Fkind=" + kind + "&api%5Fid%5Fitems=" + String.Join("%2C", ship) + "&api%5Fverno=1&api%5Ftoken=" + Token;
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
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/ndock", body, 100);
            tasks.Enqueue(req);
        }

        private void DoShip2 () {
            string body = "api%5Fsort%5Forder=2&api%5Fsort%5Fkey=2&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/ship2", body, 500);
            tasks.Enqueue(req);
        }

        private void DoBasic () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/basic", body, 100);
            tasks.Enqueue(req);
        }

        private void DoStartMission (int deckId, int missionId) {
            string body = "api%5Fdeck%5Fid=" + deckId + "&api%5Fmission%5Fid=" + missionId + "&api%5Fverno=1&api%5Ftoken=" + Token;
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
