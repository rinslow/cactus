/* The following bugs were written by d.kiss */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace play
{
	class Program
	{
		
		private const string HELP_LONG = "--help";
		private const string HELP_SHORT = "-h";
		private const string ADD_CMD = "add";
		private const string DELETE_CMD = "del";
		private const string REMOVE_CMD_LONG = "remove";
		private const string CLEAR_CMD = "clear";
		private const string REMOVE_CMD_SHORT = "rm";
		private const string EDIT_CMD = "edit";
		private const string VIEW_CMD = "view";
		private const string HELP_CMD = "help";
		private const string TAB = "    ";
		
		public const string ENVIROMENT_VAR_DB_PATH = "PLAY_DB_PATH";
		public const string DEFAULT_DB_PATH = "D:/play.txt";
			
		public const string PROGRAM_NAME = "play";
		public const string PROGRAM_DESCRIPTION = "Game shortcut manager.";
		
		public static void Main(string[] args)
		{
			if (args.Length == 0) {
				Console.WriteLine("no arguments supplied. try {0} --help for more help.", PROGRAM_NAME);
				return;
			}
			
			var db_path = get_db_path();
			try { 
			parse_command(args, db_path);
			} catch (Exception) {
				Console.WriteLine("Invalid syntax. try {0} --help for more help.", PROGRAM_NAME);
			}
		}

		public static string get_db_path( )
		{
			var db_path = Environment.GetEnvironmentVariable(ENVIROMENT_VAR_DB_PATH);
			if (db_path == null) {
				Environment.SetEnvironmentVariable(ENVIROMENT_VAR_DB_PATH, DEFAULT_DB_PATH);
				db_path = DEFAULT_DB_PATH;
			}
			
			if (!(System.IO.File.Exists(db_path))) {
				System.IO.File.WriteAllText(db_path, String.Empty);
		    }
			return db_path;
		}

		static void parse_command(string[] args, string db_path)
		{
			switch (args[0]) {
				case HELP_LONG:
				case HELP_SHORT: 
				case HELP_CMD:
					Console.WriteLine(help_message()); break;
					
				case VIEW_CMD:
					Console.WriteLine(db_content(db_path)); break;
					
				case ADD_CMD:
					add(db_path, args); break;
					
				case REMOVE_CMD_LONG:
				case REMOVE_CMD_SHORT:
				case DELETE_CMD:
					remove(db_path, args); break;
				
				case CLEAR_CMD:
					clear(db_path); break;
					
				case EDIT_CMD:
					edit(db_path, args); break;
					
				default:
					Process.Start(run_command(db_path, args)); break;
			}
		}
		
		public static string help_message()
		{
			var sb = new StringBuilder();
			sb.Append(PROGRAM_DESCRIPTION);
			sb.Append(Environment.NewLine);
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("Usage:"));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {2} {1} lol C:/Riot/LeagueOfLegends.exe", TAB, ADD_CMD, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {2} {1} lol C:/LeagueOfLegends.exe", TAB, EDIT_CMD, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {1} farcry4", TAB, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {1} help <- This help message.", TAB, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {2} {1} minecraft", TAB, REMOVE_CMD_LONG, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {2} {1}", TAB, VIEW_CMD, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}$ {2} {1}", TAB, CLEAR_CMD, PROGRAM_NAME));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("Note:"));
			sb.Append(Environment.NewLine);
			sb.Append(String.Format("{0}This tool configures to ${1} environment variable. " +
									"(default: {2})", TAB, ENVIROMENT_VAR_DB_PATH, DEFAULT_DB_PATH));
			return sb.ToString();
		}

		public static void add(string db_path, string[] args)
		{
			var name = args[1];
			var path = get_full_name(args, 2);
			File.AppendAllText(db_path, String.Format("{2}{0}={1}", name, path, 
			                                          is_empty(db_path) ? "" : ","));
		}
		
		public static bool is_empty(string db_path) {
			return File.ReadAllText(db_path) == string.Empty;
		}
		
		public static void remove(string db_path, string[] args)
		{
			IEnumerable<string> lines = File.ReadAllText(db_path).Split(',');
			string name_to_remove = args[1];
			
			var myRegex=new Regex(String.Format("{0}=.*?", name_to_remove));
			lines = lines.Where(line => !myRegex.IsMatch(line));
			
			File.WriteAllText(db_path, String.Join(",", lines));
		}

		public static void edit( string db_path, string[] args )
		{
			remove(db_path, args);
			add(db_path, args);
		}

		public static string run_command( string db_path, string[] args )
		{
			string alias_to_run = args[0];
			var txt = File.ReadAllText(db_path);
			foreach (var line in txt.Split(',')) {
				if (line.StartsWith(alias_to_run)) {
					var exe_path = line.Split('=')[1];
					return exe_path;
				}
			}
			
			throw new FileNotFoundException(String.Format("Could not find {0}.", alias_to_run));
		}

		public static void clear(string db_path)
		{
			File.WriteAllText(db_path, String.Empty);
		}
		
		public static string get_full_name(string[] args, int param_idx) {
			StringBuilder sb = new StringBuilder();
			for (int i=param_idx; i < args.Length; i++) {
				sb.Append(args[i]);
				sb.Append(" ");
			}
			sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}
		
		public static string db_content(string db_path) {
			var content = File.ReadAllText(db_path).Split(',');
			return String.Join(Environment.NewLine, content);
		}
	}
}
