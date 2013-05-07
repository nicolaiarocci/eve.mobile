using System;
using System.Collections.Generic;
using System.Linq;
using eve.mobile.BL;
using eve.mobile.DL.SQLite;
using System.IO;

namespace eve.mobile.DL {
	/// <summary>
	/// TaskDatabase builds on SQLite.Net and represents a specific database, in our case, the MWC DB.
	/// It contains methods for retreival and persistance as well as db creation, all based on the 
	/// underlying ORM.
	/// </summary>
	public class EveDatabase : SQLiteConnection {
		protected static EveDatabase me = null;
		protected static string dbLocation;

        static object locker = new object ();
		
		/// <summary>
		/// Initializes a new instance of the <see cref="eve.mobile.DL.EveDatabse"/> EveDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		protected EveDatabase (string path) : base (path)
		{
			// create the tables
//			CreateTable<Exhibitor> ();
//			CreateTable<Session> ();			
			CreateTable<Person> ();
//			CreateTable<Favorite> ();
			
			// FK
//			CreateTable<SessionSpeaker> ();

			// these are really for caches
//			CreateTable<Tweet> ();
//			CreateTable<RSSEntry> ();
		}

		static EveDatabase ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;
			
			// instantiate a new db
			me = new EveDatabase(dbLocation);
		}
		
		public static string DatabaseFilePath {
			get { 
#if SILVERLIGHT
            var path = "Eve.db3";
#else

#if __ANDROID__
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
			// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
			// (they don't want non-user-generated data in Documents)
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "../Library/");
#endif
			var path = Path.Combine (libraryPath, "Eve.db3");
#endif		
			return path;	
}
		}

		public static IEnumerable<T> GetItems<T> () where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return (from i in me.Table<T> () select i).ToList ();
            }
		}
		
		public static T GetItem<T> (int id) where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                
                // ---
                //return (from i in me.Table<T> ()
                //        where i.ID == id
                //        select i).FirstOrDefault ();

                // +++ To properly use Generic version and eliminate NotSupportedException
                // ("Cannot compile: " + expr.NodeType.ToString ()); in SQLite.cs
                return me.Table<T>().FirstOrDefault(x => x.ID == id);
            }
		}
		
		public static int SaveItem<T> (T item) where T : BL.Contracts.IBusinessEntity
		{
            lock (locker) {
                if (item.ID != 0) {
                    me.Update (item);
                    return item.ID;
                } else {
                    return me.Insert (item);
                }
            }
		}
		
		public static void SaveItems<T> (IEnumerable<T> items) where T : BL.Contracts.IBusinessEntity
		{
            lock (locker) {
                me.BeginTransaction ();

                foreach (T item in items) {
                    SaveItem<T> (item);
                }

                me.Commit ();
            }
		}

		// HACK: fixes UNHANDLED EXCEPTION: System.NotSupportedException: Cannot store type: MWC.BL.Favorite
//		public static int DeleteFavorite(int id)
//		{
//			lock (locker) {
//				return me.Delete(new Favorite() { ID = id });
//			}
//		}

		public static int DeleteItem<T>(int id) where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                return me.Delete<T> (new T () { ID = id });
            }
		}
		
		public static void ClearTable<T>() where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
                me.Execute (string.Format ("delete from \"{0}\"", typeof (T).Name));
            }
		}
		
		// helper for checking if database has been populated
		public static int CountTable<T>() where T : BL.Contracts.IBusinessEntity, new ()
		{
            lock (locker) {
				string sql = string.Format ("select count (*) from \"{0}\"", typeof (T).Name);
				var c = me.CreateCommand (sql, new object[0]);
				return c.ExecuteScalar<int>();
            }
		}
		
//		public static IEnumerable<T> Query<T>(string query)
//		{
//			return _me.Query.
//		}
		
//		public static IEnumerable<Session> GetSessionsByStartDate(DateTime dateMin, DateTime dateMax)
//		{
//            lock (locker) {
//                return (from i in me.Table<Session> ()
//                        where i.Start >= dateMin && i.Start <= dateMax
//                        select i).ToList ();
//            }		
//		}
//
//
//        /*
//         * the following two queries are currently required because the Generic versions throw
//         * an exception on this line in SQLite.cs (Android ONLY)
//         * 1565:  throw new NotSupportedException ("Cannot compile: " + expr.NodeType.ToString ());
//		 *
//		 * ALSO now I've added some additional processing to the Session and Speaker to 'join' them
//         */
//		/// <summary>
//		/// Gets the Session AND linked Speakers
//		/// </summary>
//        public static Session GetSession (int id)
//        {
//            lock (locker) {
//				Session session = (from s in me.Table<Session> ()
//                        where s.ID == id
//                        select s).FirstOrDefault ();
//				try{ // bug occurs in simulator...???
//				session.SpeakerKeys = (from ss in me.Table<SessionSpeaker> ()
//									where ss.SessionKey == session.Key
//									select ss.SpeakerKey).ToList();
//				var speakers = GetItems<Speaker>();
//
//				var speakerInSession = (from sp in speakers
//								where session.SpeakerKeys.Contains (sp.Key)
//								select sp).ToList ();
//
//				session.Speakers = speakerInSession;
//					} catch {}
//				return session;
//            }
//        }
//		/// <summary>
//		/// Gets the Session AND linked Speakers
//		/// </summary>
//        public static Session GetSessionWithKey (string key)
//        {
//            lock (locker) {
//				Session session = (from s in me.Table<Session> ()
//                        where s.Key == key
//                        select s).FirstOrDefault ();
//
//				session.SpeakerKeys = (from ss in me.Table<SessionSpeaker> ()
//									where ss.SessionKey == session.Key
//									select ss.SpeakerKey).ToList();
//				var speakers = GetItems<Speaker>();
//
//				var speakersInSession = (from sp in speakers
//								where session.SpeakerKeys.Contains (sp.Key)
//								select sp).ToList ();
//
//				session.Speakers = speakersInSession;
//
//				return session;
//            }
//        }
		/// <summary>
		/// Gets the Speaker AND linked Sessions
		/// </summary>
        public static Person GetPerson(int id)
        {
            lock (locker) {
                Person person = (from p in me.Table<Person> ()
                        where p.ID == id
                        select p).FirstOrDefault ();

//				var keys = (from ss in me.Table<SessionSpeaker> ()
//									where ss.SpeakerKey == speaker.Key
//									select ss).ToList();
//				// HACK: gets around "Default constructor not found for type System.String" error
//				speaker.SessionKeys = keys.Select (x => x.SpeakerKey).ToList ();
//
//				var sessions = GetItems<Session>();
//
//				var sessionsForSpeaker = (from se in sessions
//								where speaker.SessionKeys.Contains (se.Key)
//								select se).ToList ();
//
//				speaker.Sessions = sessionsForSpeaker;

				return person;
            }
        }
		/// <summary>
		/// Gets the Speaker AND linked Sessions
		/// </summary>
        public static Person GetSpeakerWithKey (string key)
        {
            lock (locker) {
				Person person= (from s in me.Table<Person> ()
                        where s.Key == key
                        select s).FirstOrDefault ();

//				speaker.SessionKeys = (from ss in me.Table<SessionSpeaker> ()
//									where ss.SpeakerKey == speaker.Key
//									select ss.SessionKey).ToList();
//				var sessions = GetItems<Session>();
//
//				var sessionsForSpeaker = (from se in sessions
//								where speaker.SessionKeys.Contains (se.Key)
//								select se).ToList ();
//
//				speaker.Sessions = sessionsForSpeaker;

				return person;
            }
        }
//        public static Exhibitor GetExhibitor(int id)
//        {
//            lock (locker) {
//                //return DL.MwcDatabase.GetItem<Exhibitor> (id);
//                return (from s in me.Table<Exhibitor> ()
//                        where s.ID == id
//                        select s).FirstOrDefault ();
//            }
//        }
//        public static Exhibitor GetExhibitorWithName (string name)
//        {
//            lock (locker) {
//                return (from s in me.Table<Exhibitor> ()
//                        where s.Name == name
//                        select s).FirstOrDefault ();
//            }
//        }
//        public static Tweet GetTweet(int id)
//        {
//            lock (locker) {
//                //return DL.MwcDatabase.GetItem<Tweet> (id);
//                return (from s in me.Table<Tweet> ()
//                        where s.ID == id
//                        select s).FirstOrDefault ();
//            }
//        }
//        public static RSSEntry GetNews(int id)
//        {
//            lock (locker) {
//                //return DL.MwcDatabase.GetItem<RSSEntry> (id);
//                return (from s in me.Table<RSSEntry> ()
//                        where s.ID == id
//                        select s).FirstOrDefault ();
//            }
//        }
	}
}