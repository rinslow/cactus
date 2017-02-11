/* The following bugs were written by d.kiss */
using System;
using NUnit.Framework;

namespace play
{
	[TestFixture]
	public class Program_unittest
	{
		private const string test_db_path = "D:/play_mock_db.txt";
		[SetUp]
		public void setUp() {
			if (!(System.IO.File.Exists(test_db_path))) {
				System.IO.File.WriteAllText(test_db_path, String.Empty);
		    }
		}
		
		[TearDown]
		public void tearDown() {
			System.IO.File.Delete(test_db_path);
		}
		
		[Test]
		public void Add()
		{
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			var content = Program.db_content(test_db_path);
			
			Assert.AreEqual("game_mock=C:/Game.exe", content);
			
		}
		
		[Test]
		public void AddTwo() {
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			Program.add(test_db_path, new string[] {"add", "game_mock2", "C:/Game2.exe"});
			var content = Program.db_content(test_db_path);
			
			Assert.AreEqual(String.Format("game_mock=C:/Game.exe{0}game_mock2=C:/Game2.exe", 
			                              Environment.NewLine), 
                            content);
		}
		
		[Test]
		public void AddAfterDelete() {
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			Program.add(test_db_path, new string[] {"add", "game_mock2", "C:/Game2.exe"});
			Program.remove(test_db_path, new string[] {"delete", "game_mock2" });
			
			var content = Program.db_content(test_db_path);
			Assert.AreEqual("game_mock=C:/Game.exe", content);
			
		}
		
		[Test]
		public void RunCommand() {
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			var cmd = Program.run_command(test_db_path, new string[] {"game_mock"});
			
			Assert.AreEqual("C:/Game.exe", cmd);
		}
		
		[Test] 
		public void RunCommandTwoValuesInDB() {
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			Program.add(test_db_path, new string[] {"add", "game_mock2", "C:/Game2.exe"});
			
			var cmd = Program.run_command(test_db_path, new string[] {"game_mock"});
			
			Assert.AreEqual("C:/Game.exe", cmd);
		}
		
		[Test]
		public void RunCommandAfterDeletion() {
			Program.add(test_db_path, new string[] {"add", "game_mock", "C:/Game.exe"});
			Program.add(test_db_path, new string[] {"add", "game_mock2", "C:/Game2.exe"});
			Program.remove(test_db_path, new string[] {"delete", "game_mock2" });
			
			var cmd = Program.run_command(test_db_path, new string[] {"game_mock"});
			Assert.AreEqual("C:/Game.exe", cmd);
		}
	}
}
