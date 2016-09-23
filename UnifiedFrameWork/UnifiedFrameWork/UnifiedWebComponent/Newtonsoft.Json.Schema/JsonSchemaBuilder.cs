using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaBuilder
	{
		private readonly IList<JsonSchema> _stack;

		private readonly JsonSchemaResolver _resolver;

		private readonly IDictionary<string, JsonSchema> _documentSchemas;

		private JsonSchema _currentSchema;

		private JObject _rootSchema;

		private JsonSchema CurrentSchema
		{
			get
			{
				return this._currentSchema;
			}
		}

		public JsonSchemaBuilder(JsonSchemaResolver resolver)
		{
			this._stack = new List<JsonSchema>();
			this._documentSchemas = new Dictionary<string, JsonSchema>();
			this._resolver = resolver;
		}

		private void Push(JsonSchema value)
		{
			this._currentSchema = value;
			this._stack.Add(value);
			this._resolver.LoadedSchemas.Add(value);
			this._documentSchemas.Add(value.Location, value);
		}

		private JsonSchema Pop()
		{
			JsonSchema currentSchema = this._currentSchema;
			this._stack.RemoveAt(this._stack.Count - 1);
			this._currentSchema = this._stack.LastOrDefault<JsonSchema>();
			return currentSchema;
		}

		internal JsonSchema Read(JsonReader reader)
		{
			JToken jToken = JToken.ReadFrom(reader);
			this._rootSchema = (jToken as JObject);
			JsonSchema jsonSchema = this.BuildSchema(jToken);
			this.ResolveReferences(jsonSchema);
			return jsonSchema;
		}

		private string UnescapeReference(string reference)
		{
			return Uri.UnescapeDataString(reference).Replace("~1", "/").Replace("~0", "~");
		}

		private JsonSchema ResolveReferences(JsonSchema schema)
		{
			if (schema.DeferredReference != null)
			{
				string text = schema.DeferredReference;
				bool flag = text.StartsWith("#", StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					text = this.UnescapeReference(text);
				}
				JsonSchema jsonSchema = this._resolver.GetSchema(text);
				if (jsonSchema == null)
				{
					if (flag)
					{
						string[] array = schema.DeferredReference.TrimStart(new char[]
						{
							'#'
						}).Split(new char[]
						{
							'/'
						}, StringSplitOptions.RemoveEmptyEntries);
						JToken jToken = this._rootSchema;
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++)
						{
							string reference = array2[i];
							string text2 = this.UnescapeReference(reference);
							if (jToken.Type == JTokenType.Object)
							{
								jToken = jToken[text2];
							}
							else if (jToken.Type == JTokenType.Array || jToken.Type == JTokenType.Constructor)
							{
								int num;
								if (int.TryParse(text2, out num) && num >= 0 && num < jToken.Count<JToken>())
								{
									jToken = jToken[num];
								}
								else
								{
									jToken = null;
								}
							}
							if (jToken == null)
							{
								break;
							}
						}
						if (jToken != null)
						{
							jsonSchema = this.BuildSchema(jToken);
						}
					}
					if (jsonSchema == null)
					{
						throw new JsonException("Could not resolve schema reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, schema.DeferredReference));
					}
				}
				schema = jsonSchema;
			}
			if (schema.ReferencesResolved)
			{
				return schema;
			}
			schema.ReferencesResolved = true;
			if (schema.Extends != null)
			{
				for (int j = 0; j < schema.Extends.Count; j++)
				{
					schema.Extends[j] = this.ResolveReferences(schema.Extends[j]);
				}
			}
			if (schema.Items != null)
			{
				for (int k = 0; k < schema.Items.Count; k++)
				{
					schema.Items[k] = this.ResolveReferences(schema.Items[k]);
				}
			}
			if (schema.AdditionalItems != null)
			{
				schema.AdditionalItems = this.ResolveReferences(schema.AdditionalItems);
			}
			if (schema.PatternProperties != null)
			{
				foreach (KeyValuePair<string, JsonSchema> current in schema.PatternProperties.ToList<KeyValuePair<string, JsonSchema>>())
				{
					schema.PatternProperties[current.Key] = this.ResolveReferences(current.Value);
				}
			}
			if (schema.Properties != null)
			{
				foreach (KeyValuePair<string, JsonSchema> current2 in schema.Properties.ToList<KeyValuePair<string, JsonSchema>>())
				{
					schema.Properties[current2.Key] = this.ResolveReferences(current2.Value);
				}
			}
			if (schema.AdditionalProperties != null)
			{
				schema.AdditionalProperties = this.ResolveReferences(schema.AdditionalProperties);
			}
			return schema;
		}

		private JsonSchema BuildSchema(JToken token)
		{
			JObject jObject = token as JObject;
			if (jObject == null)
			{
				throw JsonException.Create(token, token.Path, "Expected object while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
			}
			JToken value;
			if (jObject.TryGetValue("$ref", out value))
			{
				return new JsonSchema
				{
					DeferredReference = (string)value
				};
			}
			string text = token.Path.Replace(".", "/").Replace("[", "/").Replace("]", string.Empty);
			if (!string.IsNullOrEmpty(text))
			{
				text = "/" + text;
			}
			text = "#" + text;
			JsonSchema result;
			if (this._documentSchemas.TryGetValue(text, out result))
			{
				return result;
			}
			this.Push(new JsonSchema
			{
				Location = text
			});
			this.ProcessSchemaProperties(jObject);
			return this.Pop();
		}

		private void ProcessSchemaProperties(JObject schemaObject)
		{
			foreach (KeyValuePair<string, JToken> current in schemaObject)
			{
				string key;
				switch (key = current.Key)
				{
				case "type":
					this.CurrentSchema.Type = this.ProcessType(current.Value);
					break;
				case "id":
					this.CurrentSchema.Id = (string)current.Value;
					break;
				case "title":
					this.CurrentSchema.Title = (string)current.Value;
					break;
				case "description":
					this.CurrentSchema.Description = (string)current.Value;
					break;
				case "properties":
					this.CurrentSchema.Properties = this.ProcessProperties(current.Value);
					break;
				case "items":
					this.ProcessItems(current.Value);
					break;
				case "additionalProperties":
					this.ProcessAdditionalProperties(current.Value);
					break;
				case "additionalItems":
					this.ProcessAdditionalItems(current.Value);
					break;
				case "patternProperties":
					this.CurrentSchema.PatternProperties = this.ProcessProperties(current.Value);
					break;
				case "required":
					this.CurrentSchema.Required = new bool?((bool)current.Value);
					break;
				case "requires":
					this.CurrentSchema.Requires = (string)current.Value;
					break;
				case "minimum":
					this.CurrentSchema.Minimum = new double?((double)current.Value);
					break;
				case "maximum":
					this.CurrentSchema.Maximum = new double?((double)current.Value);
					break;
				case "exclusiveMinimum":
					this.CurrentSchema.ExclusiveMinimum = new bool?((bool)current.Value);
					break;
				case "exclusiveMaximum":
					this.CurrentSchema.ExclusiveMaximum = new bool?((bool)current.Value);
					break;
				case "maxLength":
					this.CurrentSchema.MaximumLength = new int?((int)current.Value);
					break;
				case "minLength":
					this.CurrentSchema.MinimumLength = new int?((int)current.Value);
					break;
				case "maxItems":
					this.CurrentSchema.MaximumItems = new int?((int)current.Value);
					break;
				case "minItems":
					this.CurrentSchema.MinimumItems = new int?((int)current.Value);
					break;
				case "divisibleBy":
					this.CurrentSchema.DivisibleBy = new double?((double)current.Value);
					break;
				case "disallow":
					this.CurrentSchema.Disallow = this.ProcessType(current.Value);
					break;
				case "default":
					this.CurrentSchema.Default = current.Value.DeepClone();
					break;
				case "hidden":
					this.CurrentSchema.Hidden = new bool?((bool)current.Value);
					break;
				case "readonly":
					this.CurrentSchema.ReadOnly = new bool?((bool)current.Value);
					break;
				case "format":
					this.CurrentSchema.Format = (string)current.Value;
					break;
				case "pattern":
					this.CurrentSchema.Pattern = (string)current.Value;
					break;
				case "enum":
					this.ProcessEnum(current.Value);
					break;
				case "extends":
					this.ProcessExtends(current.Value);
					break;
				case "uniqueItems":
					this.CurrentSchema.UniqueItems = (bool)current.Value;
					break;
				}
			}
		}

		private void ProcessExtends(JToken token)
		{
			IList<JsonSchema> list = new List<JsonSchema>();
			if (token.Type == JTokenType.Array)
			{
				using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)token).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JToken current = enumerator.Current;
						list.Add(this.BuildSchema(current));
					}
					goto IL_61;
				}
			}
			JsonSchema jsonSchema = this.BuildSchema(token);
			if (jsonSchema != null)
			{
				list.Add(jsonSchema);
			}
			IL_61:
			if (list.Count > 0)
			{
				this.CurrentSchema.Extends = list;
			}
		}

		private void ProcessEnum(JToken token)
		{
			if (token.Type != JTokenType.Array)
			{
				throw JsonException.Create(token, token.Path, "Expected Array token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
			}
			this.CurrentSchema.Enum = new List<JToken>();
			foreach (JToken current in ((IEnumerable<JToken>)token))
			{
				this.CurrentSchema.Enum.Add(current.DeepClone());
			}
		}

		private void ProcessAdditionalProperties(JToken token)
		{
			if (token.Type == JTokenType.Boolean)
			{
				this.CurrentSchema.AllowAdditionalProperties = (bool)token;
				return;
			}
			this.CurrentSchema.AdditionalProperties = this.BuildSchema(token);
		}

		private void ProcessAdditionalItems(JToken token)
		{
			if (token.Type == JTokenType.Boolean)
			{
				this.CurrentSchema.AllowAdditionalItems = (bool)token;
				return;
			}
			this.CurrentSchema.AdditionalItems = this.BuildSchema(token);
		}

		private IDictionary<string, JsonSchema> ProcessProperties(JToken token)
		{
			IDictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
			if (token.Type != JTokenType.Object)
			{
				throw JsonException.Create(token, token.Path, "Expected Object token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
			}
			using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)token).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					JProperty jProperty = (JProperty)enumerator.Current;
					if (dictionary.ContainsKey(jProperty.Name))
					{
						throw new JsonException("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, jProperty.Name));
					}
					dictionary.Add(jProperty.Name, this.BuildSchema(jProperty.Value));
				}
			}
			return dictionary;
		}

		private void ProcessItems(JToken token)
		{
			this.CurrentSchema.Items = new List<JsonSchema>();
			switch (token.Type)
			{
			case JTokenType.Object:
				this.CurrentSchema.Items.Add(this.BuildSchema(token));
				this.CurrentSchema.PositionalItemsValidation = false;
				return;
			case JTokenType.Array:
				this.CurrentSchema.PositionalItemsValidation = true;
				using (IEnumerator<JToken> enumerator = ((IEnumerable<JToken>)token).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JToken current = enumerator.Current;
						this.CurrentSchema.Items.Add(this.BuildSchema(current));
					}
					return;
				}
				break;
			}
			throw JsonException.Create(token, token.Path, "Expected array or JSON schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
		}

		private JsonSchemaType? ProcessType(JToken token)
		{
			JTokenType type = token.Type;
			if (type == JTokenType.Array)
			{
				JsonSchemaType? jsonSchemaType = new JsonSchemaType?(JsonSchemaType.None);
				foreach (JToken current in ((IEnumerable<JToken>)token))
				{
					if (current.Type != JTokenType.String)
					{
						throw JsonException.Create(current, current.Path, "Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
					}
					jsonSchemaType |= JsonSchemaBuilder.MapType((string)current);
				}
				return jsonSchemaType;
			}
			if (type != JTokenType.String)
			{
				throw JsonException.Create(token, token.Path, "Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, token.Type));
			}
			return new JsonSchemaType?(JsonSchemaBuilder.MapType((string)token));
		}

		internal static JsonSchemaType MapType(string type)
		{
			JsonSchemaType result;
			if (!JsonSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out result))
			{
				throw new JsonException("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));
			}
			return result;
		}

		internal static string MapType(JsonSchemaType type)
		{
			return JsonSchemaConstants.JsonSchemaTypeMapping.Single((KeyValuePair<string, JsonSchemaType> kv) => kv.Value == type).Key;
		}
	}
}
