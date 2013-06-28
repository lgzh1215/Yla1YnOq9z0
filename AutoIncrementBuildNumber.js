var al = WScript.Arguments.length;
if (al < 1) {
	WScript.StdErr.WriteLine ("Error: Missing input file name.");
	WScript.Quit(-1);
}
var n = WScript.Arguments(0); // file name
var c = al > 1 ? WScript.Arguments(1) : ""; // config name

var sh = new ActiveXObject("Shell.Application");
var f = sh.NameSpace(n.replace(/\\[^\\]+$/g, "")).ParseName(n.replace(/.+?\\([^\\]+)$/g, "$1"));
var ft = f.ModifyDate; // Keep the original modification date/time of the file

var s = new ActiveXObject("ADODB.Stream");
	s.Open();
	s.Type = 2;
	s.CharSet = "UTF-8"; // Make SURE that the encoding is correct, otherwise damage may occur
	s.LoadFromFile (n);
var t = s.ReadText ();
var r = /\[assembly:\s*(System\.Reflection\.)?AssemblyVersion(?:Attribute)?\s*\(\s*"(\d+\.\d+\.)(\d+)\.(\d+)"\s*\)\s*\]/g;
var a = r.exec(t);
if (a != null && a.length > 1) {
	var ns = a[1]; // System.Reflection.
	var m = a[2]; // Major.Minor.
	var b = parseInt(a[3]); // Build
	var rv = parseInt(a[4]); // Revision
	rv++;
	var v = "[assembly: "+ns+"AssemblyVersion (\""+m+b+"."+rv+"\")]";
	WScript.StdOut.WriteLine ("Assembly version changed: " + a[0] + "->" + v);
	t = t.replace(r, v);
}
else {
	WScript.StdErr.WriteLine ("Error: AssemblyVersion not found.");
	WScript.Quit(-1);
}
 
// further processing of the file is possible here
var r2 = /\[assembly:\s*(System\.Reflection\.)?AssemblyFileVersion(?:Attribute)?\s*\(\s*"(\d+\.\d+\.)(\d+)\.(\d+)"\s*\)\s*\]/g;
var a2 = r2.exec(t);
if (a2 != null && a2.length > 1) {
	var ns2 = a2[1]; // System.Reflection.
	var m2 = a2[2]; // Major.Minor.
	var b2 = parseInt(a2[3]); // Build
	var rv2 = parseInt(a2[4]); // Revision
	rv2++;
	var v2 = "[assembly: "+ns2+"AssemblyFileVersion (\""+m2+b2+"."+rv2+"\")]";
	WScript.StdOut.WriteLine ("AssemblyFile version changed: " + a2[0] + "->" + v2);
	t = t.replace(r2, v2);
}
else {
	WScript.StdErr.WriteLine ("Error: AssemblyFileVersion not found.");
	WScript.Quit(-1);
}
// save the result
s.Position = 0;
s.WriteText (t);
s.SetEOS ();
s.SaveToFile (n, 2);
s.Close ();
 
f.ModifyDate = ft; // Reset the modification date/time to prevent unnecessary rebuild afterwards