// Sets a macro's paramater value.
// Inputs: macro name, parameter name and new parameter value.
using System.Data;

void SetParameterValue(String macroName, string paramName, string paramValue)
{ 
	List<Guid> macros = GetAllEntity(EntityType.Macro);
	 
	foreach (Guid macroguid in macros)
	{
		Macro macro = Sdk.GetEntity(macroguid) as Macro;

		if (macro == null)
			continue;
		if (macro.Name.Contains(macroName))
		{
			Macro.ReadOnlyMacroParameterCollection macroParams = (macro.DefaultParameters);
			foreach (var paramDef in macroParams)
			{
				macroParams[paramName].Value = paramValue;
				macro.SetDefaultParameters(macroParams);
			}
		}
	}
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