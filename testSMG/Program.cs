using System;

namespace testSMG {
    class Program {
        enum startGroup {
            Manual,
            Immediately,
            schedule
        };

        static void Main (string[] args) {
            smgs();
        }

        static void smgs () {
            try {
                SMGCatcher.IECatcher smg = new SMGCatcher.IECatcher();
                String mvid = "050412_01";
                String space = @"D:\Downloads\youtube\";
                String text = System.IO.File.ReadAllText(space + mvid + @"\" + mvid + ".txt");
                String xxx = "s09840Gs75mMGM267673SSss019sS7G5mM19s3m875S042G70g17sGg18sMm2m18S54471M6m235S00394G518m8799M714Ms167g434136139mggM6s9S81800Mm6m2011m0436S0532S0M5MSM073Mg2M568891470Mgm2G391MMmm21G95531s3S8M3206023777018gSM56s3444GM18352sG3406480019g62SG6m6sg8971gsGM81s23284s5ms9915s67sg59747sg37S8S62Sg31M61032gM48896893m0s8";
                text = text.Replace(xxx, "");

                String referer = "";
                String commenet = mvid;
                int divCount = 1;
                bool noComfirm = false;
                bool useIECookie = false;

                smg.DownloadSelection(text, referer, commenet, divCount, noComfirm, (int) startGroup.Immediately, useIECookie, space + mvid);
            } catch (Exception ex) {
                Console.WriteLine("Unexpected COM exception: " + ex.Message);
            }
        }
    }
}
