using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codele.ApiService.Data;
using Codele.ApiService.Models;
using System.Net.Sockets;

namespace Codele.ApiService.Migration;
	public class LoadWords
	{
		static void Main(string[] args, DataContext dataContext)
		{
			var words = new []
			{
				"coder","write","debug","cobol","array","false","build","table","techy","razor","azure","agile","cloud","serve"
			};

			foreach (var word in words)
			{
				Words newWord = new();
				dataContext.Answers.Add(newWord);
				dataContext.SaveChanges();
			}
	
		}
	}
