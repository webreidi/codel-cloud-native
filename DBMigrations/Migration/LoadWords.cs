using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codele.ApiService.Data;
using DBMigrations.Models;

namespace Codele.ApiService.Migration;
	public class LoadWords
	{
		static void Main(string[] args)
		{
			var words = new []
			{
				"coder","write","debug","cobol","array","false","build","table","techy","razor","azure","agile","cloud","serve"
			};

			using (var db = new DataContext())
			{
				
				foreach (var word in words)
				{
					db.Answers.Add(new Words { Answer = word });
					db.SaveChanges();
				}
			}
		}
	}
