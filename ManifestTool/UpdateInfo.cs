// UpdateInfo.cs
// ------------------------------------------------------------------
//
// built on host: DINOCH-2
// Created Mon Aug 10 20:12:07 2009
//
// last saved: 
// Time-stamp: <2009-August-10 20:21:37>
// ------------------------------------------------------------------
//
// Copyright (c) 2009 by Dino Chiesa
// All rights reserved!
//
// ------------------------------------------------------------------

using System;
using System.Xml.Serialization;
using System.Collections.Generic;


namespace Ionic.AppUpdater {
	[XmlRoot(Namespace = "urn:Ionic.Apps.Updates")]
	public class UpdateInfo {
		public UpdateInfo () { TimeStamp = System.DateTime.Now; }

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

		[XmlElement]
		public string DownloadLocation;

		[System.Xml.Serialization.XmlAnyElementAttribute()]
		public System.Xml.XmlElement Signature;

		[XmlIgnore]
		public bool UpdateIsAvailableAndValid;
	}

	[XmlRoot(Namespace = "urn:AVSyncer.Updates")]
	public class UpdateManifest {
		public UpdateManifest () { }

		[XmlElement]
		public List<SimpleUpdateInfo> Manifest;

		[XmlElement]
		public string CodeBase;

		[System.Xml.Serialization.XmlAnyElementAttribute()]
		public System.Xml.XmlElement Signature;

		[XmlIgnore]
		public bool UpdateIsAvailableAndValid;
	}

	[XmlRoot(Namespace = "urn:AVSyncer.Updates")]
	public class SimpleUpdateInfo {
		public SimpleUpdateInfo () { }

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

