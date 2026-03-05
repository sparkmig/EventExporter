// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using System.Text.Json.Serialization;
//use this enpoint to get the json file: https://---.crm4.dynamics.com/api/data/v9.0/EntityDefinitions(LogicalName='annotation')/Attributes?$select=LogicalName,SchemaName,AttributeType,AttributeTypeName
//the files end up in the same folder as the executable, so you can just run the program and it will read the json file and output the cs file in the same folder

string fileName = "msevtmgt_event";
string filePath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.json");
string outputPath = Path.Combine(Environment.CurrentDirectory, $"{fileName}.cs");

string json = File.ReadAllText(filePath);
var model = JsonSerializer.Deserialize<CsvModel>(json) ?? throw new Exception("shit was null");
var formattedFile = Format(model);

File.WriteAllText(outputPath, formattedFile);

string Format(CsvModel model)
{
    return $$"""

    public class {{fileName}}
    {
    {{string.Join(Environment.NewLine, model.Value.Select(p => $"   [EntityAttribute(\"{p.LogicalName}\")] public {p.ConvertedAttributeType} {p.SchemaName} {{ get; set; }} {Environment.NewLine}"))}}
    }
    
    """;
}

public class CsvModel
{
    [JsonPropertyName("value")]
    public List<ModelProp> Value { get; set; } = new List<ModelProp>();
}

public class ModelProp
{
    public string LogicalName { get; set; }
    public string AttributeType { get; set; }
    public string SchemaName { get; set; }

    public string ConvertedAttributeType { get {
            switch (AttributeType)
            {
                case "Lookup":
                    return "Guid";
                case "Integer":
                    return "int?";
                case "Boolean":
                    return "bool?";
                case "Money":
                    return "decimal?";
                default:
                    return $"{AttributeType}?";
            }
        } }

}