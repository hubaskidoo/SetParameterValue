using Genetec.Sdk;
using Genetec.Sdk.Entities;
using Genetec.Sdk.Enumerations;
using Genetec.Sdk.Queries;
using Genetec.Sdk.Scripting;
using Genetec.Sdk.Scripting.Interfaces.Attributes;
using System;
using System.Data;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using System.IO;


[MacroParameters()]
public sealed class footester : UserMacro
{
	public string foo { get; set; }

	/// <summary>
	/// wrote this test Genetec Macro to figure out how to set input paramter value. 
	/// I needed the capability to clear input in another macro ran so input wasn't doubly applied if the macro was run again.
	/// </summary>
	Macro _footester = null;
	public override void Execute()
	{
		string logFile = @"C:\genetec\footest.log";
		StreamWriter file = System.IO.File.CreateText(logFile);
		try
		{
			file.WriteLine("---------- Macro started ---------- ");

			List<Guid> macros = GetAllEntity(EntityType.Macro);
			file.WriteLine(" found " + macros.Count + " macros.");

			foreach (Guid macroguid in macros)
			{
				Macro macro = Sdk.GetEntity(macroguid) as Macro;

				if (macro == null)
					continue;
				if (macro.Name.Contains("footester"))
				{
					file.WriteLine(" macro found is namedd " + macro.Name);

					Macro.ReadOnlyMacroParameterCollection macroParams = (macro.DefaultParameters);
					// 1. Create the input parameter collection

					file.WriteLine("got a ref to parameters.");
					foreach (var paramDef in macroParams)
					{
						file.WriteLine($" Param Name: {paramDef.Name}, Type: {paramDef.Type}, Value:{paramDef.Value}");
						macroParams["foo"].Value = "whoopi";
						macro.SetDefaultParameters(macroParams);
					}
				}
			}
		}
		catch (Exception ex)
		{
			file.WriteLine(ex.ToString());
		}
		finally
		{
			file.WriteLine("*** End of batch ***");
			file.WriteLine("---------- Macro stopped ---------- ");
			file.Close();
			file.Dispose();
		}

	}

	/// <summary>
	/// Called when the macro needs to clean up. 
	/// </summary>
	protected override void CleanUp()
	{
		// Release objects created by the Execute method, unhook events, and so on.
	}
	private List<Guid> GetAllEntity(EntityType entityType)
	{
		List<Guid> entityGuids = new List<Guid>();

		EntityConfigurationQuery query = Sdk.ReportManager.CreateReportQuery(ReportType.EntityConfiguration) as EntityConfigurationQuery;
		query.EntityTypes.Add(entityType);

		QueryCompletedEventArgs args = query.Query();

		if (args.Success)
		{
			foreach (DataRow row in args.Data.Rows)
			{
				entityGuids.Add((Guid)row["GUID"]);
			}
		}
		else
		{
			throw new Exception(entityType.ToString() + " query failed.");
		}

		return entityGuids;
	}
}
