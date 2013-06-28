using System;
using System.Xml.Serialization;
using System.Collections.Generic;


namespace Launcher.AppUpdater {
	[XmlRoot(Namespace = "urn:AVSyncer.Updates")]
	public class UpdateManifest {
		public UpdateManifest () { }

		[XmlElement]
		public List<SimpleUpdateInfo> Manifest;

		[XmlElement]
		public string CodeBase;

		[XmlAnyElementAttribute()]
		public System.Xml.XmlElement Signature;

		[XmlIgnore]
		public bool UpdateIsAvailableAndValid;
	}

	[XmlRoot(Namespace = "urn:AVSyncer.Updates")]
	public class SimpleUpdateInfo {
		public SimpleUpdateInfo () { }

		[XmlAttribute]
		public bool Archived;

		[XmlAttribute]
		public bool ForceUpdate;

		[XmlElement]
		public string LatestAvailableVersion;

		[XmlElement]
		public String ImageName;

		[XmlElement]
		public String AssemblyFullName;

		[XmlElement]
		public DateTime TimeStamp;

		[XmlElement(DataType = "hexBinary")]
		public byte[] Hash;

		[XmlElement]
		public String HashType;
	}
}

