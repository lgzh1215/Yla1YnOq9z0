using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Fiddler;

namespace KanColleTool {

    class RequestBuilder {

        public string Token { get; private set; }
        
        public bool OnInvoke { get; private set; }

        private Session oS;

        private Queue<KCRequest> tasks;

        public RequestBuilder (Fiddler.Session oS) {
            Token = HttpUtility.ParseQueryString(oS.GetRequestBodyAsString())["api_token"];
            this.oS = oS;
            this.tasks = new Queue<KCRequest>();
        }

        public void MissionReturn (int deckId) {
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

        public void FleetCharge (int fleet, ICollection<string> ship) {
            if (!(ship.Count > 0)) {
                return;
            }
            DoCharge(3, ship);
            DoShip2();
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
                                HTTPRequestHeaders header = (HTTPRequestHeaders) oS.oRequest.headers.Clone();
                                header.RequestPath = "/kcsapi/" + req.Path;
                                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(req.Body);
                                Session sess = new Session(header, postBytes);
                                sess.utilSetRequestBody(req.Body);
                                FiddlerApplication.oProxy.SendRequestAndWait(sess.oRequest.headers, sess.requestBodyBytes, null, null);
                                Thread.Sleep(req.Sleep);
                            }
                        } catch (Exception ex) {
                            Debug.Print(ex.Message);
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

        public void DoCharge (int kind, ICollection<string> ship) {
            string body = "api%5Fkind=" + kind + "&api%5Fid%5Fitems=" + String.Join("%2C", ship) + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_hokyu/charge", body, 100);
            tasks.Enqueue(req);
        }

        public void DoUseItem () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/useitem", body, 100);
            tasks.Enqueue(req);
        }

        public void DoResult (int deckId) {
            string body = "api%5Fdeck%5Fid=" + deckId + "&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_req_mission/result", body, 1000);
            tasks.Enqueue(req);
        }

        public void DoActionlog () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/actionlog", body, 100);
            tasks.Enqueue(req);
        }

        public void DoLoginCheck () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_auth_member/logincheck", body, 100);
            tasks.Enqueue(req);
        }

        public void DoMaterial () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/material", body, 100);
            tasks.Enqueue(req);
        }

        public void DoDeckPort () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/deck_port", body, 100);
            tasks.Enqueue(req);
        }

        public void DoNDock () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/ndock", body, 100);
            tasks.Enqueue(req);
        }

        public void DoShip2 () {
            string body = "api%5Fsort%5Forder=2&api%5Fsort%5Fkey=2&api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/ship2", body, 500);
            tasks.Enqueue(req);
        }

        public void DoBasic () {
            string body = "api%5Fverno=1&api%5Ftoken=" + Token;
            KCRequest req = new KCRequest("api_get_member/basic", body, 100);
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
