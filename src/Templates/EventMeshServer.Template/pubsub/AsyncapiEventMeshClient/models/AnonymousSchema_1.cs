namespace AsyncapiEventMeshClient.Models
{
  using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Linq;
  [JsonConverter(typeof(AnonymousSchema_1Converter))]
public class AnonymousSchema_1 {
  private string id;
  private string source;
  private string type;
  private string name;
  private Dictionary<string, dynamic> additionalProperties;

  public string Id 
  {
    get { return id; }
    set { id = value; }
  }

  public string Source 
  {
    get { return source; }
    set { source = value; }
  }

  public string Type 
  {
    get { return type; }
    set { type = value; }
  }

  public string Name 
  {
    get { return name; }
    set { name = value; }
  }

  public Dictionary<string, dynamic> AdditionalProperties 
  {
    get { return additionalProperties; }
    set { additionalProperties = value; }
  }
}

internal class AnonymousSchema_1Converter : JsonConverter<AnonymousSchema_1>
{
  public override bool CanConvert(System.Type objectType)
  {
    // this converter can be applied to any type
    return true;
  }
  public override AnonymousSchema_1 Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
  {
    if (reader.TokenType != JsonTokenType.StartObject)
    {
      throw new JsonException();
    }

    var instance = new AnonymousSchema_1();
  
    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
      {
        return instance;
      }

      // Get the key.
      if (reader.TokenType != JsonTokenType.PropertyName)
      {
        throw new JsonException();
      }

      string propertyName = reader.GetString();
      if (propertyName == "id")
      {
        var value = JsonSerializer.Deserialize<string>(ref reader, options);
        instance.Id = value;
        continue;
      }
      if (propertyName == "source")
      {
        var value = JsonSerializer.Deserialize<string>(ref reader, options);
        instance.Source = value;
        continue;
      }
      if (propertyName == "type")
      {
        var value = JsonSerializer.Deserialize<string>(ref reader, options);
        instance.Type = value;
        continue;
      }
      if (propertyName == "name")
      {
        var value = JsonSerializer.Deserialize<string>(ref reader, options);
        instance.Name = value;
        continue;
      }

    

      if(instance.AdditionalProperties == null) { instance.AdditionalProperties = new Dictionary<string, dynamic>(); }
      var deserializedValue = JsonSerializer.Deserialize<dynamic>(ref reader, options);
      instance.AdditionalProperties.Add(propertyName, deserializedValue);
      continue;
    }
  
    throw new JsonException();
  }
  public override void Write(Utf8JsonWriter writer, AnonymousSchema_1 value, JsonSerializerOptions options)
  {
    if (value == null)
    {
      JsonSerializer.Serialize(writer, null);
      return;
    }
    var properties = value.GetType().GetProperties().Where(prop => prop.Name != "AdditionalProperties");
  
    writer.WriteStartObject();

    if(value.Id != null) { 
      // write property name and let the serializer serialize the value itself
      writer.WritePropertyName("id");
      JsonSerializer.Serialize(writer, value.Id);
    }
    if(value.Source != null) { 
      // write property name and let the serializer serialize the value itself
      writer.WritePropertyName("source");
      JsonSerializer.Serialize(writer, value.Source);
    }
    if(value.Type != null) { 
      // write property name and let the serializer serialize the value itself
      writer.WritePropertyName("type");
      JsonSerializer.Serialize(writer, value.Type);
    }
    if(value.Name != null) { 
      // write property name and let the serializer serialize the value itself
      writer.WritePropertyName("name");
      JsonSerializer.Serialize(writer, value.Name);
    }


  

    // Unwrap additional properties in object
    if (value.AdditionalProperties != null) {
      foreach (var additionalProperty in value.AdditionalProperties)
      {
        //Ignore any additional properties which might already be part of the core properties
        if (properties.Any(prop => prop.Name == additionalProperty.Key))
        {
            continue;
        }
        // write property name and let the serializer serialize the value itself
        writer.WritePropertyName(additionalProperty.Key);
        JsonSerializer.Serialize(writer, additionalProperty.Value);
      }
    }

    writer.WriteEndObject();
  }

}

}