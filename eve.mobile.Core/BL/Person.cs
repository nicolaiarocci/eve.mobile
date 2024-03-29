using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace eve.mobile.BL
{
	public partial class Person: Contracts.BusinessEntityBase
	{
		[XmlAttribute("k")]
		public string Key { get; set; }
		[XmlAttribute("n")]
		public string Name { get; set; }
		[XmlAttribute("t")]
		public string Title { get; set; }
		[XmlAttribute("c")]
		public string Company { get; set; }
		[XmlAttribute("b")]
		public string Bio { get; set; }
		[XmlAttribute("i")]
		public string ImageUrl { get; set; }
		
		// These are ONLY POPULATED on the client-side, when a single Speaker is requested
		[eve.mobile.DL.SQLite.Ignore]
		public List<string> SessionKeys { get; set; }
		
//		[XmlIgnore]
//		[eve.mobile.DL.SQLite.Ignore]
//		public List<Session> Sessions { get; set; }

		public Person ()
		{
		}

        public string Index {
            get {
                return Name.Length == 0 ? "A" : Name[0].ToString().ToUpper();
            }
        }
	}
}

